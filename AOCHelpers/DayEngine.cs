using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AOCHelpers
{
    public class DayEngine : IDisposable
    {
        // default path
        private string DataPath => $"../../../Day{Day,2}/";
        // default input cache name
        private string DataCacheFileName => Path.Combine(DataPath, $"AocDay{Day,2}-MyInput.txt");
        // AoC day main URL
        private static string Url => $"https://adventofcode.com/{Year}/day/";
        // AoC answers RegEx
        private static readonly Regex GoodAnswer = new (".*That's the right answer!.*");
        private static readonly Regex TooSoon = new (".*You have (\\d*)m? (\\d*)s? left to wait\\..*");
        private readonly string _sessionId;
        private int _currentDay;
        private Task<string> _myData;
        private Task _pendingWrite;

        private readonly Dictionary<int, List<(string data, object result)>> _testData = new();
        private HttpClientHandler _handler;
        private HttpClient _client;

        /// <summary>
        /// Gets/sets the current day 
        /// </summary>
        public int Day
        {
            get => _currentDay;
            set
            {
                if (value == _currentDay)
                {
                    return;
                }
                _currentDay = value;
                _testData.Clear();
                InitiatePersonalInputFetching();
            }
        }

        /// <summary>
        /// Gets/Sets the current year
        /// </summary>
        public static int Year { get; set; }

        /// <summary>
        /// Gets the personal AoC input (either from aoc website or local cache if it exists)
        /// </summary>
        /// <remarks>Data fetching is initiated earlier.</remarks>
        private string MyData
        {
            get
            {
                var result = _myData.Result;
                if (File.Exists(DataCacheFileName))
                {
                    return result;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(DataCacheFileName) ?? throw new InvalidOperationException());
                _pendingWrite= File.WriteAllTextAsync(DataCacheFileName, result);

                return result;
            }
        }


        public DayEngine()
        {
            _sessionId = Environment.GetEnvironmentVariable("AOC_SESSION");
            if (string.IsNullOrEmpty(_sessionId))
            {
                throw new InvalidOperationException(
                    "AOC_SESSION environment variable must contain an Advent Of Code session id.");
            }
        }
        /// <summary>
        /// Runs a given day.
        /// </summary>
        /// <typeparam name="T"><see cref="Algorithm"/> type for the day.</typeparam>
        /// <exception cref="InvalidOperationException">when the method fails to create an instance of the algorithm.</exception>
        public void RunDay<T>() where T:Algorithm
        {
            var dayAlgoType = typeof(T);
            var constructorInfo = dayAlgoType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo?.Invoke(null) is not T dayAlgo)
            {
                throw new InvalidOperationException($"Failed to build the algorithm instance. Make sure there is a parameterless constructor for {nameof(T)}.");
            }
            dayAlgo.SetupRun(this);
            
            // tests if data are provided
            if (_testData.Count > 0)
            {
                if (!RunTests(constructorInfo))
                {
                    // tests have failed, no need to continue
                    return;
                }
            }
            // perform the actual run
            dayAlgo.Parse(MyData);
            if (CheckResponse(1, dayAlgo.GetAnswer1())) return;
            if (CheckResponse(2, dayAlgo.GetAnswer2())) return;
            EnsureDataIsCached();
        }

        private bool CheckResponse(int id, object answer2)
        {
            if (answer2 == null)
            {
                Console.Error.WriteLine($"No answer provided! Please overload GetAnswer{id}() with your code.");
                return false;
            }
            Console.WriteLine($"Day {Day}-{id}: {answer2} [{Year}].");
            return !PostAnswer(id, answer2.ToString());
        }

        private void EnsureDataIsCached()
        {
            // ensure data are cached properly
            if (_pendingWrite == null)
            {
                return;
            }
            // wait for cache writing completion
            if (!_pendingWrite.IsCompleted)
            {
                _pendingWrite.Wait(500);
            }

            _pendingWrite = null;
        }

        private bool RunTests(ConstructorInfo constructor)
        {
            Console.WriteLine("*** Run tests ***");
            var algorithms = new Dictionary<string, Algorithm>();
            if (!RunTest(1, constructor, algorithms)) return false;
            if (!RunTest(2, constructor, algorithms)) return false;
            Console.WriteLine("*** Tests succeeded ***");
            return true;
        }

        private static object GetAnswer(int id, Algorithm algorithm)
        {
            return id == 1 ? algorithm.GetAnswer1() : algorithm.GetAnswer2();
        }
        
        private bool RunTest(int id, ConstructorInfo constructor, Dictionary<string, Algorithm> algorithms)
        {
            if (!_testData.ContainsKey(id))
            {
                return true;
            }
            var success = true;
            Console.WriteLine($"* Test question {id} *");
            foreach (var (data, expected) in GetTestInfo(id))
            {
                // gets a cached algorithm if any
                var testAlgo = algorithms.GetValueOrDefault(data);
                if (testAlgo == null)
                {
                    testAlgo = constructor.Invoke(null) as Algorithm;
                    if (testAlgo == null)
                    {
                        throw new Exception($"Can't build an instance of {constructor.DeclaringType?.Name}.");
                    }
                    testAlgo.Parse(data);
                    algorithms[data] = testAlgo;
                }
                var answer = GetAnswer(id, testAlgo);
                if (!answer.Equals(expected))
                {
                    Console.Error.WriteLine($"Test failed: got {answer} instead of {expected} using:");
                    Console.Error.WriteLine(data);
                    success = false;
                }
                else
                {
                    Console.WriteLine($"Test success: got {answer} using:");
                    Console.WriteLine(data);
                }
            }

            return success;
        }

        /// <summary>
        /// Get the test data.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IEnumerable<(string data, object result)> GetTestInfo(int id)
        {
            for(var i = 0; i<_testData[id].Count; i++)
            {
                var (data, result) = _testData[id][i];
                if (string.IsNullOrEmpty(data))
                {
                    if (id == 1)
                    {
                        throw new Exception($"Can't test question 1 for expected {result}. No associated test data.");
                    }

                    if (_testData[1].Count <= i)
                    {
                        throw new Exception($"Can't test question 2 for expected {result}. No associated test data (incl. for question 1).");
                    }
                    // fetch data used for question 1.
                    data = _testData[1][i].data;
                }

                yield return (data, result);
            }
        }


        private static bool IsAcceptableInAFileName(string name) => name.Length <= 20 && name.All(character => char.IsDigit(character) || char.IsLetter(character) || "-_#*".Contains(character));

        /// <summary>
        /// Posts an answer to the AoC website.
        /// </summary>
        /// <param name="question">question id (1 or 2)</param>
        /// <param name="value">proposed answer (as a string)</param>
        /// <returns>true if this is the good answer</returns>
        /// <remarks>Posted answers are cached locally to avoid stressing the AoC website.
        /// The result of previous answers is stored as an html file. The filename depends on the question and the provided answer; it contains either
        /// the answer itself, or its hash if the answer cannot be part of a filename.
        /// You can examine the response file to get details and/or removes them to resubmit a previously send proposal.</remarks>
        private bool PostAnswer(int question, string value)
        {
            var answerId = value;
            if (!IsAcceptableInAFileName(answerId))
            {
                answerId = answerId.GetHashCode().ToString();
            }
            var responseFilename = Path.Combine(DataPath, $"Answer{question} for {answerId}.html");
            var responseText = PostAndRetrieve(question, value, responseFilename, out var responseTime);
            // extract the response as plain text
            var resultText = ExtractAnswerText(responseText);
            OutputAoCMessage(resultText);
            // did we answer too fast?
            var match = TooSoon.Match(resultText);
            while (match.Success)
            {
                // we need to wait.
                responseTime += TimeSpan.FromSeconds(int.Parse(match.Groups[2].Value))+TimeSpan.FromMinutes(int.Parse(match.Groups[1].Value));
                Console.WriteLine($"Wait until {responseTime}.");
                // wait until we can try again
                do
                {
                    Thread.Sleep(1000);
                } while (responseTime>= DateTime.Now);
                // delete any cached response to ensure we post again
                File.Delete(responseFilename);
                // send our new answer
                responseText = PostAndRetrieve(question, value, responseFilename, out responseTime);
                OutputAoCMessage(resultText);
                match = TooSoon.Match(resultText);
            }
            // is it the correct answer ?
            var result = GoodAnswer.IsMatch(resultText);
            if (!result)
            {
            }
            
            if (_pendingWrite is { IsCompleted: false })
            {
                // await the ongoing write
                _pendingWrite.Wait();
            }

            _pendingWrite = File.WriteAllTextAsync(responseFilename, responseText);
            return result;
        }

        private static void OutputAoCMessage(string resultText)
        {
            Console.WriteLine("AoC site response: '{0}'", resultText);
        }

        private string PostAndRetrieve(int question, string value, string responseFilename, out DateTime responseTime)
        {
            string responseText;
            // if we already have the response file...
            if (File.Exists(responseFilename))
            {
                Console.WriteLine($"Response {value} as already been attempted.");
                responseText = File.ReadAllText(responseFilename);
                responseTime = File.GetLastWriteTime(responseFilename);
            }
            else
            {
                var url = $"{Url}{Day}/answer";
                var data = new Dictionary<string, string>
                {
                    ["answer"] = value,
                    ["level"] = question.ToString()
                };
                var response = _client.PostAsync(url, new FormUrlEncodedContent(data));
                responseText = response.Result.Content.ReadAsStringAsync().Result;
                responseTime = DateTime.Now;
            }

            return responseText;
        }

        private static string ExtractAnswerText(string response)
        {
            var start = response.IndexOf("<article>", StringComparison.InvariantCulture);
            if (start == -1)
            {
                Console.Error.WriteLine("Failed to parse response.");
                return response;
            }

            start += 9;
            var end = response.IndexOf("</article>", start, StringComparison.InvariantCulture);
            if (end == -1)
            {
                Console.Error.WriteLine("Failed to parse response.");
                return response;
            }
            response = response.Substring(start, end - start);
            return Regex.Replace(response,@"<(.|\n)*?>",string.Empty);
        }

        /// <summary>
        /// Retrieves personal data (associated to the AoC session ID).
        /// </summary>
        /// <remarks>Input retrieval is done asynchronously, so it can happen in parallel with testing.</remarks>
        private void InitiatePersonalInputFetching()
        {
            _handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
            _client = new HttpClient(_handler);
            // add our identifier to the request
            _handler.CookieContainer.Add(new Cookie("session",
                _sessionId, "/", ".adventofcode.com"));
            var fileName = DataCacheFileName;
            _myData = File.Exists(fileName) ? File.ReadAllTextAsync(fileName) : _client.GetStringAsync($"{Url}{Day}/input");
        }

        /// <summary>
        /// Registers test data so that they are used.
        /// </summary>
        /// <param name="question">question id (1 or 2)</param>
        /// <param name="data">input data as a string.</param>
        /// <param name="expected">expected result (either string or a number).</param>
        public void RegisterTestData(int question, string data, object expected)
        {
            if (!_testData.ContainsKey(question))
            {
                _testData[question] = new List<(string data, object result)>();
            }
            _testData[question].Add((data, expected));
        }

        /// <summary>
        /// Cleanup data. Mainly ensure that ongoing writes are persisted, if any, and closes the HTTP session.
        /// </summary>
        public void Dispose()
        {
            _myData?.Dispose();
            _pendingWrite?.Dispose();
            _handler?.Dispose();
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
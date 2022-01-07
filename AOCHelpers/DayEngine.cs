using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace AOCHelpers
{
    public abstract class Algorithm
    {
        public abstract void ParseLine(string line, int index, int lineCount);

        public virtual object GetAnswer1()
        {
            return "undef";
        }

        public virtual object GetAnswer2()
        {
            return "undef";
        }

        public abstract void SetupRun(DayEngine dayEngine);

        public void Parse(string data)
        {
            var lines = data.Split('\n');
            // we discard the last line if it is empty (trailing newline), but we keep any internal newlines
            if (lines[^1].Length == 0)
            {
                lines = lines[0..^1];
            }
            var index=0;
            foreach (var line in lines)
            {
                ParseLine(line, index++, lines.Length);
            }
        }
    }
    
    public class DayEngine : IDisposable
    {
        private readonly string _sessionId;
        private int _currentDay;
        private Task<string> _myData;
        private Task _pendingWrite;

        private readonly Dictionary<int, List<(string data, object result)>> _testData = new();
        private HttpClientHandler _handler;
        private HttpClient _client;

        public int Day
        {
            get => _currentDay;
            set
            {
                if (value != _currentDay)
                {
                    _currentDay = value;
                    _testData.Clear();
                    FetchPersonalInput();
                }                
            }
        }

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
                if (!File.Exists(DataCacheFileName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(DataCacheFileName) ?? throw new InvalidOperationException());
                    _pendingWrite= File.WriteAllTextAsync(DataCacheFileName, result);
                }

                return result;
            }
        }

        public void RunDay<T>() where T:Algorithm
        {
            var dayAlgoType = typeof(T);
            var constructorInfo = dayAlgoType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo?.Invoke(null) is not T dayAlgo)
            {
                throw new InvalidOperationException("Failed to build the algorithm instance.");
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
            var answer1 = dayAlgo.GetAnswer1();
            
            Console.WriteLine($"Day {Day}-1: {answer1} [{Year}].");
            if (!PostAnswer(1, answer1.ToString()))
            {
                Console.WriteLine("Sorry, this is not the right answer for (1).");
                return;
            }
            Console.WriteLine("Good answer for (1).");
            var answer2 = dayAlgo.GetAnswer2();
            Console.WriteLine($"Day {Day}-2: {answer2} [{Year}].");
            if (!PostAnswer(2, answer2.ToString()))
            {
                Console.WriteLine("Sorry, this is not the right answer for (2).");
                return;
            }
            Console.WriteLine("Good answer for (2).");
            // ensure data are cached properly
            if (_pendingWrite != null)
            {
                // wait for cache writing completion
                if (!_pendingWrite.IsCompleted)
                {
                    _pendingWrite.Wait(500);
                }
                _pendingWrite = null;
            }
        }

        private bool RunTests(ConstructorInfo constructor)
        {
            Console.WriteLine("*** Run tests ***");
            var algorithms = new Dictionary<string, Algorithm>();
            if (_testData.ContainsKey(1))
            {
                var success = true;
                Console.WriteLine("* Test question 1 *");
                foreach (var (data, expected) in _testData[1])
                {
                    var testAlgo = constructor.Invoke(null) as Algorithm;
                    Debug.Assert(testAlgo!=null);
                    algorithms.Add(data, testAlgo);
                    testAlgo.Parse(data);
                    var answer = testAlgo.GetAnswer1();
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

                if (!success)
                {
                    return false;
                }
            }

            if (_testData.ContainsKey(2))
            {
                Console.WriteLine("* Test question 2 *");
                var success = true;
                for (var i = 0; i < _testData[2].Count; i++)
                {
                    var (data, expected) = _testData[2][i];
                    if (data == null)
                    {
                        // reuse data for test one
                        data = _testData[1][i].data;
                    }

                    Algorithm testAlgo = algorithms.GetValueOrDefault(data);
                    if (testAlgo == null)
                    {
                        testAlgo = constructor.Invoke(null) as Algorithm;
                        testAlgo.Parse(data);
                    }
                    var answer = testAlgo.GetAnswer2();
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
                if (!success)
                {
                    return false;
                }
            }
            Console.WriteLine("*** Tests succeeded ***");
            return true;
        }

        private string DataPath => $"../../../Day{Day,2}/";
        private string DataCacheFileName => Path.Combine(DataPath, $"AocDay{Day,2}-MyInput.txt");

        private string Url => $"https://adventofcode.com/{Year}/day/";

        public DayEngine()
        {
            _sessionId = Environment.GetEnvironmentVariable("AOC_SESSION");
            if (string.IsNullOrEmpty(_sessionId))
            {
                throw new InvalidOperationException(
                    "AOC_SESSION environment variable must contain an Advent Of Code session id.");
            }
        }

        private static bool IsAcceptableInAFileName(string name) => name.Length <= 20 && name.All(character => char.IsDigit(character) || char.IsLetter(character) || "-_#*".Contains(character));

        private bool PostAnswer(int question, string value)
        {
            var data = new Dictionary<string, string>
            {
                ["answer"] = value,
                ["level"] = question.ToString()
            };
            var url = $"{Url}{Day}/answer";
            var answerId = value;
            if (!IsAcceptableInAFileName(answerId))
            {
                answerId = answerId.GetHashCode().ToString();
            }
            var responseFilename = Path.Combine(DataPath, $"Answer{question} for {answerId}.html");
            string responseText;
            // if we already have the response file...
            if (File.Exists(responseFilename))
            {
                Console.WriteLine($"Response {value} as already been attempted.");
                responseText = File.ReadAllText(responseFilename);
            }
            else
            {
                var response = _client.PostAsync(url, new FormUrlEncodedContent(data));

                responseText = response.Result.Content.ReadAsStringAsync().Result;
            }
            var result = responseText.Contains("That's the right answer!");
            if (_pendingWrite is { IsCompleted: false })
            {
                // await the ongoing write
                _pendingWrite.Wait();
            }

            _pendingWrite = File.WriteAllTextAsync(responseFilename, responseText);
            return result;
        }
        
        private void FetchPersonalInput()
        {
            _handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
            _client = new HttpClient(_handler);
            // add our identifier to the request
            _handler.CookieContainer.Add(new Cookie("session",
                _sessionId, "/", ".adventofcode.com"));
            var fileName = DataCacheFileName;
            _myData = File.Exists(fileName) ? File.ReadAllTextAsync(fileName) : _client.GetStringAsync($"{Url}{Day}/input");
        }

        public void RegisterTestData(int question, string data, object expected)
        {
            if (!_testData.ContainsKey(question))
            {
                _testData[question] = new List<(string data, object result)>();
            }
            _testData[question].Add((data, expected));
        }

        public void Dispose()
        {
            _myData?.Dispose();
            _pendingWrite?.Dispose();
            _handler?.Dispose();
            _client?.Dispose();
        }
    }
}
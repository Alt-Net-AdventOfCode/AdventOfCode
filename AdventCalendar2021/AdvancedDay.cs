using System;
using System.IO;
using System.Net;
using System.Net.Http;
using AOCHelpers;

namespace AdventCalendar2021
{
    public abstract class AdvancedDay : DupdobDayWithTest
    {
        private const int Year = 2021;
        private readonly string _sessionId;
        protected AdvancedDay(int day)
        {
            Day = day;
            _sessionId = Environment.GetEnvironmentVariable("AOC_SESSION");
            if (string.IsNullOrEmpty(_sessionId))

                throw new InvalidOperationException(
                    "AOC_SESSION environment variable must contain an Advent Of Code session id.");
        }

        protected override string Input
        {
            get
            {
                // did we already got our data?
                var input = GetAocInputFile($"../../../Day{Day,2}/", _sessionId, Day);
                return input;
            }
        }

        /// <summary>
        /// Returns AdventOfCode input data, locally cached
        /// </summary>
        /// <param name="pathName">Path were the data are stored</param>
        /// <param name="sessionId">session identifier (use your own id, stored in AoC session cookie)</param>
        /// <param name="day">Day number (1-24)</param>
        /// <returns>Your input data.</returns>
        /// <remarks>The first call for a given will get the data from AoC site and cache it locally. If the file
        /// exist, returns the file content.
        /// </remarks>
        static string GetAocInputFile(string pathName, string sessionId, int day)
        {
            string input;
            var fileName =  Path.Combine( pathName, $"AocDay{day,2}-MyInput.txt");
            if (File.Exists(fileName))
            {
                // get the data we already fetched
                input = File.ReadAllText(fileName);
            }
            else
            {
                var uri = new Uri($"https://adventofcode.com/{Year}/day/{day}/input");
                using var handler = new HttpClientHandler() { CookieContainer = new CookieContainer() };
                using var client = new HttpClient(handler);
                // add our identifier to the request
                handler.CookieContainer.Add(new Cookie("session",
                    sessionId, "/", ".adventofcode.com"));
                // get our data
                input = client.GetStringAsync(uri).Result;
                // save it for the next run
                File.WriteAllText(fileName, input);
            }

            return input;
        }

        public override int Day { get; }
    }
}
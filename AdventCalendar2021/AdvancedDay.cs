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
        private const string SessionId = "53616c7465645f5fb4c94bfcf4deb0c66bff4f49ffd026330b69d806269a4f31ebe0f5a60f696c11fe2e96a5f02a6fbb";
        protected AdvancedDay(int day)
        {
            Day = day;
        }

        protected override string Input
        {
            get
            {
                // did we already got our data?
                var input = GetAocInputFile("../../../", SessionId, Day);
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
            var fileName =  Path.Combine( pathName, $"AocDay{day}-MyInput.txt");
            if (!File.Exists(fileName))
            {
                var uri = new Uri($"https://adventofcode.com/2021/day/{day}/input");
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
            else
            {
                // get the data we already fetched
                input = File.ReadAllText(fileName);
            }

            return input;
        }

        public override int Day { get; }
    }
}
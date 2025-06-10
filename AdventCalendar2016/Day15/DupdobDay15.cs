// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
// ---
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2016
{
    public class DupdobDay15 : SolverWithLineParser
    {
        private static Regex Parser =
            new Regex("Disc #(-?\\d+) has (-?\\d+) positions; at time=(-?\\d+), it is at position (-?\\d+).");

        private List<(int max, int current)> _positions = new();

        public override void SetupRun(DayAutomaton dayAutomaton)
        {
            dayAutomaton.Day = 15;
            dayAutomaton.RegisterTestDataAndResult(@"Disc #1 has 5 positions; at time=0, it is at position 4.
Disc #2 has 2 positions; at time=0, it is at position 1.
", 5, 1);
        }

        public override object GetAnswer1()
        {
            var time = 0;
            var repeatAfter = 1;
            for (var index = 0; index < _positions.Count; index++)
            {
                var scan = _positions[index].max;
                var firstTime = scan - _positions[index].current-index-1;
                if (firstTime < 0)
                {
                    firstTime += scan;
                }
                while (firstTime != time)
                {
                    if (firstTime < time)
                    {
                        // the fist time the capsule can slide in is earlier than the current evaluated time
                        firstTime += scan;
                    }
                    else
                    {
                        // the current time is before the first time the capsule can slide in
                        time += repeatAfter;
                    }
                }

                repeatAfter *= scan;
            }

            return time;
        }

        public override object GetAnswer2()
        {
            _positions.Add((11, 0));
            return GetAnswer1();
        }

        protected override void ParseLine(string line, int index, int lineCount)
        {
            var match = Parser.Match(line);
            if (!match.Success)
            {
                return;
            }
            // capture positions
            _positions.Add((int.Parse(match.Groups[2].Value), int.Parse(match.Groups[4].Value)));
        }
    }
}
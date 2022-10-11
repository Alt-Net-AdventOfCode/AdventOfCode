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

using System.Text;
using AoC;

namespace AdventCalendar2016
{
    public class DupdobDay16 : SolverWithLineParser
    {
        private string _input;
        private int _targetLength = 272;

        public override void SetupRun(Automaton automaton)
        {
            automaton.Day = 16;
            automaton.RegisterTestDataAndResult(@"10000
20", "01100", 1);
        }

        private string Transform(string input)
        {
            var output = new StringBuilder(input);
            output.Append('0');
            for (var i = input.Length-1; i >=0; i--)
            {
                output.Append(input[i] == '0' ? '1' : '0');
            }

            return output.ToString();
        }

        private string Checksum(string input)
        {
            var output = new StringBuilder(input.Length / 2);
            for (var i = 0; i < input.Length; i+=2)
            {
                output.Append(input[i] == input[i + 1] ? '1' : '0');
            }

            return output.ToString();
        }

        public override object GetAnswer1()
        {
            var input = _input;
            while (input.Length < _targetLength)
            {
                input = Transform(input);
            }

            input = input.Substring(0, _targetLength);
            while (input.Length % 2 == 0)
            {
                input = Checksum(input);
            }

            return input;
        }

        public override object GetAnswer2()
        {
            _targetLength = 35651584;
            return GetAnswer1();
        }

        protected override void ParseLine(string line, int index, int lineCount)
        {
            if (index == 0)
            {
                _input = line;
            }
            else if (!string.IsNullOrEmpty(line))
            {
                _targetLength = int.Parse(line);
            }
        }
    }
}
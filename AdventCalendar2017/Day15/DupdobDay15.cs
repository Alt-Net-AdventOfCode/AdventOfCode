// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
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
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2017;

public class DupdobDay15 : SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 15;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                Generator A starts with 65
                                                Generator B starts with 8921
                                                """, 588, 1).RegisterTestResult(309,2);
    }

    public override object GetAnswer1()
    {
        long a = _A;
        long b = _B;
        var matches = 0;
        for (var i = 0; i < 40000000; i++)
        {
            a = (a * 16807) % 2147483647;
            b = (b * 48271) % 2147483647;
            if ((a & 0xFFFF) == (b & 0xFFFF))
            {
                matches++;
            }
        }

        return matches;
    }

    public override object GetAnswer2()
    {
        long a = _A;
        long b = _B;
        var matches = 0;
        for (var i = 0; i < 5000000; i++)
        {
            do
            {
                a = (a * 16807) % 2147483647;
            }
            while (a % 4 != 0);
            do
            {
                b = (b * 48271) % 2147483647;
            }
            while (b % 8 != 0);
            if ((a & 0xFFFF) == (b & 0xFFFF))
            {
                matches++;
            }
        }

        return matches;
    }

    private int _A;
    private int _B;
    
    protected override void Parse(string data) => ParseLines(data.SplitLines());

    private void ParseLines(string[] lines)
    {
        var parser = new Regex(@"Generator (\w+) starts with (\d+)");
        foreach (var line in lines)
        {
            var match = parser.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse {0}", line);
                continue;
            }

            var val = int.Parse(match.Groups[2].Value);
            if (match.Groups[1].Value == "A")
            {
                _A = val;
            }
            else
            {
                _B = val;
            }
        }
    }
}
// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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

using AoC;

namespace AdventCalendar2023;

public class DupdobDay1 : SolverWithLineParser
{
    private List<string> _lines = new();
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 1;
        automatonBase.RegisterTestDataAndResult(@"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet", 142, 1);
        automatonBase.RegisterTestDataAndResult(@"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen", 281, 2);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        foreach (var line in _lines)
        {
            var score = 0;
            var index = 0;
            while (index<line.Length && (line[index] < '0' || line[index] > '9'))
            {
                index++;
            }

            if (index == line.Length)
            {
                continue;
            }
            score = (line[index] - '0') * 10;
            index = line.Length - 1;
            while (line[index] < '0' || line[index] > '9')
            {
                index--;
            }

            score += line[index] - '0';
            result += score;
        }

        return result;
    }

    public override object GetAnswer2()
    {
        var digits = new[]
        {
            "1", "one", "2", "two", "3", "three", "4", "four", "5", "five", "6", "six", "7", "seven", "8", "eight", "9",
            "nine"
        };
        var result = 0;
        foreach (var line in _lines)
        {
            var score = 0;
            var found = false;
            for (var i = 0; i < line.Length && !found; i++)
            {
                for (var j = 0; j < digits.Length; j++)
                {
                    if (line[i..].StartsWith(digits[j]))
                    {
                        score = (j/2+1)*10;
                        found = true;
                        break;
                    }
                }
            }
            
            found = false;
            for (var i = line.Length-1; i >= 0 && !found; i--)
            {
                for (var j = 0; j < digits.Length; j++)
                {
                    if (line[i..].StartsWith(digits[j]))
                    {
                        score += j/2+1;
                        found = true;
                        break;
                    }
                }
            }

            result += score;
        }

        return result;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;
        _lines.Add(line);
    }
}
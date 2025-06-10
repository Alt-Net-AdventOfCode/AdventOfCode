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
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay25 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 25;
        dayAutomaton.AddExample(@"1=-0-2
12111
2=0=
21
2=01
111
20012
112
1=-1=
1-12
12
1=
122");
        dayAutomaton.RegisterTestResult("2=-1=0");
    }

    private const string Digits = "=-012";
    
    public override object GetAnswer1() => IntToSnafu(_list.Sum(SnafuToInt));

    private static string IntToSnafu(long score)
    {
        var digitCount = 0L;
        var rank = 1L;
        var level = new Dictionary<long, long>
        {
            [0] = 0
        };
        while (digitCount<score)
        {
            digitCount+=2*rank;
            level[rank] = digitCount;
            rank *= 5;
        }
        // we step down one rank
        rank/=5;
        var currentDigit = 0;
        var result = string.Empty;
        while (true)
        {
            while (Math.Abs(score) <= level[rank/5])
            {
                result += Digits[currentDigit+2];
                currentDigit = 0;
                rank /= 5;
                if (rank == 0)
                {
                    break;
                }
            }

            if (score == 0 && rank == 0)
            {
                break;
            }
            if (score > 0)
            {
                currentDigit++;
                score -= rank;
            }
            else
            {
                score += rank;
                currentDigit--;
            }
        }
        
        return result;
    }

    private static long SnafuToInt(string number)
    {
        var total = 0L;
        var rank = 1L;
        foreach (var digit in number.Reverse())
        {
            total += (Digits.IndexOf(digit)-2)*rank;
            rank *= 5;
        }

        return total;
    }

    public override object GetAnswer2()
    {
        return "ok";
    }

    private readonly List<string> _list = new();
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _list.Add(line);
    }
}
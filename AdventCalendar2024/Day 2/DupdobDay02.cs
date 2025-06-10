// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2024;

public class DupdobDay02 : SolverWithLineParser
{
    private readonly List<List<int>> _numbers = new();
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 2;
        
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                7 6 4 2 1
                                                1 2 7 8 9
                                                9 7 6 2 1
                                                1 3 2 4 5
                                                8 6 4 4 1
                                                1 3 6 7 9
                                                """
            , 2, 1);
        dayAutomatonBase.RegisterTestResult(4, 2);
    }

    public override object GetAnswer1()
    {
        return _numbers.Count(ReportIsSafe);
    }

    private static bool ReportIsSafe(List<int> line)
    {
        var dir = line[0].CompareTo(line[1]);
        if (dir == 0 || (line[0]-line[1])*dir>3)
            return false;
        for (var i = 1; i < line.Count-1; i++)
        {
            var step = (line[i] - line[i + 1])*dir;
            if (step is >= 1 and <= 3) continue;
            return false;
        }
        return true;
    }

    public override object GetAnswer2()
    {
        var score = 0;
        foreach (var line in _numbers)
        {
            if (ReportIsSafe(line))
            {
                score++;
                continue;
            }

            for (var i = 0; i < line.Count; i++)
            {
                var temp = new List<int>(line);
                temp.RemoveAt(i);
                if (!ReportIsSafe(temp)) continue;
                score++;
                break;
            }
        }
        return score;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _numbers.Add(line.Split(' ').Select(int.Parse).ToList());
    }
}
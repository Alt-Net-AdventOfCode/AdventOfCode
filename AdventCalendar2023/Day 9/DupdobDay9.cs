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

using System.Xml;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay9 : SolverWithLineParser
{
    private List<List<long>> _numbers = new();
    
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 9;
        dayAutomatonBase.RegisterTestDataAndResult(@"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45", 114, 1);
        dayAutomatonBase.RegisterTestResult(2, 2);
        
    }

    public override object GetAnswer1()
    {
        var result = 0L;
        foreach (var list in _numbers)
        {
            result += Next(list);
        }

        return result;
    }

    private long Next(List<long> list)
    {
        var first = list[0];
        if (list.All(x => x == first))
        {
            return first;
        }
        else
        {
            // we need to compute the diffs
            var diffList = new List<long>(list.Count);
            for (var i = 1; i < list.Count; i++)
            {
                diffList.Add(list[i]-list[i-1]);
            }
            return list.Last() + Next(diffList);
        }
    }

    public override object GetAnswer2()
    {
        var result = 0L;
        foreach (var list in _numbers)
        {
            result += Previous(list);
        }

        return result;
    }

    private long Previous(List<long> list)
    {
        var first = list[0];
        if (list.All(x => x == first))
        {
            return first;
        }
        else
        {
            // we need to compute the diffs
            var diffList = new List<long>(list.Count);
            for (var i = 1; i < list.Count; i++)
            {
                diffList.Add(list[i]-list[i-1]);
            }
            return list[0] - Previous(diffList);
        }
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;
        _numbers.Add(line.Split(' ', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList());
    }
}
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

using System;
using System.Collections.Generic;
using System.Linq;
using AoC;

namespace AdventCalendar2016;

public class DupdobDay20 : SolverWithLineParser
{
    private List<(uint low, uint high)> _ranges = new();
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 20;
        automaton.RegisterTest(@"5-8
0-2
4-7").Answer1(3);
    }

    public override object GetAnswer1()
    {
        var merged = new List<(uint low, uint high)>();
        foreach (var range in _ranges)
        {
            var tryToMerge = true;
            var rangeToMerge = range;
            while (tryToMerge)
            {
                tryToMerge = false;
                for (var i = 0; i < merged.Count; i++)
                {
                    var currentRange = merged[i];
                    if ((range.low > 0 && range.low-1 > currentRange.high) || (currentRange.low>0 && range.high < currentRange.low-1))
                    {
                        continue;
                    }
                    // we can create a new merged range
                    rangeToMerge = (Math.Min(rangeToMerge.low, currentRange.low), Math.Max(rangeToMerge.high, currentRange.high));
                    tryToMerge = true;
                    merged.RemoveAt(i--);
                }
            }
            merged.Add(rangeToMerge);
        }

        _ranges = merged.OrderBy(range => range.low).ToList();
        return merged[0].low > 0 ? 0 : merged[0].high + 1;
    }

    public override object GetAnswer2()
    {
        var count = 0u;
        (uint low, uint high) lastRange = (0, uint.MaxValue);
        foreach (var range in _ranges)
        {
            count += range.low - lastRange.high-1;
            lastRange = range;
        }

        count += uint.MaxValue - lastRange.high;
        return count;
    } 

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        var limits = line.Split('-');
        _ranges.Add((uint.Parse(limits[0]), uint.Parse(limits[1])));
    }
}
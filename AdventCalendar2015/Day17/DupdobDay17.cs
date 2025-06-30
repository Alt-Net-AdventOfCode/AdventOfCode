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

using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(17)]
public class DupdobDay17: SolverWithParam<int>
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    protected override void Parse(string data) => _eggNogContainer = data.SplitLines().Select(int.Parse).ToArray();

    private int[] _eggNogContainer = null;
    [Example(1,"""
             20
             15
             10
             5
             5
             """, 4, 25)]
    protected override object GetAnswer1(int targetSize = 150) => PossibleSizes(0).GetValueOrDefault(targetSize).count;

    private Dictionary<int, (int count, int minSize, int minCount)> PossibleSizes(int i)
    {
        if (_cache.TryGetValue(i, out var result))
        {
            return result;
        }
        var thisContainer = _eggNogContainer[i];
        if (i==_eggNogContainer.Length-1)
        {
            return new Dictionary<int, (int count, int minSize, int minCount)> { { thisContainer, (1, 1, 1) } };
        }
        var source = PossibleSizes(i + 1);
        result = source.ToDictionary();
        foreach (var (value, count) in source)
        {
            result[value+thisContainer] = MergeCounts(value+thisContainer, (count.count, count.minSize+1, count.minCount));
        }

        result[thisContainer] = MergeCounts(thisContainer, (1, 1, 1));
        _cache[i] = result;
        return result;

        (int count, int minSize, int minCount) MergeCounts(int value, (int count, int minSize, int minCount) count)
        {
            (int count, int minSize, int minCount) defaultSlot = (0, 0, 0);
            var extract = result.GetValueOrDefault(value, defaultSlot);
            if (extract.minSize > count.minSize || extract.minSize == 0)
            {
                extract = (extract.count+count.count, count.minSize, count.minCount);
            }
            else if (extract.minSize == count.minSize)
            {
                extract = (extract.count+count.count, extract.minSize, extract.minCount + count.minCount);
            }
            else
            {
                extract = (count.count + extract.count, extract.minSize, extract.minCount);
            }

            return extract;
        }
    }

    private readonly Dictionary<int, Dictionary<int, (int count, int minSize, int minCount)>> _cache = new();
    
    [ReuseExample(1, 3)]
    protected override object GetAnswer2(int targetSize = 150) => PossibleSizes(0).GetValueOrDefault(targetSize).minCount;
}
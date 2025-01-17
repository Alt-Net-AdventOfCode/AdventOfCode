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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay24 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 24;
        automatonBase.AddExample("""
                                 0/2
                                 2/2
                                 2/3
                                 3/4
                                 3/5
                                 0/1
                                 10/1
                                 9/10
                                 """).Answer1(31).Answer2(19);
    }

    public override object GetAnswer1()
    {
        var subPath = Recurse(_bridges, 0, 0, 0);

        _subPathLongestWeight = subPath.longestWeight;

        return subPath.weight;
    }

    private static (int weight, int longest, int longestWeight) Recurse(IEnumerable<(int start, int end)> list, int weight, int len, int next)
    {
        var maxWeight = weight;
        var longest= ++len;
        var longestWeight = weight;
        foreach (var (start, end)  in list)
        {
            if (start != next && end != next)
            {
                continue;
            }
            
            var nextList = list.Except([(start, end)]);
            var subPath = Recurse(nextList, weight+start+end, len, next == start ? end : start);
            if (maxWeight < subPath.weight)
            {
                maxWeight = subPath.weight;
            }

            if (longest >= subPath.longest &&
                (longest != subPath.longest || longestWeight >= subPath.longestWeight)) continue;
            longest = subPath.longest;
            longestWeight = subPath.longestWeight;
        }

        return (maxWeight, longest, longestWeight);
    }

    public override object GetAnswer2()
    {
        return _subPathLongestWeight;
    }

    private readonly HashSet<(int start, int end)> _bridges = [];
    private int _subPathLongestWeight;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var ends = line.Split('/', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        if (!_bridges.Add((ends[0], ends[1])))
        {
            throw new NotSupportedException("Does not support duplicate bridge.");
        }
    }
}


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

public class DupdobDay05: SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 5;
        automatonBase.RegisterTestDataAndResult("""
                                                47|53
                                                97|13
                                                97|61
                                                97|47
                                                75|29
                                                61|13
                                                75|53
                                                29|13
                                                97|29
                                                53|29
                                                61|53
                                                97|53
                                                61|29
                                                47|13
                                                75|47
                                                97|75
                                                47|61
                                                75|61
                                                47|29
                                                75|13
                                                53|13

                                                75,47,61,53,29
                                                97,61,53,29,13
                                                75,29,13
                                                75,97,47,61,53
                                                61,13,29
                                                97,13,75,29,47
                                                """, 143, 1);
        automatonBase.RegisterTestResult(123, 2);
    }

    private readonly Dictionary<int, List<int>> _order = new();
    private readonly List<int[]> _lists = [];
    private bool _parsingLists = false;
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line))
        {
            _parsingLists = true;
            return;
        }

        if (_parsingLists)
        {
            _lists.Add(line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
        }
        else
        {
            var parts = line.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var key = int.Parse(parts[0]);
            var value = int.Parse(parts[1]);
            if (!_order.TryGetValue(key, out var list))
            {
                list = [];
                _order[key] = list;
            }
            list.Add(value);
        }
    }

    public override object GetAnswer1()
    {
        return _lists.Where(IsInValidOrder).Sum(list => list[(list.Length - 1) / 2]);
    }

    private bool IsInValidOrder(int[] list)
    {
        for (var i = 0; i < list.Length; i++)
        {
            var current = list[i];
            for (var j = i + 1; j < list.Length; j++)
            {   
                if (_order.TryGetValue(list[j], out var order) && order.Contains(current))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override object GetAnswer2()
    {
        var result = 0;
        foreach (var list in _lists.Where(l => !IsInValidOrder(l)))
        {
            // we assume we have complete ordering
            var fixedList = new int[list.Length];
            foreach (var entry in list)
            {
                int countOf;
                if (_order.TryGetValue(entry, out var successors))
                {
                    countOf = list.Count( e => successors.Contains(e));
                }
                else
                {
                    countOf = 0;
                }
                fixedList[list.Length-countOf-1] = entry;
            }

            if (!IsInValidOrder(fixedList))
            {
                throw new Exception("Algo is invalid");
            }
            result += fixedList[(list.Length - 1)/2];
        }
        return result;
    }

}
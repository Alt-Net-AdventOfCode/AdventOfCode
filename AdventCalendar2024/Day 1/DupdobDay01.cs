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

public class DupdobDay01 : SolverWithLineParser
{
    private readonly List<int> _first = new();
    private readonly List<int> _second = new();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 1;
        automatonBase.RegisterTestDataAndResult("""
                                                3   4
                                                4   3
                                                2   5
                                                1   3
                                                3   9
                                                3   3
                                                """, 11, 1);
        automatonBase.RegisterTestResult(31, 2);
    }

    public override object GetAnswer1()
    {
        var orderedFirst = _first.Order().ToList();
        var orderedSecond = _second.Order().ToList();
        var result = 0L;
        for (var i = 0; i < orderedFirst.Count; i++)
        {
            result += Math.Abs(orderedFirst[i] - orderedSecond[i]);
        }
        return result;
    }

    public override object GetAnswer2()
    {
        var result = 0L;
        var countMap = new Dictionary<int, int>();
        foreach (var entry in _second.Where(entry => !countMap.TryAdd(entry, 1)))
        {
            countMap[entry]++;
        }

        foreach (var entry in _first)
        {
            if (countMap.TryGetValue(entry, out var times))
            {
                result += entry * times;
            }    
        }
        return result;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var numbers = line.Split(" ", StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
        _first.Add(int.Parse(numbers[0]));
        _second.Add(int.Parse(numbers[1]));
    }
}
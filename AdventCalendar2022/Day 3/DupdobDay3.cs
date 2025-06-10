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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay3 : SolverWithLineParser
{
    private readonly List<string> _list = new();
    
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 3;
        dayAutomaton.RegisterTestDataAndResult(@"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw", 157, 1);
        dayAutomaton.RegisterTestDataAndResult(@"vJrwpWtwJgWrhcsFMMfFFhFp
jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
PmmdzqPrVvPwwTWBwg
wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
ttgJtRGJQctTZtZT
CrZsJsPPZsGzwwsLwLmpwMDw", 70, 2);
    }

    private static int Priority(char car) => car + (car >= 'a' ? 1 - 'a' : 27 - 'A');

    public override object GetAnswer1()
    {
        var result = 0;
        foreach (var line  in _list)
        {
            var (left, right) = (line[..(line.Length / 2)], line[(line.Length / 2)..]);
            foreach (var item in left.Where(item => right.Contains(item)))
            {
                // found duplicate
                result += Priority(item);
                break;
            }
        }

        return result;
    }

    public override object GetAnswer2()
    {
        var result = 0;
        for (var i = 0; i < _list.Count; i+=3)
        {
            foreach (var item in _list[i].Where(item => _list[i + 1].Contains(item) && _list[i + 2].Contains(item)))
            {
                result += Priority(item);
                break;
            }
        }

        return result;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }
        _list.Add(line);
    }
}   
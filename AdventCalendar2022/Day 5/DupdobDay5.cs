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

using System.Text;
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2022;

public class DupdobDay5 : SolverWithLineParser
{
    private readonly List<List<char>> _stacks = new();
    private readonly Regex _instruction = new Regex("move (\\d*) from (\\d*) to (\\d*)", RegexOptions.Compiled);
    
    private readonly List<(int repeat, int from, int to)> _operations = new();
    
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 5;
        dayAutomaton.RegisterTestDataAndResult(@"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
", "CMZ", 1);        dayAutomaton.RegisterTestDataAndResult(@"    [D]    
[N] [C]    
[Z] [M] [P]
 1   2   3 

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2
", "MCD", 2);
    }

    public override object GetAnswer1()
    {
        var stacks = _stacks.Select(stack => stack.ToList()).ToList();
        foreach (var (repeat, from, to)  in _operations)
        {
            for (var j = 0; j < repeat; j++)
            {
                var crate = stacks[from - 1][0];
                stacks[from-1].RemoveAt(0);
                stacks[to - 1].Insert(0, crate);
            }
        }

        return TopOfStacks(stacks);
    }

    private static object TopOfStacks(List<List<char>> stacks)
    {
        var result = new StringBuilder(stacks.Count);
        foreach (var t in stacks)
        {
            result.Append(t[0]);
        }

        return result.ToString();
    }

    public override object GetAnswer2()
    {
        var stacks = _stacks.Select(stack => stack.ToList()).ToList();
        foreach (var (repeat, from, to)  in _operations)
        {
            stacks[to - 1].InsertRange(0, stacks[from-1].GetRange(0, repeat));
            stacks[from-1].RemoveRange(0, repeat);
        }

        return TopOfStacks(stacks);
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (line.Contains('['))
        {
            // crates
            for (var i = 0; i < (line.Length+1)/4; i++)
            {
                var entry = line[1 + i * 4];
                if (entry == ' ')
                {
                    continue;
                }

                if (_stacks.Count <= i)
                {
                    for (var j = _stacks.Count; j <= i; j++)
                    {
                        _stacks.Add(new List<char>());
                    }
                }
                _stacks[i].Add(entry);
            }
            return;
        }

        var match = _instruction.Match(line);
        if (!match.Success)
        {
            return;
        }

        _operations.Add((int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
            int.Parse(match.Groups[3].Value)));
    }
}
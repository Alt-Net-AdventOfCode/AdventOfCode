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

public class DupdobDay1 : SolverWithLineParser
{
    private readonly List<List<int>> _calories = new();
    private readonly List<int> _totals = new();
    
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 1;
    }

    public override object GetAnswer1()
    {
        foreach (var list in _calories)
        {
            _totals.Add(list.Sum());
        }
        _totals.Sort();
        _totals.Reverse();
        return _totals[0]+1;
    }

    public override object GetAnswer2() => _totals[0] + _totals[1] + _totals[2];

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            _calories.Add(new List<int>());
            return;
        }

        if (index == 0)
        {
            _calories.Add(new List<int>());
        }
        _calories.Last().Add(int.Parse(line));
    }
}
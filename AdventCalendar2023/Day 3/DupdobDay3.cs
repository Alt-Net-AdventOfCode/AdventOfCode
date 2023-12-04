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

using AoC;

namespace AdventCalendar2023;

public class DupdobDay3 : SolverWithLineParser
{
    private readonly List<string> _lines = new();
    private List<(int number, List<(int y, int x)> stars)> _singleGear = new();

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 3;
        automatonBase.RegisterTestDataAndResult(@"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..", 4361, 1);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        for(var i = 0; i < _lines.Count; i++)
        {
            var line = _lines[i];
            var cursor = 0;
            while(cursor<line.Length)
            {
                // find beginning of numbers
                while (cursor<line.Length && (line[cursor] < '0' || line[cursor] > '9'))
                {
                    cursor++;
                }

                if (cursor >= line.Length)
                {
                    break;
                }
                var begin = cursor;
                while (cursor<line.Length && line[cursor] >= '0' && line[cursor] <= '9')
                {
                    cursor++;
                }

                var foundSymbol = false;
                var stars = new List<(int y, int x)>();
                // check around
                for (var y = Math.Max(0, i - 1); y <= Math.Min(_lines.Count - 1, i + 1); y++)
                {
                    for (var x = Math.Max(0, begin - 1); x <= Math.Min(cursor, line.Length - 1); x++)
                    {
                        var car = _lines[y][x];
                        if (car is '.' or >= '0' and <= '9')
                            continue;
                        foundSymbol = true;
                        if (car == '*')
                        {
                            stars.Add((y,x));
                        }
                    }
                }

                if (foundSymbol)
                {
                    var number = int.Parse(line[begin .. cursor]);
                    result += number;
                    if (stars.Count > 0)
                    {
                        _singleGear.Add((number, stars));
                    }
                }
            }
        }

        return result;
    }
    
    public override object GetAnswer2()
    {
        var result = 0;
        for (var i = 0; i < _singleGear.Count; i++)
        {
            for (var j = i + 1; j < _singleGear.Count(); j++)
            {
                if (_singleGear[j].stars.Any(x => _singleGear[i].stars.Contains(x)))
                {
                    result += _singleGear[i].number * _singleGear[j].number;
                    // we assume no chain of gearts
                    break;
                }
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
        _lines.Add(line);
    }
}
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
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay14 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 14;
        automaton.AddExample(@"498,4 -> 498,6 -> 496,6
503,4 -> 502,4 -> 502,9 -> 494,9
");
        automaton.RegisterTestResult(24);
        automaton.RegisterTestResult(93,2);
    }

    private void DrawCave()
    {
        for (var y = _minDepth; y <= _maxDepth; y++)
        {
            for (var x = _left; x <= _right; x++)
            {
                Console.Write(_rocks.Contains((x,y)) ? "#" : ".");
            }
            Console.WriteLine();
        }
    }

    public override object GetAnswer1()
    {
        var sandUnit = 0;
        DrawCave();
        var rocks = new HashSet<(int x, int y)>(_rocks);
        while (true)
        {
            (int x, int y) start = (500, 0);
            while (true)
            {
                var next = (start.x, start.y + 1);
                if (next.Item2 > _maxDepth)
                {
                    // sand falls through
                    return sandUnit;
                }
                if (rocks.Contains(next))
                {
                    next = (start.x - 1, start.y + 1);
                    if (rocks.Contains(next))
                    {
                        next = (start.x + 1, start.y + 1);
                        if (rocks.Contains(next))
                        {
                            // sands can no longer move
                            rocks.Add(start);
                            sandUnit++;
                            break;
                        }
                    }
                }

                start = next;
            }
        }
    }

    public override object GetAnswer2()
    {
        var sandUnit = 0;
        DrawCave();
        while (true)
        {
            (int x, int y) start = (500, 0);
            while (true)
            {
                var next = (start.x, start.y + 1);
                if (next.Item2 == _maxDepth+2)
                {
                    //sand hit bottom
                    _rocks.Add(start);
                    sandUnit++;
                    break;
                }
                if (_rocks.Contains(next))
                {
                    next = (start.x - 1, start.y + 1);
                    if (_rocks.Contains(next))
                    {
                        next = (start.x + 1, start.y + 1);
                        if (_rocks.Contains(next))
                        {
                            // sands can no longer move
                            _rocks.Add(start);
                            sandUnit++;
                            if (start.y == 0)
                            {
                                return sandUnit;
                            }
                            break;
                        }
                    }
                }

                start = next;
            }
        }
    }

    private readonly HashSet<(int x, int y)> _rocks = new();
    private int _maxDepth = int.MinValue;
    private int _minDepth = int.MaxValue;
    private int _left = int.MaxValue;
    private int _right = int.MinValue;
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        var segments = line.Split("->");

        var coordinatesAsText = segments[0].Split(",");
        (int x, int y) start = (int.Parse(coordinatesAsText[0]), int.Parse(coordinatesAsText[1]));
        _maxDepth = Math.Max(_maxDepth, start.y);
        _minDepth = Math.Min(_minDepth, start.y);
        _left = Math.Min(_left, start.x);
        _right = Math.Max(_right, start.x);
        for (var i = 1; i < segments.Length; i++)
        {
            coordinatesAsText = segments[i].Split(",");
            (int x, int y) end = (int.Parse(coordinatesAsText[0]), int.Parse(coordinatesAsText[1]));

            if (start.x == end.x)
            {
                if (end.y > start.y)
                {
                    for(var y = start.y; y<=end.y; y++)
                    {
                        _rocks.Add((start.x, y));
                    }
                }
                else
                {
                    for(var y = end.y; y<=start.y; y++)
                    {
                        _rocks.Add((start.x, y));
                    }
                }
            }
            else
            {
                if (end.x>start.x)
                {
                    for (var x = start.x; x <= end.x; x++)
                    {
                        _rocks.Add((x, start.y));
                    }
                }
                else
                {
                    for (var x = end.x; x <= start.x; x++)
                    {
                        _rocks.Add((x, start.y));
                    }
                }
            }

            _maxDepth = Math.Max(_maxDepth, end.y);
            _minDepth = Math.Min(_minDepth, end.y);
            _left = Math.Min(_left, end.x);
            _right = Math.Max(_right, end.x);
            start = end;
        }
    }
}
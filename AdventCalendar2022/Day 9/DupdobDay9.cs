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

public class DupdobDay9 : SolverWithLineParser
{
    private readonly List<(char direction, int steps)> _steps = new();
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 9;
        automaton.RegisterTestDataAndResult(@"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2", 13, 1);
        automaton.RegisterTestDataAndResult(@"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20", 36, 2);
    }

    private static (int x, int y) MoveTail((int x, int y) head, (int x, int y) tail)
    {
        var dx = tail.x - head.x;
        var dy = tail.y - head.y;
        // if both are on the same cell or next to the other, no move
        if (Math.Abs(dx) <=1 && Math.Abs(dy) <= 1)
        {
            return tail;
        }

        // diagonal move
        dx = dx == 0 ? 0:  dx / Math.Abs(dx);
        dy = dy == 0 ? 0: dy / Math.Abs(dy);
        return (tail.x - dx, tail.y - dy);
    }
    
    public override object GetAnswer1() => SolveForGivenLength(2);

    public override object GetAnswer2() => SolveForGivenLength(10);

    private object SolveForGivenLength(int len)
    {
        var cells = new HashSet<(int x, int y)>();
        var knots = new (int x, int y)[len];
        cells.Add(knots[len - 1]);
        foreach (var (direction, steps) in _steps)
        {
            var dx = 0;
            var dy = 0;
            switch (direction)
            {
                case 'U':
                    dy = -1;
                    break;
                case 'D':
                    dy = 1;
                    break;
                case 'L':
                    dx = -1;
                    break;
                case 'R':
                    dx = 1;
                    break;
            }

            for (var i = 0; i < steps; i++)
            {
                knots[0] = (knots[0].x + dx, knots[0].y + dy);
                for (var j = 1; j < len; j++)
                {
                    knots[j] = MoveTail(knots[j - 1], knots[j]);
                }

                cells.Add(knots[len - 1]);
            }
        }

        return cells.Count;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }
        _steps.Add((line[0], int.Parse(line[2..]))); 
    }
}
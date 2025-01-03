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
using AoCAlgorithms;

namespace AdventCalendar2017;

public class DupdobDay03: SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 3;
        automatonBase.RegisterTestDataAndResult("12", 3, 1);
        automatonBase.RegisterTestDataAndResult("23", 2, 1);
    }

    public override object GetAnswer1()
    {
        var target = _target;
        for (var depth = 0; depth < 1_000_000; depth++)
        {
            var perimeter = depth == 0 ? 1 : depth * 8;
            if (perimeter < target)
            {
                target -= perimeter;
                continue;
            }
            // we are at the proper ring
            // we do not care on which side, they are equivalent
            var position = (target+1)%(depth+1);
            return position + depth;
        }

        return null;
    }

    private static readonly (int dx, int dy)[] Vectors = [(0, -1), (-1, 0), (0, 1), (1, 0)];

    public override object GetAnswer2()
    {
        var map = new SparseMap2D<int>(0);
        var depth = 0;
        var direction = 3;
        var len = 1;
        (int x, int y) cell = (0, 0);
        map[cell] = 1;
        for(;;)
        {
            cell = (cell.x + Vectors[direction].dx, cell.y + Vectors[direction].dy);
            len--;  
            if (len == 0)
            {
                // we turn
                direction++;
                if (direction > 3)
                {
                    depth += 2;
                    direction = 0;
                    len = depth - 1;
                }
                else
                {
                    len = direction == 3 ? depth+1 : depth;
                }
            }
            var total = map.Neighbors8Of(cell).Sum(next => map[next]);
            map[cell] = total;
            if (total > _target)
            {
                return total;
            }
        }
    }

    private int _target = 0;
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _target = int.Parse(line);
    }
}
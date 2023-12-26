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

public class DupdobDay11: SolverWithLineParser
{
    private List<(int y, int x)> _stars = new ();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 11;
        automatonBase.RegisterTestDataAndResult(@"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....", 374, 1);
    }

    public override object GetAnswer1()
    {
        return ComputeWithExpansion(2);
    }

    private object ComputeWithExpansion(int increase)
    {
        var minX = _stars.Min(s => s.x);
        var minY = _stars.Min(s => s.y);
        var maxX = _stars.Max(s => s.x);
        var maxY = _stars.Max(s => s.y);

        var lineMap = new Dictionary<int, int>();
        var correctedY = minY;
        for (var y = minY; y <= maxY; y++)
        {
            if (_stars.All(s => s.y != y))
            {
                // emptyLine
                correctedY+=increase;
            }
            else
            {
                correctedY++;
            }

            lineMap[y] = correctedY;
        }

        var columnMap = new Dictionary<int, int>();
        var correctedX = minX;
        for (var x = minX; x <= maxX; x++)
        {
            columnMap[x] = correctedX;
            if (_stars.All(s => s.x != x))
            {
                correctedX+=increase;
            }
            else
            {
                correctedX++;
            }
        }

        var result = 0;
        for (var i = 0; i < _stars.Count; i++)
        {
            (int y, int x) refStar = (lineMap[_stars[i].y], columnMap[_stars[i].x]);
            for (var j = i + 1; j < _stars.Count; j++)
            {
                result += Math.Abs(lineMap[_stars[j].y] - refStar.y) + Math.Abs(columnMap[_stars[j].x] - refStar.x);
            }
        }
        return result;
    }

    public override object GetAnswer2()
    {
        return ComputeWithExpansion(1_000_000);
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '#')
            {
                _stars.Add((index, i));
            }
        }
    }
}
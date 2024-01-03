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

namespace AdventCalendar2023;

public class DupdobDay16 : SolverWithLineParser
{
    private readonly List<string> _map = new();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 16;
        automatonBase.RegisterTestDataAndResult(@".|...\....
|.-.\.....
.....|-...
........|.
..........
.........\
..../.\\..
.-.-/..|..
.|....-|.\
..//.|....", 46, 1);
    }

    private readonly (int dy, int dx)[] _vectors = { (0, 1), (1, 0), (0, -1), (-1, 0) };
    
    public override object GetAnswer1()
    {
        var width = _map[0].Length;
        var height = _map.Count;
        var dir = 0;
        var initialPosition = (0,0);
        return Energize(dir, initialPosition, height, width);
    }

    private int Energize(int dir, (int, int) initialPosition, int height, int width)
    {
        var energized= new int[height, width];
        LightTravel(initialPosition, energized, dir);

        var score = 0;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (energized[y, x]>0)
                {
                    score++;
                }
            }
        }

        return score;
    }

    private void LightTravel((int y, int x) pos, int[,] energized, int dir)
    {
        var width = energized.GetLength(1);
        var height = energized.GetLength(0);
        while (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
        {
            if (energized[pos.y, pos.x] == dir + 1)
            {
                // we have a loop
                return;
            }
            energized[pos.y, pos.x] = dir+1;
            switch (_map[pos.y][pos.x])
            {
                case '.':
                    break;
                case '/':
                    dir = new []{3,2,1,0}[dir];
                    break;
                case '\\':
                    dir = new []{1,0,3,2}[dir];
                    break;
                case '|':
                    if ((dir & 1) == 0)
                    {
                        dir = 1;
                        LightTravel(pos, energized, 3);
                    }
                    break;
                case '-':
                    if ((dir & 1) == 1)
                    {
                        dir = 0;
                        LightTravel(pos, energized, 2);
                    }
                    break;
            }
            pos = (pos.y + _vectors[dir].dy, pos.x + _vectors[dir].dx);
        }
    }

    public override object GetAnswer2()
    {
        var width = _map[0].Length;
        var height = _map.Count;
        var score = 0;
        for (var y = 0; y < height; y++)
        {
            score = Math.Max(score, Energize(0, (y, 0), height, width));
            score = Math.Max(score, Energize(2, (y, width-1), height, width));
        }
        for (var x = 0; x < width; x++)
        {
            score = Math.Max(score, Energize(1, (0, x), height, width));
            score = Math.Max(score, Energize(3, (height-1, x), height, width));
        }
        return score;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line);
    }
}
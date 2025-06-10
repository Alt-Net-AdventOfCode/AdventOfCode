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

public class DupdobDay10 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 10;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                ...0...
                                                ...1...
                                                ...2...
                                                6543456
                                                7.....7
                                                8.....8
                                                9.....9
                                                """, 2, 1);
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                ..90..9
                                                ...1.98
                                                ...2..7
                                                6543456
                                                765.987
                                                876....
                                                987....
                                                """, 4, 1);
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                89010123
                                                78121874
                                                87430965
                                                96549874
                                                45678903
                                                32019012
                                                01329801
                                                10456732
                                                """, 36, 1);
        dayAutomatonBase.RegisterTestResult(81, 2);
    }

    public override object GetAnswer1()
    {
        var score = 0;
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if (_map[y][x] == 0)
                {
                    score += ComputeScore(y, x);
                }
            }
        }
        return score;
    }

    private int ComputeScore(int y, int x, bool skipVisited = true)
    {
        var countOfPeaks = 0;
        var current = (y, x);
        var pending = new Queue<(int y, int x)>();
        pending.Enqueue(current);
        var visited = new HashSet<(int y, int x)>();
        while (pending.TryDequeue(out current))
        {
            if (skipVisited && !visited.Add(current))
            {
                continue;
            }
            (y,x) = (current.y, current.x);
            var altitude = _map[y][x];
            if (altitude == 9)
            {
                countOfPeaks++;
            }
            else
            {
                // scan neighbors
                if (x > 0 && _map[y][x - 1] == altitude + 1)
                {
                    pending.Enqueue((y , x - 1));
                }
                if (y > 0 && _map[y-1][x] == altitude + 1)
                {
                    pending.Enqueue((y-1 , x));
                }
                if (x < _width-1 && _map[y][x+1] == altitude + 1)
                {
                    pending.Enqueue((y , x+1));
                }

                if (y < _height - 1 && _map[y + 1][x] == altitude + 1)
                {
                    pending.Enqueue((y+1 , x));
                }
            }
        }
        return countOfPeaks;
    }

    public override object GetAnswer2()
    {
        var score = 0;
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if (_map[y][x] == 0)
                {
                    score += ComputeScore(y, x, false);
                }
            }
        }
        return score;
    }

    private readonly List<int[]> _map = [];
    private int _width;
    private int _height;
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line.Select(c => c-'0').ToArray());
        if (index == 0)
        {
            _height = lineCount;
            _width = line.Length;
        }
    }
}
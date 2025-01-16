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
using AoCAlgorithms;

namespace AdventCalendar2022;

public class DupdobDay24 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 24;
        automaton.AddExample(@"#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#");
        automaton.RegisterTestResult(18);
        automaton.RegisterTestResult(54,2);
    }

    private readonly (int dx, int dy)[] _vectors = {(1, 0), (0, 1), (-1, 0), (0, -1), (0, 0) };
    private readonly int[] _strategy = { 1,2,0,3,4};

    public override object GetAnswer1()
    {
        _cycle = (int)MathHelper.Lcm(_width, _height);
        InitBlizzardsState();
        return ShortestPathFromTo(_start, _exit, 0);
    }
    
    public override object GetAnswer2()
    {
        var firstTrip = ShortestPathFromTo(_start, _exit, 0);
        var secondtrip = ShortestPathFromTo(_exit, _start, firstTrip);
        return ShortestPathFromTo(_start, _exit, secondtrip);
    }

    private int ShortestPathFromTo((int x, int y) pos, (int x, int y) exit, int round)
    {
        var start = pos;
        var pending = new PriorityQueue<((int x, int y), int round), int>();
        pending.Enqueue((pos, round), Priority(pos, round));
        var distances = new Dictionary<((int x, int y), int phase), int>
        {
            [(pos, round % _cycle)] = 0
        };

        while (true)
        {
            (pos, round) = pending.Dequeue();
            if (pos == exit)
            {
                break;
            }

            round++;
            var phase = round % _cycle;
            foreach (var moveIndex in _strategy)
            {
                (int x, int y) cell = (pos.x + _vectors[moveIndex].dx, pos.y + _vectors[moveIndex].dy);
                if ((cell.x < 0 || cell.y < 0 || cell.x >= _width || cell.y >= _height) && (cell != start && cell != exit))
                {
                    continue;
                }

                if (!_emptyCells[phase].Contains(cell))
                {
                    continue;
                }

                if (distances.TryGetValue((cell, phase), out var value) && value <= round)
                {
                    // we know a shorter path to this state
                    continue;
                }

                distances[(cell, phase)] = round;
                pending.Enqueue((cell, round), Priority((cell), round));
            }
        }

        return round;
    }

    private void InitBlizzardsState()
    {
        var blizzardsPositions = _blizzards.Select(entry => (entry.x, entry.y)).ToList();
        var blizzards = blizzardsPositions.ToHashSet();
        _emptyCells.Add(ExtractEmptyCells(blizzards));
        for (var i = 1; i < _cycle; i++)
        {
            for (var j = 0; j < _blizzards.Count; j++)
            {
                var direction = _blizzards[j].direction;
                blizzardsPositions[j] = ((blizzardsPositions[j].x + _vectors[direction].dx + _width) % _width,
                    ((blizzardsPositions[j].y + _vectors[direction].dy + _height) % _height));
            }

            var phaseBlizzards = blizzardsPositions.ToHashSet();
            _emptyCells.Add(ExtractEmptyCells(phaseBlizzards));
        }
    }

    private HashSet<(int x, int y)> ExtractEmptyCells(IReadOnlySet<(int x, int y)> blizzards)
    {
        var emptyCells = new HashSet<(int x, int y)>
        {
            _start,
            _exit
        };
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if (blizzards.Contains((x, y)))
                {
                    continue;
                }

                emptyCells.Add((x, y));
            }
        }

        return emptyCells;
    }

    private static int ManhattanDistance((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    
    private int Priority((int x, int y) coord, int round) => round + ManhattanDistance(coord, _exit);

    private readonly List<(int x, int y, int direction)> _blizzards = new();
    private readonly List<HashSet<(int x, int y)>> _emptyCells = new();
    private int _width;
    private int _height;
    private (int x, int y) _start;
    private (int x, int y) _exit;
    private int _cycle;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        if (_width == 0)
        {
            _width = line.Length - 2;
        }

        index--;
        if (index == -1)
        {
            _start = (line.IndexOf('.') - 1, index);
        }
        else if (index == lineCount - 2)
        {
            _exit = (line.IndexOf('.') - 1, index);
            _height = lineCount-2;
        }
        else
        {
            for (var i = 0; i < line.Length-2; i++)
            {
                switch (line[i+1])    
                {
                    case '^':
                        _blizzards.Add((i, index, 3));
                        break;
                    case '>':
                        _blizzards.Add((i, index, 0));
                        break;
                    case 'v':
                        _blizzards.Add((i, index, 1));
                        break;
                    case '<':
                        _blizzards.Add((i, index, 2));
                        break;
                }
            }
        }
    }
}
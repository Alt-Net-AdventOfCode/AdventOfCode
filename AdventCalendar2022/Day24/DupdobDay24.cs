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
        automaton.RegisterTestData(@"#.######
#>>.<^<#
#.<..<<#
#>v.><>#
#<^v^^>#
######.#");
        automaton.RegisterTestResult(18);
    }

    private readonly (int dx, int dy)[] _vectors = {(1, 0), (0, 1), (-1, 0), (0, -1), (0, 0) };
    private readonly int[] _strategy = { 1,2,0,3,4};
    
    public override object GetAnswer1()
    {
        InitBlizzardsState();
        var cycle = (int)MathHelper.Lcm(_width, _height);
        var pending = new PriorityQueue<((int x, int y), int round), int>();
        var pos = _start;
        var round = 0;
        var phase = 0;
        pending.Enqueue((pos, 0), Priority(pos, round));
        var distances = new Dictionary<((int x, int y), int phase), int>
        {
            [(pos, phase)] = 0
        };
        
        while (true)
        {
            (pos, round) = pending.Dequeue();
            if (pos == _exit)
            {
                break;
            }

            round++;
            phase = round % cycle;
            foreach (var moveIndex in _strategy)
            {
                (int x, int y) cell = (pos.x + _vectors[moveIndex].dx, pos.y + _vectors[moveIndex].dy);
                if ((cell.x < 0  || cell.y < 0 || cell.x >= _width || cell.y >= _height) && (cell!= _start && cell != _exit))
                {
                    continue;
                }
                if (_blizzardsState[phase].Contains(cell))
                {
                    continue;
                }
                if (distances.TryGetValue((cell, phase), out var value) && value < round)
                {
                    // we know a shorter path to this state
                    continue;
                }

                distances[(cell, phase)] = round;
                if (!_blizzardsState[phase].Contains(cell))
                {
                    pending.Enqueue((cell, round), Priority((cell), round));
                }
            }
        }

        return round;
    }

    private void InitBlizzardsState()
    {
        var cycle = (int)MathHelper.Lcm(_width, _height);
        var blizzardsPositions = _blizzards.Select(entry => (entry.x, entry.y)).ToList();
        _blizzardsState.Add(blizzardsPositions.ToHashSet());
        for (var i = 1; i < cycle; i++)
        {
            for (var j = 0; j < _blizzards.Count; j++)
            {
                var direction = _blizzards[j].direction;
                blizzardsPositions[j] = ((blizzardsPositions[j].x + _vectors[direction].dx + _width) % _width,
                    ((blizzardsPositions[j].y + _vectors[direction].dy + _height) % _height));
            }
            _blizzardsState.Add(blizzardsPositions.ToHashSet());
        }
    }

    private static int ManhattanDistance((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    
    private int Priority((int x, int y) coord, int round) => round + ManhattanDistance(coord, _exit);

    private List<(int x, int y, int direction)> MoveBlizzards(IEnumerable<(int x, int y, int direction)> blizzards) 
        => blizzards.Select(t => ((t.x + _vectors[t.direction].dx +_width) % _width, (t.y+_vectors[t.direction].dy+_height)%_height, t.direction)).ToList();

    public override object GetAnswer2()
    {
        return null;
    }

    private readonly List<(int x, int y, int direction)> _blizzards = new();
    private readonly List<HashSet<(int x, int y)>> _blizzardsState = new();
    private int _width;
    private int _height;
    private (int x, int y) _start;
    private (int x, int y) _exit;
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
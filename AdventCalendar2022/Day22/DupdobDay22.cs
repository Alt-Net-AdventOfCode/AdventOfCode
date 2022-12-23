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

using System.Runtime.Intrinsics.X86;
using System.Security;
using System.Security.AccessControl;
using AoC;
using AoCAlgorithms;

namespace AdventCalendar2022;

public class DupdobDay22 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 22;
        automaton.RegisterTestData(@"        ...#
        .#..
        #...
        ....
...#.......#
........#...
..#....#....
..........#.
        ...#....
        .....#..
        .#......
        ......#.

10R5L5R10L4R5L5");
        automaton.RegisterTestResult(6032);
        automaton.RegisterTestResult(5031,2);
    }

    public override object GetAnswer1()
    {
        _height = _map.Max(c => c.y);
        for (var line = 0; line <= _height; line++)
        {
            var cells = _map.Where(c => c.y == line).ToList();
            _xBorders.Add((cells.Min(c => c.x), cells.Max(c => c.x)));
        }

        _width = _map.Max(c => c.x);
        for (var col = 0; col <= _width; col++)
        {
            var cells = _map.Where(c => c.x == col).ToList();
            _yBorders.Add((cells.Min(c => c.y), cells.Max(c => c.y)));
        }
        _sideSize = (int) Math.Sqrt(_map.Count / 6);

        var y = 0;
        var x = _xBorders[y].left;
        var dir = 0;
        // lets move
        foreach (var (steps, direction) in _path)
        {
            if (dir % 2 == 0)
            {
                // horizontal move
                var step = dir == 0 ? 1 : -1;
                for (var i = 0; i < steps; i++)
                {
                    var newX = x + step;
                    if (!_map.Contains((newX, y)))
                    {
                        // wraparound
                        newX = (dir == 0) ? _xBorders[y].left : _xBorders[y].right;
                    }

                    if (_walls.Contains((newX, y)))
                    {
                        // wall
                        break;
                    }

                    x = newX;
                }
            }
            else
            {
                var step = dir == 1 ? 1 : -1;
                for (var i = 0; i < steps; i++)
                {
                    var newY = y + step;
                    if (!_map.Contains((x, newY)))
                    {
                        // wraparound
                        newY = (dir == 1) ? _yBorders[x].top : _yBorders[x].bottom;
                    }

                    if (_walls.Contains((x, newY)))
                    {
                        // wall
                        break;
                    }

                    y = newY;
                }
            }

            dir += direction;
            if (dir == -1)
            {
                dir = 3;
            }
            else if (dir == 4)
            {
                dir = 0;
            }
        }

        return (y+1) * 1000 + (x+1) * 4 + dir;
    }

    private readonly List<(int left, int right)> _xBorders = new ();
    private readonly List<(int top, int bottom)> _yBorders = new ();

    public override object GetAnswer2()
    {
        (int x, int y) pos = (_xBorders[0].left,0);
        var dir = 0;
        var originalDirection = 0;
        (int dx, int dy)[] vectors = {(1,0),(0,1),(-1,0),(0,-1) };
        var path = new Dictionary<(int x, int y), char> { { (pos.x, pos.y), ">v<^"[dir] } };
        // dump the unfolded cube
        for(var y=0; y<4; y++)
        {
            for (var x = 0; x < 4; x++)
            {
                if (_map.Contains((x * _sideSize, y * _sideSize)))
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        // lets move
        foreach (var (steps, direction) in _path)
        {
            for (var i = 0; i < steps; i++)
            {
                var vector = vectors[dir];
                (int x, int y) next = (pos.x + vector.dx, pos.y + vector.dy);
                (int X, int Y) block = (next.x / _sideSize - (next.x<0 ? 1 : 0), next.y / _sideSize - (next.y<0 ? 1 : 0));
                (int dx, int dy) offset = (next.x % _sideSize, next.y % _sideSize);
                if (!_map.Contains(next))
                {
                    var nextBlock = block;
                    var oldDir = dir;
                    // non trivial wrap around, we change face
                    switch (dir)
                    {
                        case 0:
                        {
                            // try line below
                            if (TryGetBlock(block, (0, 1), ref nextBlock))
                            { 
                                // below right
                                next = BlockAndOffsetToPos(nextBlock, (-offset.dy-1, 0));
                                dir++;
                            }
                            // try two lines below
                            else if (TryGetBlock(block, (0, 2), ref nextBlock) || TryGetBlock(block, (-2, 2), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (-1, -offset.dy-1));
                                dir+=2;
                            }
                            // try three lines below
                            else if (TryGetBlock(block, (0, 3), ref nextBlock))
                            {   
                                next = BlockAndOffsetToPos(nextBlock, (offset.dy, -1));
                                dir += 3;
                            }
                            else
                            {
                                Console.WriteLine("failed to move right");
                            }
                            break;
                        }
                        case 1:
                        {
                            if (TryGetBlock(block, (3, 0), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (-1, offset.dx));
                                dir++;
                            }
                            else if (TryGetBlock(block, (1, 0), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (0, offset.dx));
                                dir--;
                            }
                            else if (TryGetBlock(block, (2, -2), ref nextBlock) )
                            {
                                next = BlockAndOffsetToPos(nextBlock, (-offset.dx-1, -1));
                                dir+=2;
                            }
                            else
                            {
                                Console.WriteLine("failed to move down");
                            }
                            break;
                        }
                        case 2:
                        {
                            // try line below
                            if (TryGetBlock(block, (0, 1), ref nextBlock))
                            { 
                                // below right
                                next = BlockAndOffsetToPos(nextBlock, (offset.dy, 0));
                                dir--;
                            }
                            else
                                // try two lines below
                            if (TryGetBlock(block, (0, 2), ref nextBlock) || TryGetBlock(block, (-2, -2), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (0, -offset.dy-1));
                                dir-=2;
                            }
                            // try three lines below
                            else if (TryGetBlock(block, (0, 3), ref nextBlock))
                            {   
                                next = BlockAndOffsetToPos(nextBlock, (-offset.dy-1, -1));
                                dir ++;
                            }
                            else
                            {
                                Console.WriteLine($"failed to move left {nextBlock}.");
                            }
                            break;
                        }
                        case 3:
                        {
                            if (TryGetBlock(block, (1, 0), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (0, offset.dx));
                                dir++;
                            }
                            else if (TryGetBlock(block, (2, 0), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (-offset.dx-1,0));
                                dir-=2;
                            }
                            else if (TryGetBlock(block, (3, 0), ref nextBlock))
                            {
                                next = BlockAndOffsetToPos(nextBlock, (0, -offset.dx-1));
                                dir--;
                            }
                            else
                            {
                                Console.WriteLine("failed to move up");
                            }
                            break;
                        }
                    }
                    
                    Console.WriteLine($"Move from {block}({oldDir}) to {nextBlock}({dir}), {pos}==>{next}.");
                } 
                
                if (_walls.Contains(next))
                {
                    // wall
                    break;
                }

                pos = next;
                if (dir < 0)
                {
                    dir += 4;
                }
                else if (dir >3)
                {
                    dir -= 4;
                }
                path[(pos.x, pos.y)]= ">v<^"[dir];
            }

            originalDirection = (originalDirection+dir) % 4;
            dir += direction;
            if (dir < 0)
            {
                dir += 4;
            }
            else if (dir >3)
            {
                dir -= 4;
            }
            path[(pos.x, pos.y)]= ">v<^"[dir];
        }

        for (var y = 0; y <= _height; y++)
        {
            for (var x = 0; x <= _width; x++)
            {
                if (path.ContainsKey((x, y)))
                {
                    Console.Write(path[(x,y)]);
                }
                else if (_walls.Contains((x, y)))
                {
                    Console.Write('#');
                }
                else if (_map.Contains((x, y)))
                {
                    Console.Write('.');
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
        
        return (pos.y+1) * 1000 + (pos.x+1) * 4 + originalDirection;
    }

    private (int x, int y) BlockAndOffsetToPos((int x, int y) block, (int dx, int dy) offset)
    {
        return (block.x * _sideSize + offset.dx + (offset.dx<0 ? _sideSize : 0), block.y * _sideSize + offset.dy+ (offset.dy<0 ? _sideSize : 0));
    }

    private bool TryGetBlock((int X, int Y) block, (int dx, int dy) offset, ref (int X, int Y) resultBlock)
    {
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <=1; j++)
            {
                var tryBlock = (block.X + i * 4 + offset.dx, block.Y + j*4 + offset.dy);
                if (_map.Contains((tryBlock.Item1 * _sideSize, tryBlock.Item2 * _sideSize)))
                {
                    resultBlock = tryBlock;
                    return true;
                }
            }
        }
        return false;
    }

    private readonly HashSet<(int x, int y)> _map = new();
    private readonly HashSet<(int x, int y)> _walls = new();
    private readonly List<(int steps, int direction)> _path = new();
    private int _sideSize;
    private int _height;
    private int _width;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        if (line.Contains('.'))
        {
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x]==' ')
                    continue;
                _map.Add((x, index));
                if (line[x] == '#')
                {
                    _walls.Add((x, index));
                }
            }
        }
        else
        {
            var steps = 0;
            foreach (var car in line)
            {
                if (car is >= '0' and <= '9')
                {
                    steps = steps * 10 + car - '0';
                }
                else
                {
                    _path.Add((steps, car=='L' ? -1 : +1));
                    steps = 0;
                }
            }
            _path.Add((steps, 0));
        }
    }
}
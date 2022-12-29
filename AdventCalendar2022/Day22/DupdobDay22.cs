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
        _height = _map.Max(c => c.y)+1;
        _width = _map.Max(c => c.x)+1;
        // ReSharper disable once PossibleLossOfFraction
        _sideSize = (int) Math.Sqrt(_map.Count / 6);

        var (dir, pos) = TraversePath(BuildFlatSideDescription(LocateSides()));
        // score
        return (pos.y+1) * 1000 + (pos.x+1) * 4 + dir;
    }
    
    private class Side
    {
        public int Id { get; }
        public (int x, int y) TopLeft { get; }
        public (int side, int edge)[] Connections { get; } = new (int side, int edge)[4];

        public Side(int id, (int x, int y) topLeft, (int x, int y) bottomRight)
        {
            Id = id;
            TopLeft = topLeft;
            BottomRight = bottomRight;
        }

        private (int x, int y) BottomRight { get; }

        public bool IsWithinSide((int x, int y) coordinates) => (coordinates.x>= TopLeft.x && coordinates.y>= TopLeft.y) && coordinates.x<=BottomRight.x && coordinates.y<=BottomRight.y;
    }
    
    public override object GetAnswer2()
    {
        var (dir, pos) = TraversePath(BuildCubeSideDescription(LocateSides()));
        // score
        return (pos.y+1) * 1000 + (pos.x+1) * 4 + dir;
    }

    private (int dir, (int x, int y) pos) TraversePath(IReadOnlyDictionary<int, Side> sideDescription)
    {
        var upperBound = _sideSize - 1;
        // starting point
        var dir = 0;
        var side = 1;
        var pos = (sideDescription[side].TopLeft.x, sideDescription[side].TopLeft.y);

        // lets move
        foreach (var (steps, direction) in _path)
        {
            for (var i = 0; i < steps; i++)
            {
                var vector = _vectors[dir];
                var nextSide = side;
                var newDir = dir;
                (int x, int y) next = (pos.x + vector.dx, pos.y + vector.dy);
                if (!sideDescription[side].IsWithinSide(next))
                {
                    // we have to change sides
                    // get which side and with move to and on which edge we land
                    (nextSide, var newEdge) = sideDescription[side].Connections[dir];
                    // moving direction is opposite to edge
                    newDir = newEdge ^ 2;
                    // get the current coordinate (within the side) of interest
                    var offsetToKeep = dir switch
                    {
                        0 => upperBound - (next.y - sideDescription[side].TopLeft.y),
                        1 => (next.x - sideDescription[side].TopLeft.x),
                        2 => next.y - sideDescription[side].TopLeft.y,
                        3 => upperBound - (next.x - sideDescription[side].TopLeft.x),
                        _ => throw new ArgumentException()
                    };
                    // perform coordinates translation
                    (int dx, int dy) offset = newEdge switch
                    {
                        0 => (upperBound, offsetToKeep),
                        1 => (upperBound - offsetToKeep, upperBound),
                        2 => (0, upperBound - offsetToKeep),
                        3 => (offsetToKeep, 0),
                        _ => throw new ArgumentException()
                    };
                    // establish new absolute coordinates
                    next = (sideDescription[nextSide].TopLeft.x + offset.dx,
                        sideDescription[nextSide].TopLeft.y + offset.dy);
                }

                // if we hit a wall there,  we stop (and we do not change side)
                if (_walls.Contains(next))
                {
                    // wall
                    break;
                }

                (pos, side, dir) = (next, nextSide, newDir);
            }

            dir += direction;
            dir = (dir + 4) % 4;
        }

        return (dir, pos);
    }

    private Dictionary<int, Side> BuildFlatSideDescription(Dictionary<int, Side> sideDescription)
    {
        foreach (var (_, description) in sideDescription)
        {
            // look if there is a neighbor on right
            var x = description.TopLeft.x;
            var y = description.TopLeft.y;
            x = (description.TopLeft.x + _sideSize) % _width;
            while(true)
            {
                var side = FindSide(sideDescription, (x, y));
                if (side>0)
                {
                    description.Connections[0] = (side, 2);
                    break;
                }

                x = (x + _sideSize) % _width;
            } 

            x = (description.TopLeft.x - _sideSize+_width) % _width;
            while(true)
            {
                var side = FindSide(sideDescription, (x, y));
                if (side>0)
                {
                    description.Connections[2] = (side, 0);
                    break;
                }
                x = (x - _sideSize+_width) % _width;
            }

            x = description.TopLeft.x;
            y = (description.TopLeft.y + _sideSize) % _height;
            while(true)
            {
                var side = FindSide(sideDescription, (x, y));
                if (side>0)
                {
                    description.Connections[1] = (side, 3);
                    break;
                }

                y = (y + _sideSize) % _height;
            }

            y = (description.TopLeft.y - _sideSize+_width) % _height;
            while(true)
            {
                var side = FindSide(sideDescription, (x, y));
                if (side>0)
                {
                    description.Connections[3] = (side, 1);
                    break;
                }
                y = (y - _sideSize+_height) % _height;
            }
        }

        return sideDescription;
    }

    private static int FindSide(Dictionary<int, Side> sideDescription, (int x, int y) coordinates) => (from value in sideDescription.Values where value.IsWithinSide(coordinates) select value.Id).FirstOrDefault();

    private Dictionary<int, Side> BuildCubeSideDescription(Dictionary<int, Side> sideDescription)
    {
        // identify side connections
        var connectionMade = true;
        while (connectionMade)
        {
            connectionMade = false;
            foreach (var (_, description) in sideDescription)
            {
                for (var i = 0; i < description.Connections.Length; i++)
                {
                    if (description.Connections[i].side>0) continue;
                    var topLeft = description.TopLeft;
                    var next = sideDescription.Values.FirstOrDefault(d =>
                        d.TopLeft.y == topLeft.y + _vectors[i].dy * _sideSize &&
                        d.TopLeft.x == topLeft.x + _vectors[i].dx * _sideSize);
                    // we have a neighbor side
                    if (next != null)
                    {
                        description.Connections[i] = (next.Id, i ^ 2);
                        connectionMade = true;
                    }
                    else if (ScanNeighbor(description, sideDescription, i, 1) || ScanNeighbor(description, sideDescription, i, 3))
                    {
                        connectionMade = true;
                    }
                }
            }
        }
        return sideDescription;
    }

    private Dictionary<int, Side> LocateSides()
    {
        // identify sides and their relationships
        Dictionary<int, Side> sideDescription = new();
        var sideId = 1;
        // identify sides position within the map
        for (var y = 0; y < _height; y += _sideSize)
        {
            for (var x = 0; x < _width; x += _sideSize)
            {
                if (!_map.Contains((x, y)))
                {
                    continue;
                }

                sideDescription[sideId] = new Side(sideId, (x, y), (x + _sideSize - 1, y + _sideSize - 1));
                sideId++;
            }
        }

        return sideDescription;
    }

    private static bool ScanNeighbor(Side description, IReadOnlyDictionary<int, Side> sideDescription, int i, int offset)
    {
        var (nextSide, angle) = description.Connections[(i + offset) % 4];
        if (nextSide <= 0)
        {
            return false;
        }
        var sideOffset = angle - (i + offset) % 4^2;
        var (foundSide, nextAngle) = sideDescription[nextSide].Connections[(i + sideOffset + 4) % 4];
        if (foundSide == 0)
        {
            return false;
        }
        var connectedSide = (nextAngle + offset) % 4;
        description.Connections[i] = (foundSide, connectedSide);
        sideDescription[foundSide].Connections[connectedSide] = (description.Id, i);
        return true;

    }

    private readonly HashSet<(int x, int y)> _map = new();
    private readonly HashSet<(int x, int y)> _walls = new();
    private readonly List<(int steps, int direction)> _path = new();
    private int _sideSize;
    private int _height;
    private int _width;
    private readonly (int dx, int dy)[] _vectors;

    public DupdobDay22()
    {
        _vectors = new[] {(1,0),(0,1),(-1,0),(0,-1) };
    }

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
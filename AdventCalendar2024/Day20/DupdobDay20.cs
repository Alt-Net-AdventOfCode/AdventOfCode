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

public class DupdobDay20: SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 20;
        automatonBase.RegisterTestDataAndResult("""
                                                ###############
                                                #...#...#.....#
                                                #.#.#.#.#.###.#
                                                #S#...#.#.#...#
                                                #######.#.#.###
                                                #######.#.#...#
                                                #######.#.###.#
                                                ###..E#...#...#
                                                ###.#######.###
                                                #...###...#...#
                                                #.#####.#.###.#
                                                #.#...#.#.#...#
                                                #.#.#.#.#.#.###
                                                #...#...#...###
                                                ###############
                                                """, 0, 1);
    }

    public override object GetAnswer1()
    {
        _distancesFromStart = DistancesFrom(_start);
        _distancesToEnd = DistancesFrom(_end);
        var referenceDistance = _distancesFromStart[_end];
        // we scan all accessible point from the start
        var width = _map[0].Length;
        var height = _map.Count;
        var threshold = 100;
        var shortcutLengths = new Dictionary<int, int>();
        foreach (var (position, distanceFromStart) in _distancesFromStart)
        {
            foreach (var (dy, dx) in _vectors)
            {
                var neighbor = new Position(position.Y + dy, position.X + dx);
                // do we cross a wall
                if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.Y >= height || neighbor.X >= width
                    || _map[neighbor.Y][neighbor.X]!='#')
                {
                    continue;
                }
                neighbor = new Position(neighbor.Y + dy, neighbor.X + dx);
                if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.Y >= height || neighbor.X >= width)
                {
                    continue;
                }

                var shortcutLength = 2;
                // still a wall?
                if (_map[neighbor.Y][neighbor.X] == '#')
                {
                    // try dig one step further
                    neighbor = new Position(neighbor.Y + dy, neighbor.X + dx);
                    if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.Y >= height || neighbor.X >= width
                        || _map[neighbor.Y][neighbor.X]=='#')
                    {
                        continue;
                    }

                    shortcutLength++;
                }
                // un reachable position
                if (!_distancesToEnd.TryGetValue(neighbor, out var distanceToEnd) 
                    || (distanceFromStart + shortcutLength + distanceToEnd > referenceDistance))
                {
                    continue;
                }

                var shortcut = referenceDistance - (distanceFromStart + shortcutLength + distanceToEnd);
                shortcutLengths[shortcut] = shortcutLengths.GetValueOrDefault(shortcut) + 1;
            }
        }

        return shortcutLengths.Sum( p=> p.Key>=threshold ? p.Value : 0);
    }
    
    private readonly (int dy, int dx)[] _vectors = [(0, 1), (1, 0), (0,-1), (-1, 0)];

    private Dictionary<Position, int> DistancesFrom(Position start)
    {
        _width = _map[0].Length;
        _height = _map.Count;
        var distances = new Dictionary<Position, int>(_width * _height);
        var distance = 0;
        distances[start] = distance;
        var pending = new PriorityQueue<Position, int>();
        pending.Enqueue(start, 0);
        while (pending.TryDequeue(out start, out distance))
        {
            distance++;
            foreach (var (dx, dy) in _vectors)
            {
                var neighbor = new Position(start.Y + dy, start.X + dx);
                if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.Y >= _height || neighbor.X >= _width ||
                    _map[neighbor.Y][neighbor.X] == '#' ||
                    (distances.TryGetValue(neighbor, out var nextDistance) && nextDistance<distance))
                {
                    continue;
                }

                distances[neighbor] = distance;
                pending.Enqueue(neighbor, distance);
            }        
        }

        return distances;
    }

    public override object GetAnswer2()
    {
        var referenceDistance = _distancesFromStart[_end];
        var result = 0;
        var shortcutLengths1 = new Dictionary<int, List<(Position, Position)>>();

        // using Manhattan distance
        foreach (var (position, initDistance) in _distancesFromStart)
        {
            foreach (var (endPosition, endDistance) in _distancesToEnd)
            {
                var manhattanDistance = Math.Abs(position.X-endPosition.X)+Math.Abs(position.Y-endPosition.Y);
                var save = referenceDistance - (initDistance + endDistance + manhattanDistance);
                if (manhattanDistance < 20
                    && save >1)
                {
                    if (!shortcutLengths1.TryGetValue(save, out var list))
                    {
                        shortcutLengths1[save]= list = [];
                    }
                    list.Add((position, endPosition));
                    result++;
                }
            }
        }

        // using pathfinding
        // we scan all accessible point from the start
        const int threshold = 100;
        var shortcutLengths = new Dictionary<int, List<(Position, Position)>>();
        const int maxShortcut = 20;
        var shortcuts = new Dictionary<Position, Dictionary<Position, int>>();
        foreach (var position in _distancesFromStart.Keys)
        {
            ScanShortcuts(
                referenceDistance,
                maxShortcut,
                position, shortcuts);
        }

        foreach (var (start, destinations) in shortcuts)
        {
            foreach (var (end, save) in destinations)
            {
                if (!shortcutLengths.TryGetValue(save, out var list))
                {
                    shortcutLengths[save]= list = [];
                }
                list.Add((start, end));
            }
        }
        return shortcutLengths.Sum( p=> p.Key>=threshold ? p.Value.Count : 0);
        
    }
    
    private void ScanShortcuts(int referenceDistance, int maxShortcut, Position start,
        Dictionary<Position, Dictionary<Position, int>> shortcuts)
    {
        Dictionary<Position, int> result = [];  
        Dictionary<Position, int> visitedWalls = [];
        var pendingWalls = new PriorityQueue<Position, int>();
        var current = start;
        var initDistance = _distancesFromStart[current];
        var totalDistance = _distancesFromStart[_end];
        pendingWalls.Enqueue(current, initDistance);
        shortcuts[start]=[];
        while(pendingWalls.TryDequeue(out current, out var distance))
        {
            distance++;
            foreach (var (dy, dx) in _vectors)
            {
                var neighbor = new Position(current.Y + dy, current.X + dx);
                // still in map?
                if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.Y >= _height || neighbor.X >= _width)
                {
                    continue;
                }

                // still a wall?
                if (_map[neighbor.Y][neighbor.X] == '#')
                {
                    if (distance-initDistance<maxShortcut && 
                        (!visitedWalls.TryGetValue(neighbor, out var distanceToWall) ||distanceToWall > distance))
                    {
                        visitedWalls[neighbor] = distance;
                        pendingWalls.Enqueue(neighbor, distance);
                    }
                    continue;
                }

                if (!_distancesToEnd.TryGetValue(neighbor, out var distanceToEnd)
                    || distance + distanceToEnd > referenceDistance)
                {
                    continue;
                }

                if (!result.TryGetValue(neighbor, out var currentDistance) ||
                    currentDistance > distance + distanceToEnd)
                {
                    result[neighbor] = distance + distanceToEnd;
                    var saved = totalDistance - distance - distanceToEnd;
                    if (saved > 1)
                    {
                        shortcuts[start][neighbor] = saved;
                    }
                }
            }
        }
    }

    private record Position(int Y, int X);

    private Position _start = null!;
    private Position _end = null!;
    private readonly List<string> _map = [];
    private Dictionary<Position, int> _distancesFromStart;
    private Dictionary<Position, int> _distancesToEnd;
    private int _width;
    private int _height;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var start = line.IndexOf('S');
        if (start >= 0)
        {
            _start = new Position(index, start);
            line = line.Replace('S', '.');
        }

        start = line.IndexOf('E');
        if (start >= 0)
        {
            _end = new Position(index, start);
            line = line.Replace('E', '.');
        }

        _map.Add(line);
    }
}
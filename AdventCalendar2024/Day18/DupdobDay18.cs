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

public class DupdobDay18 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 18;
        automatonBase.RegisterTestDataAndResult("""
                                                5,4
                                                4,2
                                                4,5
                                                3,0
                                                2,1
                                                6,3
                                                2,4
                                                1,5
                                                0,6
                                                3,3
                                                2,6
                                                5,1
                                                1,2
                                                5,5
                                                2,5
                                                6,5
                                                1,4
                                                0,4
                                                6,4
                                                1,1
                                                6,1
                                                1,0
                                                0,5
                                                1,6
                                                2,0
                                                """, 22, 1);
    }

    private readonly (int dy, int dx)[] _vectors = [(0, 1), (1, 0), (0,-1), (-1, 0)];

    public override object GetAnswer1()
    {
        var isTest = _list.Count < 1000;
        var width = isTest ? 7 : 71;
        var time = isTest ? 12 : 1024;

        var fallen = _list[..time].ToHashSet();
        var path = FinPath(width, width, fallen);
        return path.Count;
    }

    private List<Position> FinPath(int heigh, int width, HashSet<Position> fallen)
    {
        var path = new List<Position>();
        var distances = new Dictionary<Position, int>();
        var pending = new PriorityQueue<Position, int>();
        var previous = new Dictionary<Position, Position>();
        var current = new Position(0, 0);
        var end = new Position(heigh - 1, width - 1);
        distances[current] = 0;
        pending.Enqueue(current, 0);
        while (pending.Count > 0)
        {
            current = pending.Dequeue();
            if (current == end)
            {
                break;
            }
            var distance = distances[current]+1;
            foreach (var vector in _vectors)
            {
                var next = new Position(current.Y + vector.dy, current.X + vector.dx);
                if (next.X < 0 || next.Y < 0 || next.X >= width || next.Y >= heigh || fallen.Contains(next))
                {
                    continue;
                }

                if (!distances.TryGetValue(next, out var neighborDistance) || neighborDistance > distance)
                {
                    distances[next] = distance;
                    pending.Enqueue(next, distance);
                    previous[next] = current;
                }

            }            

        }

        current = end;
        if (!previous.ContainsKey(current))
        {
            return [];
        }
        
        while (current.X != 0 || current.Y != 0)
        {
            path.Add(current);
            current = previous[current];
        }

        return path;
    }

    public override object GetAnswer2()
    {
        var isTest = _list.Count < 1000;
        var width = isTest ? 7 : 71;
        var time = isTest ? 12 : 1024;

        var fallen = _list[..time].ToHashSet();
        var currentPath = FinPath(width, width, fallen);
        for (; time < _list.Count; time++)
        {
            var position = _list[time - 1];
            if (!currentPath.Contains(position)) continue;
            // the path is broken
            currentPath = FinPath(width, width, _list[..time].ToHashSet());
            if (currentPath.Count == 0)
            {
                return $"{position.X},{position.Y}";
            }
        }

        return null;
    }

    private record Position(int Y, int X);

    private readonly List<Position> _list = [];
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var coordinates = line.Split(',');
        _list.Add(new Position(int.Parse(coordinates[1]), int.Parse(coordinates[0])));
    }
}
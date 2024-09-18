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

using System.Collections.Immutable;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay23 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 23;
        automatonBase.RegisterTestDataAndResult(@"#.#####################
#.......#########...###
#######.#########.#.###
###.....#.>.>.###.#.###
###v#####.#v#.###.#.###
###.>...#.#.#.....#...#
###v###.#.#.#########.#
###...#.#.#.......#...#
#####.#.#.#######.#.###
#.....#.#.#.......#...#
#.#####.#.#.#########v#
#.#...#...#...###...>.#
#.#.#v#######v###.###v#
#...#.>.#...>.>.#.###.#
#####v#.#.###v#.#.###.# 
#.....#...#...#.#.#...#
#.#########.###.#.#.###
#...###...#...#...#.###
###.###.#.###v#####v###
#...#...#.#.>.>.#.>.###
#.###.###.#.###.#.#v###
#.....###...###...#...#
#####################.#", 94, 1);
        automatonBase.RegisterTestResult(154, 2);
    }

    public override object GetAnswer1()
    {
        _startX = _map[0].IndexOf('.');
        (int y, int x) start = (0, _startX);
        (int y, int x) end = (_map.Count-1, _map[^1].IndexOf('.'));
        
        var map = BuildGraph(start, end);
        var maxLength = FindMaxPath(start, end, 0, map, ImmutableList<(int, int)>.Empty);

        return maxLength;
    }

    private Dictionary<(int y, int x), List<((int y, int x) next, int dist)>> BuildGraph((int y, int x) start, (int y, int x) end)
    {
        var map = new Dictionary<(int y, int x), List<((int y, int x) next, int dist)>>
        {
            [start] = new()
        };

        (int y, int x) dimensions = (_map.Count, _map[0].Length);
        var nodes = new Stack<((int x, int y) start, List<(int y, int x)> cells)>();
        nodes.Push((start, new List<(int, int)>{(start.y+1, start.x)}));
        while (nodes.Count>0)
        {
            var next = nodes.Pop();
            foreach (var nextCell in next.cells)
            {
                var previous = next.start;
                var curDist = 1;
                var current = nextCell;
                for (;;)
                {
                    var list = new List<(int y, int x)>();
                    // scan neighbours
                    if (current.x > 1 && _map[current.y][current.x - 1] is '.' or '<')
                    {
                        list.Add((current.y, current.x - 1));
                    }
                    if (current.y > 1 && _map[current.y -1][current.x ] is '.' or '^')
                    {
                        list.Add((current.y-1, current.x));
                    }
                    if (current.x < dimensions.x-2 && _map[current.y][current.x+1] is '.' or '>')
                    {
                        list.Add((current.y, current.x+1));
                    }
                    if (current.y < dimensions.y-1 && _map[current.y+1][current.x] is '.' or 'v')
                    {
                        list.Add((current.y+1, current.x));
                    }
                    // remove previous node
                    list.Remove(previous);
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (!map.ContainsKey(list[i])) continue;
                        map[next.start].Add((list[i], curDist+1));
                        // no need to rescan it
                        list.RemoveAt(i--);
                    }

                    if (list.Count == 0)
                    {
                        if (current == end)
                        {
                            // it's the end
                            map[next.start].Add((current, curDist));
                        }
                        break;
                    }

                    if (list.Count == 1)
                    {
                        previous = current; 
                        current = list[0];
                        curDist++;
                    }
                    else
                    {
                        // we have more than one path, we create a new node of interest
                        map[next.start].Add((current, curDist));
                        map[current] = new();
                        list.Add(previous);
                        nodes.Push((current, list));
                        break;
                    }
                }
            }
        }

        return map;
    }

    private int FindMaxPath((int y, int x) current, (int y, int x) end, int distance, Dictionary<(int y, int x),List<((int y, int x) next, int dist)>> map, ImmutableList<(int, int)> visited)
    {
        if (current == end)
        {
            return distance;
        }
        var resultDistance = 0;
        foreach (var nextNode in map[current])
        {
            if (visited.Contains(nextNode.next)) continue;
            resultDistance = Math.Max(resultDistance, FindMaxPath(nextNode.next, end, distance+nextNode.dist, map, visited.Add(nextNode.next)));
        }
        return resultDistance;
    }

    public override object GetAnswer2()
    {
        (int y, int x) start = (0, _startX);
        (int y, int x) end = (_map.Count-1, _map[^1].IndexOf('.'));
        for (var i = 0; i < _map.Count; i++)
        {
            _map[i] = _map[i].Replace('<', '.').Replace('>', '.').Replace('^', '.').Replace('v', '.');
        }
        var map = BuildGraph(start, end);
        var maxLength = FindMaxPath(start, end, 0, map, ImmutableList<(int, int)>.Empty);

        return maxLength;
    }

    private readonly List<string> _map = new();
    private int _startX;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (!string.IsNullOrEmpty(line))
        {
            _map.Add(line);
        }
    }
}
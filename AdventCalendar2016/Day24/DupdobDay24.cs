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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using AoC;
using AoCAlgorithms;

namespace AdventCalendar2016;

public class DupdobDay24 : SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 24;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                ###########
                                                #0.1.....2#
                                                #.#######.#
                                                #4.......3#
                                                ###########
                                                """, 14, 1);
    }

    public override object GetAnswer1()
    {
        _distances = new Dictionary<int, Dictionary<int, int>>();
        foreach (var (id, point) in _points) _distances[id] = Djistkra(id);
        
        return FindShorTestTravel([0], _distances, 0, int.MaxValue);
    }

    private int FindShorTestTravel(List<int> id, Dictionary<int,Dictionary<int,int>> distances, int distance, int minValue, bool returnTo0 = false)
    {
        if (id.Count == _points.Count)
        {
            return returnTo0 ? distance+distances[id[^1]][0] : distance;
        }
        
        foreach (var (key, next) in distances[id[^1]].Where(k => !id.Contains(k.Key)))
        {
            var step = distance + next;
            if (step > minValue)
            {
                continue;
            }
            var nextDistance = FindShorTestTravel(id.Append(key).ToList(), distances, step, minValue, returnTo0);
            minValue = Math.Min(minValue, nextDistance);
        }
        return minValue;
    }

    private Dictionary<int,int> Djistkra(int id)
    {
        var point = _points[id];
        var distances = new Dictionary<(int x, int y), int>
        {
            [(point.X, point.Y)] = 0
        };
        var result = new Dictionary<int, int>
        {
            [id] = 0
        };
        var pending = new PriorityQueue<(int x, int y), int>();
        pending.Enqueue((point.X, point.Y), distances[(point.X, point.Y)]);
        while (pending.TryDequeue(out (int X, int Y) cell, out var distance) && result.Count<_points.Count)
        {
            distance++;
            foreach (var (x,y) in _map.NeighborsOf((cell.X, cell.Y)))
            {
                var c = _map[(x, y)];
                if (c == '#') continue;
                if (distances.TryGetValue((x, y), out var current) && current <= distance)
                {
                    continue;
                }

                distances[(x, y)] = distance;
                pending.Enqueue((x,y), distance);
                if (c is < '0' or > '9') continue;
                id = _points.FirstOrDefault(kv => kv.Value.X == x && kv.Value.Y == y).Key;
                result[id] = distance;
            }
        }

        return result;
    }

    public override object GetAnswer2()
    {
        return FindShorTestTravel([0], _distances, 0, int.MaxValue, true);
    }

    private Map2D<char> _map = null!;

    private record Point(int Y, int X);

    private readonly Dictionary<int, Point> _points = [];
    private Dictionary<int, Dictionary<int, int>> _distances;

    protected override void Parse(string input)
    {
        var blockIndex = 0;
        foreach (var block in input.SplitLineBlocks())
        {
            ParseBlock(block.ToList(), blockIndex++);
        }
    }

    protected void ParseBlock(List<string> block, int blockIndex)
    {
        _map = Map2D.FromBlock(block, OutBoundHandling.DefaultValue, '#');
        for(var y = 0; y < _map.GetUpperBound(1); y++)
        {
            for (var x = 0; x < _map.GetUpperBound(0); x++)
            {
                if (_map[(x, y)] is >= '0' and <= '9')
                {
                    _points.Add(_map[(x,y)]-'0', new Point(y, x));
                }
            }
        }
    }
}
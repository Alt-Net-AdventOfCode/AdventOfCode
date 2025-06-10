// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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
using System.Text.RegularExpressions;
using AoC;
using AoCAlgorithms;

namespace AdventCalendar2016;

public class DupdobDay22 : SolverWithLineParser
{
    private readonly Regex _lineFilter =
        new("\\/dev\\/grid\\/node-x(\\d+)-y(\\d+) *(\\d+)T *(\\d+)T *(\\d+)T *(\\d+)%", RegexOptions.Compiled);

    private readonly Dictionary<(int x, int y), (int size, int used, int avail)> _nodes = new();

    private (int x, int y) _bottomRight;

    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 22;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                Filesystem            Size  Used  Avail  Use%
                                                /dev/grid/node-x0-y0   10T    8T     2T   80%
                                                /dev/grid/node-x0-y1   11T    6T     5T   54%
                                                /dev/grid/node-x0-y2   32T   28T     4T   87%
                                                /dev/grid/node-x1-y0    9T    7T     2T   77%
                                                /dev/grid/node-x1-y1    8T    0T     8T    0%
                                                /dev/grid/node-x1-y2   11T    7T     4T   63%
                                                /dev/grid/node-x2-y0   10T    6T     4T   60%
                                                /dev/grid/node-x2-y1    9T    8T     1T   88%
                                                /dev/grid/node-x2-y2    9T    6T     3T   66%
                                                """, 7, 2);
        dayAutomatonBase.RegisterTestResult(7, 1);
    }

    public override object GetAnswer1()
    {
        var nodePerAvailSize = new Dictionary<int, int>();
        foreach (var node in _nodes.Values.Where(node => !nodePerAvailSize.TryAdd(node.avail, 1)))
        {
            nodePerAvailSize[node.avail]++;
        }
        // aggregate
        foreach (var size in nodePerAvailSize.Keys.Order())
        {
            foreach (var size2 in nodePerAvailSize.Keys.Where(size2 => size2 > size))
            {
                nodePerAvailSize[size] += nodePerAvailSize[size2];
            }
        }
        // check result
       
        var sizes = nodePerAvailSize.Keys.Order().ToList();
        var maxSize = sizes[^1];
        var pairs = 0;
        foreach (var node in _nodes.Values)
        {
            if (node.used > maxSize || node.used == 0 )
            {
                continue;
            }
            var availSize = sizes.First(s => s >= node.used);
            pairs += nodePerAvailSize[availSize];
            if (node.avail >= node.used)
            {
                pairs--;
            }
        }

        return pairs;
    }


    private class State : IEquatable<State>
    {
        private readonly int[,] _grid;
        
        public (int X, int Y) EmptySlot { get; }
        public (int X, int Y) NodeOfInterest { get; private set; }

        private State(Func<int, int, int> extractor, int height, int width)
        {
            _grid = new int[width, height];
            NodeOfInterest = (width-1, 0);
            for(var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var used = extractor(x, y);
                    _grid[x, y] = used;
                    if (used == 0)
                    {
                        EmptySlot = (x, y);
                    }
                }
            }
        }
        
        public State(IDictionary<(int x, int y), (int size, int used, int avail)> nodes, int height, int width) : 
            this((x, y) => nodes[(x, y)].used, height, width)
        {}
        
        public State MoveToEmpty((int X, int Y) node)
        {
            var result = new State((x, y) =>
            {
                if (x == node.X && y == node.Y)
                {
                    return 0;
                }

                if (x == EmptySlot.X && y == EmptySlot.Y)
                {
                    return _grid[node.X, node.Y];
                }

                return _grid[x, y];
            }, _grid.GetLength(1), _grid.GetLength(0));
            if (node.X == NodeOfInterest.X && node.Y == NodeOfInterest.Y)
            {
                result.NodeOfInterest = EmptySlot;
            }
            else
            {
                result.NodeOfInterest = NodeOfInterest;
            }

            return result;
        }

        public bool Equals(State other)
        {
            if (other is null) return false;
            if (other.GetHashCode() != GetHashCode()) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other._grid.GetLength(0) != _grid.GetLength(0) || other._grid.GetLength(1)!=_grid.GetLength(1)) return false;
            return other.NodeOfInterest == NodeOfInterest && other.EmptySlot == EmptySlot;
        }

        public int Score() => ManhattanDistance((0, 0), NodeOfInterest) * 10000 + ManhattanDistance(EmptySlot, NodeOfInterest)*100;

        public override bool Equals(object obj) 
            => obj is not null && (ReferenceEquals(this, obj) 
                                   || obj.GetType() == GetType() && Equals((State)obj));

        public override int GetHashCode() => NodeOfInterest.GetHashCode()*23+EmptySlot.GetHashCode();

        public static bool operator ==(State left, State right) => Equals(left, right);

        public static bool operator !=(State left, State right) => !Equals(left, right);
    }
    

    // It turns out that slot are large and stuffed with data or can be stored in any other slot
    // Therefore, there is no need to store the state of each slot, we just need to track the 
    // empty slot and the node of interest
    public override object GetAnswer2()
    {
       // we store the current state
       var state = new State(_nodes, _bottomRight.y+1, _bottomRight.x+1);
       var pendingStates = new PriorityQueue<State, int>();
       pendingStates.Enqueue(state, state.Score());
       var distances = new Dictionary<State, int>
       {
           [state] = 0
       };
       var minDist = int.MaxValue;
       while (pendingStates.TryDequeue(out state, out _))
       {
           var current = state.EmptySlot;
           var size = _nodes[state.EmptySlot].size;
           var dist = distances[state]+1;
           if (dist > minDist)
           {
               continue;
           }
           if (state.NodeOfInterest is { X: 0, Y: 0 })
           {
               minDist = Math.Min(minDist, dist-1);
           }
           // we try to see if we can transfer a slot
           foreach (var (dx, dy) in _vectors)
           {
               (int X, int Y) neighbor = (current.X + dx, current.Y + dy);
               if (neighbor.Y < 0 || neighbor.X < 0 || neighbor.X >_bottomRight.x || neighbor.Y >_bottomRight.y
                   || _nodes[neighbor].used>size)
               {
                   continue;
               }
               
               var newState = state.MoveToEmpty(neighbor);
               if (distances.TryGetValue(newState, out var currentDistance) && currentDistance <= dist) continue;
               pendingStates.Enqueue(newState, newState.Score());
               distances[newState] = dist;
           }
       }

       return minDist;
    }

    private readonly (int Dx, int Dy)[] _vectors = [(1, 0), (0, 1), (-1, 0), (0, -1)];

    private static int ManhattanDistance((int Y, int X) a, (int Y, int X) b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _lineFilter.Match(line);
        if (!match.Success)
        {
            return;
        }

        var x = match.GetInt(1);
        var y = match.GetInt(2);
        _nodes[(x, y)] = (match.GetInt(3), match.GetInt(4), match.GetInt(5));
        _bottomRight.x = Math.Max(_bottomRight.x, x);
        _bottomRight.y = Math.Max(_bottomRight.y, y);
    }
}
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

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 22;
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
        private readonly int _hash;

        private State(Func<int, int, int> extractor, int height, int width)
        {
            _grid = new int[width, height];
            for(var y = 0; y < height; y++)
            for(var x = 0; x < width; x++)
            {
                var used = extractor(x, y);
                _grid[x, y] = used;
                _hash = _hash * 23 + used;
            }
        }
        
        public State(IDictionary<(int x, int y), (int size, int used, int avail)> nodes, int height, int width) : 
            this((int x, int y) => nodes[(x, y)].used, height, width)
        {}

        public static State FromSize(IDictionary<(int x, int y), (int size, int used, int avail)> nodes, int height, int width)
        {
            return new State((x, y) => nodes[(x, y)].size, height, width);
        }

        public bool Equals(State other)
        {
            if (other is null) return false;
            if (other._hash != _hash) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other._grid.GetLength(0) != _grid.GetLength(0) || other._grid.GetLength(1)!=_grid.GetLength(1)) return false;
            for(var y = 0; y < _grid.GetLength(1); y++)
            for(var x = 0; x < _grid.GetLength(0); x++)
            {
                if (_grid[x, y] != other._grid[x, y]) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((State)obj);
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public static bool operator ==(State left, State right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State left, State right)
        {
            return !Equals(left, right);
        }
    }
    
    
    public override object GetAnswer2()
    {
       // find empty drive
       (int x, int y) empty = (0, 0);
       var nodeOfInterest = (_bottomRight.x, 0);
       for(var y = 0; y < _bottomRight.y; y++)
       for (var x = 0; x < _bottomRight.x; x++)
       {
           if (_nodes[(x, y)].used == 0)
           {
               empty = (x, y);
           }
       }
       // we store the current state
       var sizes = State.FromSize(_nodes, _bottomRight.y, _bottomRight.x);
       var current = new State(_nodes, _bottomRight.y, _bottomRight.x);
       var passedStates = new HashSet<(State state, int x, int y)> { (current, empty.x, empty.y) };
       while (nodeOfInterest != (0,0))
       {
           // we try to move the empty slot
       }
       return 0;
    }

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
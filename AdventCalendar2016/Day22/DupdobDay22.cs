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

    private readonly Regex LineFilter =
        new("\\/dev\\/grid\\/node-x(\\d+)-y(\\d+) *(\\d+)T *(\\d+)T *(\\d+)T *(\\d+)%", RegexOptions.Compiled);

    private Dictionary<(int x, int y), (int used, int avail)> _nodes = new();

    private (int x, int y) _topLeft;
    private (int x, int y) _bottomRight;

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 22;
    }

    public override object GetAnswer1()
    {
        var nodePerAvailSize = new Dictionary<int, int>();
        foreach (var node in _nodes.Values)
        {
            if (!nodePerAvailSize.ContainsKey(node.avail))
            {
                nodePerAvailSize[node.avail] = 1;
            }
            else
            {
                nodePerAvailSize[node.avail]++;
            }
        }
        // aggregate
        foreach (var size in nodePerAvailSize.Keys.Order())
        {
            foreach (var size2 in nodePerAvailSize.Keys)
            {
                if (size2 > size)
                {
                    nodePerAvailSize[size] += nodePerAvailSize[size2];
                }
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

    public override object GetAnswer2()
    {
        throw new System.NotImplementedException();
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = LineFilter.Match(line);
        if (!match.Success)
        {
            return;
        }

        var x = match.GetInt(1);
        var y = match.GetInt(2);
        _nodes[(x, y)] = (match.GetInt(4), match.GetInt(5));
        _topLeft.x = Math.Min(_topLeft.x, x);
        _topLeft.y = Math.Min(_topLeft.y, y);
        _bottomRight.x = Math.Max(_bottomRight.x, x);
        _bottomRight.y = Math.Max(_bottomRight.y, y);
    }
}
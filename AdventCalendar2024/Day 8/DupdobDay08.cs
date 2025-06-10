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
using AoCAlgorithms;

namespace AdventCalendar2024;

public class DupdobDay08 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 8;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                ............
                                                ........0...
                                                .....0......
                                                .......0....
                                                ....0.......
                                                ......A.....
                                                ............
                                                ............
                                                ........A...
                                                .........A..
                                                ............
                                                ............
                                                """, 14, 1);
        dayAutomatonBase.RegisterTestResult(34, 2);
    }

    public override object GetAnswer1()
    {
        HashSet<(int x, int y)> antiNodes = [];

        foreach (var antennas in _antennas.Values)
        {
            for (var i = 0; i < antennas.Count; i++)
            {
                var current = antennas[i];
                for (var j = 0; j < antennas.Count; j++)
                {
                    if (i == j) continue;
                    (int x, int y) antiNode = (2*current.x - antennas[j].x, 2*current.y - antennas[j].y);
                    if (antiNode is { x: >= 0, y: >= 0 } && antiNode.y<_map.Count && antiNode.x < _map[0].Length)
                    {
                        antiNodes.Add(antiNode);
                    }
                }
            }
        }
        return antiNodes.Count;
    }

    public override object GetAnswer2()
    {
        HashSet<(int x, int y)> antiNodes = [];

        foreach (var antennas in _antennas.Values)
        {
            for (var i = 0; i < antennas.Count-1; i++)
            {
                var current = antennas[i];
                for (var j = i+1; j < antennas.Count; j++)
                {
                    var dx=current.x - antennas[j].x;
                    var dy=current.y - antennas[j].y;
                    var steps = MathHelper.Gcd(Math.Abs(dx), Math.Abs(dy));
                    var antiNode = current;
                    while (antiNode is { x: >= 0, y: >= 0 } && antiNode.y<_map.Count && antiNode.x < _map[0].Length)
                    {
                        antiNodes.Add(antiNode);
                        antiNode.x += dx;
                        antiNode.y += dy;
                    }
                    antiNode = (current.x-dx, current.y-dy);
                    while (antiNode is { x: >= 0, y: >= 0 } && antiNode.y<_map.Count && antiNode.x < _map[0].Length)
                    {
                        antiNodes.Add(antiNode);
                        antiNode.x -= dx;
                        antiNode.y -= dy;
                    }
                }
            }
        }
        return antiNodes.Count;
    }

    private readonly List<string> _map = new();
    private readonly Dictionary<char, List<(int x, int y)>> _antennas = new();
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line);
        for (var x = 0; x < line.Length; x++)
        {
            if (line[x] != '.')
            {
                if (!_antennas.TryGetValue(line[x], out var antennas))
                {
                    antennas = [];
                    _antennas[line[x]] = antennas;
                }
                antennas.Add((x, index));
            }
        }
    }
}
// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
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
using AoC;

namespace AdventCalendar2015;

[Day(18)]
public class DupdobDay18 : SolverWithParam<int>
{
    private readonly HashSet<(int x, int y)> _lights = [];
    private int _width,_height;
    
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    protected override void Parse(string data)
    {
        var lines = data.SplitLines();
        _height = lines.Length;
        _width = lines[0].Length;
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    _lights.Add((x, y));
                }
            }
        }
    }

    [Example("""
             .#.#.#
             ...##.
             #....#
             ..#...
             #.#..#
             ####..
             """, 4, 4)]
    protected override object GetAnswer1(int nbSteps = 100) => Loop(nbSteps, _lights.ToHashSet());

    private readonly (int dx, int dy)[] _directions = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];
    private object Loop(int nbSteps, HashSet<(int x, int y)> lights, Action<HashSet<(int x, int y)>> cycleEnd = null)
    {
        for (var i = 0; i < nbSteps; i++)
        {
            cycleEnd?.Invoke(lights);
            var newLights = new HashSet<(int x, int y)>();
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    var count = 0;
                    foreach ((var dx, var dy)  in _directions)
                    {
                        if (lights.Contains((x + dx, y + dy)))
                        {
                            count++;
                        }
                    }
                    if (count==3 || count == 2 && lights.Contains((x, y)))
                    {
                        newLights.Add((x, y));
                    }
                }
            }
            lights = newLights;
        }

        cycleEnd?.Invoke(lights);
        return lights.Count;
    }

    [Example("""
                  .#.#.#
                  ...##.
                  #....#
                  ..#...
                  #.#..#
                  ####..
                  """, 17, 5)]
    protected override object GetAnswer2(int nbSteps = 100)
    {
        return Loop(nbSteps, _lights.ToHashSet(), lights => { lights.Add((0, 0));
            lights.Add((0, _height - 1));
            lights.Add((_width - 1, 0));
            lights.Add((_width - 1, _height - 1));
        });
    }
}
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
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2015;

[Day(6)]
public class DupdobDay06 : SolverWithParser
{

    [Example("""
             turn on 0,0 through 999,999
             toggle 0,0 through 999,0
             turn off 499,499 through 500,500
             """, 998996)]
    public override object GetAnswer1()
    {
        var lights = new HashSet<Light>();
        foreach (var (operation, from, to) in _tasks)
        {
            Action<Light> task = operation switch
            {
                Operation.On => l => lights.Add(l),
                Operation.Off => l => lights.Remove(l),
                Operation.Toggle => l =>
                {
                    if (!lights.Remove(l))
                    {
                        lights.Add(l);
                    }
                }
            };
            Apply(task, from, to);
        }
        return lights.Count;
    }

    private static void Apply(Action<Light> task, Light from, Light to)
    {
        for (var x = from.X; x <= to.X; x++)
        {
            for (var y = from.Y; y <= to.Y; y++)
            {
                task(new Light(x, y));
            }
        }
    }

    [Example("""
             turn on 0,0 through 0,0
             toggle 0,0 through 999,999
             turn off 0,0 through 999,999
             """, 1000001)]
    public override object GetAnswer2()
    {
        var lightsOn = new Dictionary<Light, int>();
        foreach (var (operation, from, to) in _tasks)
        {
            Action<Light> task = operation switch
            {
                Operation.On => l => lightsOn[l] = lightsOn.GetValueOrDefault(l)+1,
                Operation.Off => l =>
                {
                    if (!lightsOn.TryGetValue(l, out var level)) return;
                    if (level == 1)
                    {
                        lightsOn.Remove(l);
                    }
                    else
                    {
                        lightsOn[l] = level-1;
                    }
                },
                Operation.Toggle => l => lightsOn[l] = lightsOn.GetValueOrDefault(l)+2
            };
            Apply(task, from, to);
        }
        return lightsOn.Values.Sum();
    }

    private record Light(int Y, int X);
    
    private static readonly Regex Parser =
        new(@"((?:turn on)|(?:toggle)|(?:turn off)) (\d+),(\d+) through (\d+),(\d+)", RegexOptions.Compiled);

    private enum Operation
    {
        On,
        Off,
        Toggle
    }

    private readonly List<(Operation, Light, Light)> _tasks = [];
    
    protected override void Parse(string data)
    {
        foreach (var match in data.SplitLines().Select(s => Parser.Match(s)))
        {
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse:{0}", match.Groups[0].Value);
                continue;
            }

            var task = match.Groups[1].Value switch
            {
                "turn on" => Operation.On,
                "turn off" => Operation.Off,
                "toggle" => Operation.Toggle,
                _ => throw new InvalidOperationException()
            };
            _tasks.Add((task, new Light(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)), 
                new Light(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value))));
        }
    }
}
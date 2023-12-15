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

using System.ComponentModel;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay5 : SolverWithLineParser
{
    private List<long> _seeds = new List<long>();
    private List<List<(long target, long source, long lenght)>> _maps = new();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 5;
        automatonBase.RegisterTestDataAndResult(@"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4", 35, 1);
        automatonBase.RegisterTestResult(46, 2);
    }

    public override object GetAnswer1()
    {
        var minDist = long.MaxValue;
        foreach (var start in _seeds)
        {
            var seed = start;
            foreach (var map in _maps)
            {
                foreach (var entry in map)
                {
                    if (seed>=entry.source && seed<entry.source+entry.lenght)
                    {
                        seed = entry.target + seed - entry.source;
                        break;
                    }
                }
            }

            minDist = Math.Min(minDist, seed);
        }

        return minDist;
    }

    public override object GetAnswer2()
    {
        var minDist = long.MaxValue;
        for(var i= 0; i<_seeds.Count; i+=2)
        {
            for(var init= _seeds[i]; init<_seeds[i]+_seeds[i+1]; init++)
            {
                var seed = init;
                foreach (var map in _maps)
                {
                    foreach (var entry in map)
                    {
                        if (seed>=entry.source && seed<entry.source+entry.lenght)
                        {
                            seed += entry.target - entry.source;
                            break;
                        }
                    }
                }
                minDist = Math.Min(minDist, seed);
            }

        }

        return minDist;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        if (line.StartsWith("seeds"))
        {
            _seeds = line.Split(':')[1].Split(' ', StringSplitOptions.TrimEntries| StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
        }
        else
        {
            if (line.Contains("map"))
            {
                _maps.Add(new List<(long target, long source, long lenght)>());
            }
            else
            {
                var numbers = line.Split(' ', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                _maps.Last().Add((numbers[0], numbers[1], numbers[2]));
            }
        }
    }
}
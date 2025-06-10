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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay21: SolverWithLineParser
{
    private readonly Dictionary<int, List<string>?> _rulesFor2X2 = [];
    private readonly Dictionary<int, List<string>?> _rulesFor3X3 = [];

    private (int index, List<string> map) _cache;

    public DupdobDay21()
    {
        _cache = (0,
        [
            ".#.",
            "..#",
            "###"
        ]);
    }

    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 21;
        dayAutomatonBase.AddExample("""
                                 ../.# => ##./#../...
                                 .#./..#/### => #..#/..../..../#..#
                                 """).WithParameters(2).Answer1(12);
    }

    public override object GetAnswer1() => Transform(GetParameter(0, 5)).Sum(p => p.Count(c => c == '#'));

    private List<string> Transform(int generation)
    {
        var map = _cache.map;
        for (var i = _cache.index; i < generation; i++)
        {
            var split = map.Count % 2 == 0 ? 2 : 3;
            var dico = split == 2 ? _rulesFor2X2 : _rulesFor3X3;
            var block = map.Count/split;
            var newMap = new List<string>(block*(split+1));
            for (var j = 0; j < block; j++)
            {
                var local = new List<string>();
                for (var k = 0; k <= split; k++)
                {
                    local.Add(string.Empty);
                }

                for (var k = 0; k < block; k++)
                {
                    var key = Encode(map, j * split, k * split, split).First();
                    {
                        if (!dico.TryGetValue(key, out var list))
                        {
                            list = dico.First().Value;
                        }
                        for(var z = 0; z<=split; z++)
                        {
                            local[z] += list[z];
                        }
                    }

                }
                newMap.AddRange(local);
            }

            map = newMap;
        }
        _cache = (generation, map);
        return map;
    }

    private static IEnumerable<int> Encode(List<string> map, int y, int x, int size)
    {
        var result = 0;
        var bit = 1;
        // from top left, forward
        for (var i = y; i < y+size; i++)
        {
            for (var j = x; j < x+size; j++)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;
        // from top right, backward
        result = 0;
        bit = 1;
        
        for (var i = y; i < y+size; i++)
        {
            for (var j = x+size-1; j>=x; j--)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;

        result = 0;
        bit = 1;
        // from top right, forward (ie. downward first)
        for (var j = x+size-1; j>=x; j--)
        {
            for (var i = y; i <size+y; i++)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;
        // from bottom right, backward (i.e. upward)
        result = 0;
        bit = 1;
        for (var j = x+size-1; j>=x; j--)
        {
            for (var i = y+size-1; i >=y; i--)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;
        result = 0;
        bit = 1;
        // from bottom right, forward (i.e right to left)
        for (var i = y+size-1; i >=y; i--)
        {
            for (var j = x+size-1; j>=x; j--)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;

        result = 0;
        bit = 1;
        // from bottom left, backward (i.e left to right)
        for (var i = y+size-1; i >=y; i--)
        {
            for (var j = x; j<x+size; j++)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;

        result = 0;
        bit = 1;
        // from bottom left, forward (i.e upward)
        for (var j = x; j<+size; j++)
        {
            for (var i = y+size-1; i >=y; i--)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;

        result = 0;
        bit = 1;
        // from top left, backward (i.e downward)
        for (var j = x; j<x+size; j++)
        {
            for (var i = y; i <y+size; i++)
            {
                if (map[i][j] == '#')
                {
                    result += bit;
                }

                bit <<= 1;
            }
        }

        yield return result;
    }

    public override object GetAnswer2() => Transform(GetParameter(0, 18)).Sum(p => p.Count(c => c == '#'));

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var blocks = line.Split("=>", StringSplitOptions.TrimEntries);
        var map = blocks[0].Split('/', StringSplitOptions.TrimEntries).ToList();
        foreach (var key in Encode(map, 0, 0, map.Count))
        {
            var pattern = blocks[1].Split('/').ToList();
            var dico = map.Count == 2 ? _rulesFor2X2 : _rulesFor3X3;
            if (dico.TryGetValue(key, out var list))
            {
                if (pattern.Where((t, i) => t != list[i]).Any())
                {
                    throw new Exception("Invalid rule");
                }
            }
            dico[key]=pattern;
        }
    }
}
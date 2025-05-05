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

[Day(16)]
public class DupdobDay16: SolverWithParser
{
    public override void SetupRun(Automaton automaton)
    {
    }

    private readonly Dictionary<int, Dictionary<string, int>> _aunts = [];
    private readonly Dictionary<string, int> _criteria = [];
    private const string Criteria = """
                                    children: 3
                                    cats: 7
                                    samoyeds: 2
                                    pomeranians: 3
                                    akitas: 0
                                    vizslas: 0
                                    goldfish: 5
                                    trees: 3
                                    cars: 2
                                    perfumes: 1
                                    """;

    protected override void Parse(string data)
    {
        // Parse the criteria
        foreach (var line in Criteria.SplitLines())
        {
            var entry = line.Split(':', StringSplitOptions.TrimEntries);
            _criteria[entry[0]] = int.Parse(entry[1]);
        }
        // Parse the aunts
        foreach (var line in data.SplitLines())
        {
            var separator = line.IndexOf(':');
            string[] blocks = [line[0..separator], line[(separator + 1)..]];
            var auntId = int.Parse(blocks[0][4..]);
            var aunt = new Dictionary<string, int>();
            foreach (var entry in blocks[1].Split(',', StringSplitOptions.TrimEntries))
            {
                var parts = entry.Split(':', StringSplitOptions.TrimEntries);
                aunt[parts[0]] = int.Parse(parts[1]);
            }
            _aunts.Add(auntId, aunt);
        }
    }

    public override object GetAnswer1()
    {
        foreach (var (auntId, aunt) in _aunts)
        {
            var found = true;
            foreach (var (key, value) in aunt)
            {
                if (_criteria[key] == value) continue;
                found = false;
                break;
            }

            if (found)
            {
                return auntId;
            }
        }

        return null;
    }

    private readonly string[] _greaterThan = ["cats", "trees"];
    private readonly string[] _lowerThan = ["pomeranians", "goldfish"];
    public override object GetAnswer2()
    {
        foreach (var (auntId, aunt) in _aunts)
        {
            var found = true;
            foreach (var (key, value) in aunt)
            {
                if (_greaterThan.Contains(key))
                {
                    found = value > _criteria[key];
                }
                else if (_lowerThan.Contains(key))             
                {
                    found = value < _criteria[key];
                }
                else
                {
                    found = value == _criteria[key];    
                }
                if (!found) break;
            }

            if (found)
            {
                return auntId;
            }
        }

        return null;
    }
}
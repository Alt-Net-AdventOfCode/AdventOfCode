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

[Day(13)]
public class DupdobDay13: SolverWithParser
{
    public override void SetupRun(Automaton automaton)
    {
    }

    private readonly Dictionary<string, Dictionary<string, int>> _happiness = new();
    
    protected override void Parse(string data)
    {
        var lines = data.SplitLines();
        var parser = new Regex(@"(\w+) would (gain|lose) (\d+) happiness units by sitting next to (\w+).",
            RegexOptions.Compiled);
        foreach (var match in lines.Select(s => parser.Match(s)))
        {
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse line: {0}", match);
                continue;
            }
            var list = _happiness.GetValueOrDefault(match.Groups[1].Value, new Dictionary<string, int>());
            var value = (match.Groups[2].Value == "lose" ? -1 : 1) * int.Parse(match.Groups[3].Value);
            list[match.Groups[4].Value] = value;
            _happiness[match.Groups[1].Value] = list;
        }
    }

    [Example("Alice would gain 54 happiness units by sitting next to Bob.\nAlice would lose 79 happiness units by sitting next to Carol.\nAlice would lose 2 happiness units by sitting next to David.\nBob would gain 83 happiness units by sitting next to Alice.\nBob would lose 7 happiness units by sitting next to Carol.\nBob would lose 63 happiness units by sitting next to David.\nCarol would lose 62 happiness units by sitting next to Alice.\nCarol would gain 60 happiness units by sitting next to Bob.\nCarol would gain 55 happiness units by sitting next to David.\nDavid would gain 46 happiness units by sitting next to Alice.\nDavid would lose 7 happiness units by sitting next to Bob.\nDavid would gain 41 happiness units by sitting next to Carol.",
        330)]
    public override object GetAnswer1() => CreateCombinations(_happiness.Keys.ToList()).Max(ComputeHappiness);

    private int ComputeHappiness(IEnumerable<string> guests)
    {
        var happiness = 0;
        var previous = guests.Last();
        foreach (var guest in guests)
        {
            happiness += GetHappiness(guest, previous) + GetHappiness(previous, guest);
            previous = guest;
        }

        return happiness;

        int GetHappiness(string t, string s)
        {
            if (!_happiness.TryGetValue(t, out var map) || !map.TryGetValue(s, out var value))
            {
                return 0;
            }

            return value;
        }
    }
    
    // this method creates all sitting combinations for the guests having the first one fixed
    private IEnumerable<IEnumerable<string>> CreateCombinations(List<string> guests) => CreateSubCombinations(guests[1..]).Select(sub => sub.Prepend(guests[0]));

    private IEnumerable<IEnumerable<string>> CreateSubCombinations(List<string> guests)
    {
        if (guests.Count == 1)
        {
            yield return guests;
            yield break;
        }
        for (var i = 0; i < guests.Count; i++)
        {
            var sublist = new List<string>(guests);
            sublist.RemoveAt(i);
            foreach (var sub in CreateSubCombinations(sublist))
            {
                yield return sub.Prepend(guests[i]);
            }
        }
    }

    public override object GetAnswer2() => CreateCombinations(_happiness.Keys.Append("me").ToList()).Max(ComputeHappiness);
}
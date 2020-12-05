using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2015
{
    public class DupdobDay13
    {
        private IDictionary<string, IDictionary<string, int>> _happiness = new Dictionary<string, IDictionary<string, int>>();
        
        public void Parse(string input = Input)
        {
            const string template = "(\\w+)\\b would (gain|lose) (\\d+) happiness units by sitting next to (\\w+).";

            var parser = new Regex(template, RegexOptions.Compiled);
            foreach (var line in input.Split('\n'))
            {
                var match = parser.Match(line);
                if (!match.Success)
                {
                    Console.WriteLine("Failed to parse line: {0}", line);
                    continue;
                }

                var name = match.Groups[1].Value;
                if (!_happiness.ContainsKey(name))
                {
                    _happiness[name] = new Dictionary<string, int>();
                }

                var increment = int.Parse(match.Groups[3].Value);
                if (match.Groups[2].Value == "lose")
                {
                    increment = -increment;
                }
                _happiness[name][match.Groups[4].Value] = increment;
            }
        }

        public object? Compute1()
        {
            var guests = _happiness.Keys;
            return GetMaxHapinness(guests);
        }

        private object GetMaxHapinness(ICollection<string> guests)
        {
            var maxHapinness = int.MinValue;
            foreach (var combo in EnumerateCombination(guests))
            {
                var list = combo.ToArray();
                var temp = 0;
                for (var i = 0; i < list.Length; i++)
                {
                    temp += GetHappiness(list[i], list[(i + 1) % list.Length]);
                    temp += GetHappiness(list[i], list[(i + list.Length - 1) % list.Length]);
                }

                if (maxHapinness < temp)
                {
                    maxHapinness = temp;
                    Console.WriteLine("{0} = {1}", maxHapinness, string.Join(',', list));
                }
            }

            return maxHapinness;
        }

        private int GetHappiness(string name1, string name2)
        {
            if (name2 == "Me" || name1=="Me")
                return 0;
            return _happiness[name1][name2];
        }
        private IEnumerable<IEnumerable<string>> EnumerateCombination(IEnumerable<string> list)
        {
            if (!list.Any())
            {
                yield return ArraySegment<string>.Empty;
                yield break;
            }
            foreach (string entry in list)
            foreach (var sub in EnumerateCombination(list.Where(t => t != entry)))
                yield return sub.Append(entry);
        }
        
        public object? Compute2()
        {
            var guests = _happiness.Keys.ToList();
            guests.Add("Me");
            return GetMaxHapinness(guests);
        }

        private const string Input = @"Alice would gain 54 happiness units by sitting next to Bob.
Alice would lose 81 happiness units by sitting next to Carol.
Alice would lose 42 happiness units by sitting next to David.
Alice would gain 89 happiness units by sitting next to Eric.
Alice would lose 89 happiness units by sitting next to Frank.
Alice would gain 97 happiness units by sitting next to George.
Alice would lose 94 happiness units by sitting next to Mallory.
Bob would gain 3 happiness units by sitting next to Alice.
Bob would lose 70 happiness units by sitting next to Carol.
Bob would lose 31 happiness units by sitting next to David.
Bob would gain 72 happiness units by sitting next to Eric.
Bob would lose 25 happiness units by sitting next to Frank.
Bob would lose 95 happiness units by sitting next to George.
Bob would gain 11 happiness units by sitting next to Mallory.
Carol would lose 83 happiness units by sitting next to Alice.
Carol would gain 8 happiness units by sitting next to Bob.
Carol would gain 35 happiness units by sitting next to David.
Carol would gain 10 happiness units by sitting next to Eric.
Carol would gain 61 happiness units by sitting next to Frank.
Carol would gain 10 happiness units by sitting next to George.
Carol would gain 29 happiness units by sitting next to Mallory.
David would gain 67 happiness units by sitting next to Alice.
David would gain 25 happiness units by sitting next to Bob.
David would gain 48 happiness units by sitting next to Carol.
David would lose 65 happiness units by sitting next to Eric.
David would gain 8 happiness units by sitting next to Frank.
David would gain 84 happiness units by sitting next to George.
David would gain 9 happiness units by sitting next to Mallory.
Eric would lose 51 happiness units by sitting next to Alice.
Eric would lose 39 happiness units by sitting next to Bob.
Eric would gain 84 happiness units by sitting next to Carol.
Eric would lose 98 happiness units by sitting next to David.
Eric would lose 20 happiness units by sitting next to Frank.
Eric would lose 6 happiness units by sitting next to George.
Eric would gain 60 happiness units by sitting next to Mallory.
Frank would gain 51 happiness units by sitting next to Alice.
Frank would gain 79 happiness units by sitting next to Bob.
Frank would gain 88 happiness units by sitting next to Carol.
Frank would gain 33 happiness units by sitting next to David.
Frank would gain 43 happiness units by sitting next to Eric.
Frank would gain 77 happiness units by sitting next to George.
Frank would lose 3 happiness units by sitting next to Mallory.
George would lose 14 happiness units by sitting next to Alice.
George would lose 12 happiness units by sitting next to Bob.
George would lose 52 happiness units by sitting next to Carol.
George would gain 14 happiness units by sitting next to David.
George would lose 62 happiness units by sitting next to Eric.
George would lose 18 happiness units by sitting next to Frank.
George would lose 17 happiness units by sitting next to Mallory.
Mallory would lose 36 happiness units by sitting next to Alice.
Mallory would gain 76 happiness units by sitting next to Bob.
Mallory would lose 34 happiness units by sitting next to Carol.
Mallory would gain 37 happiness units by sitting next to David.
Mallory would gain 40 happiness units by sitting next to Eric.
Mallory would gain 18 happiness units by sitting next to Frank.
Mallory would gain 7 happiness units by sitting next to George.";
    }
}
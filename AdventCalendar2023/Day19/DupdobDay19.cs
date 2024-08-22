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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay19: SolverWithLineParser
{
    private static string Ids = "xmas";
    private class Filter
    {
        private readonly List<(int attribute, bool lower,  int threshold, string target)> _criteria = new();
        
        public string Evaluate(List<int> item)
        {
            foreach (var rule in _criteria)
            {
                if ((rule.lower && item[rule.attribute]<rule.threshold) || (!rule.lower && item[rule.attribute]>rule.threshold))
                {
                    return rule.target;
                }
            }

            throw new Exception("General criteria is missing");
        }

        public long PossibleItems(Dictionary<string, Filter> filters, (int min, int max)[] ranges)
        {
            var result = 0L;
            var nextRange = ((int min, int max)[])ranges.Clone(); 
            foreach ((int attribute, bool lower, int threshold, string target)  in _criteria)
            {
                var newRange = ((int min, int max)[])nextRange.Clone(); 
                if (lower)
                {
                    if (newRange[attribute].min > threshold)
                    {
                        // impossible
                        continue;
                    }

                    newRange[attribute].max = Math.Min(threshold -1, newRange[attribute].max);
                    nextRange[attribute].min = Math.Max(threshold, nextRange[attribute].min);
                }
                else
                {
                    if (newRange[attribute].max < threshold)
                    {
                        // impossible
                        continue;
                    }

                    newRange[attribute].min = Math.Max(threshold+1, newRange[attribute].min);
                    nextRange[attribute].max = Math.Min(threshold, nextRange[attribute].max);
                }

                switch (target)
                {
                    case "R":
                        continue;
                    case "A":
                        var possibilities = 1L;
                        foreach (var (min, max) in newRange)
                        {
                            possibilities *= (max - min + 1);
                        }

                        result += possibilities;
                        break;
                    default:
                        result += filters[target].PossibleItems(filters, newRange);
                        break;
                }

                if (nextRange[attribute].min > nextRange[attribute].max)
                {
                    // there is no possible combination left
                    break;
                }
            }

            return result;
        }

        public void Parse(string line)
        {
            foreach (var rule in line.Split(','))
            {
                var tokens = rule.Split(':');
                if (tokens.Length == 1)
                {
                    _criteria.Add((0, false, 0, tokens[0]));
                }
                else
                {
                    var attribute = Ids.IndexOf(tokens[0][0]);
                    var lower = tokens[0][1] == '<';
                    var threshold = int.Parse(tokens[0].Substring(2));
                    _criteria.Add((attribute, lower, threshold, tokens[1]));
                }
            }
        }
    }

    private readonly Dictionary<string, Filter> _filters = new();
    private readonly List<List<int>> _items = new();
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 19;
        automatonBase.RegisterTestDataAndResult(@"px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}", 19114, 1);
        automatonBase.RegisterTestResult(167409079868000L, 2);
    }

    public override object GetAnswer1()
    {
        var result = 0L;
        foreach (var item in _items)
        {
            var next = "in";
            for (;next!="R" && next!="A";)
            {
                next = _filters[next].Evaluate(item);
            }
            if (next == "A")
            {
                result = item.Aggregate(result, (current, attribute) => current + attribute);
            }
        }

        return result;
    }

    public override object GetAnswer2()
    {
        var fullScope = new[]{ (1, 4000), (1, 4000), (1, 4000), (1, 4000) };
        return _filters["in"].PossibleItems(_filters, fullScope);
    }

    private readonly Regex _parser = new("(.*)\\{([^\\}]+)\\}", RegexOptions.Compiled);

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _parser.Match(line);
        if (!match.Success)
        {
            return;
        }

        if (string.IsNullOrEmpty(match.Groups[1].Value))
        {
            var item = new List<int>{0,0,0,0};
            foreach (var attribute in match.Groups[2].Value.Split(','))
            {
                var blocs = attribute.Split('=');
                item[Ids.IndexOf(blocs[0][0])] = int.Parse(blocs[1]);
            }
            _items.Add(item);
        }
        else
        {
            var entry = new Filter();
            entry.Parse(match.Groups[2].Value);
            _filters[match.Groups[1].Value] = entry;
        }
    }
}
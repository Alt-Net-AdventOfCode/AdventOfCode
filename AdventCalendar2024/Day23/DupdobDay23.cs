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

using System.Reflection.PortableExecutable;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay23: SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 23;
        automatonBase.RegisterTestDataAndResult("""
                                                kh-tc
                                                qp-kh
                                                de-cg
                                                ka-co
                                                yn-aq
                                                qp-ub
                                                cg-tb
                                                vc-aq
                                                tb-ka
                                                wh-tc
                                                yn-cg
                                                kh-ub
                                                ta-co
                                                de-co
                                                tc-td
                                                tb-wq
                                                wh-td
                                                ta-ka
                                                td-qp
                                                aq-cg
                                                wq-ub
                                                ub-vc
                                                de-ta
                                                wq-aq
                                                wq-vc
                                                wh-yn
                                                ka-de
                                                kh-ta
                                                co-tc
                                                wh-qp
                                                tb-vc
                                                td-yn
                                                """, 7, 1);
        automatonBase.RegisterTestResult("co,de,ka,ta", 2); 
    }

    public override object GetAnswer1()
    {
        _neighbors = new Dictionary<string, HashSet<string>>();
        foreach (var (c1, c2)  in _links)
        {
            if (!_neighbors.TryGetValue(c1, out var links))
            {
                _neighbors[c1] = links = [];
            }

            links.Add(c2);
            if (!_neighbors.TryGetValue(c2, out links))
            {
                _neighbors[c2] = links = [];
            }

            links.Add(c1);
        }

        _groups = new HashSet<(string c1, string c2, string c3)>(_links.Count);
        foreach (var (lead, list) in _neighbors)
        {
            foreach (List<string>? group in list.SelectMany(computer => from lastOne in _neighbors[computer] where lastOne != lead && _neighbors[lead].Contains(lastOne) select new List<string> { lead, computer, lastOne }.Order().ToList()))
            {
                _groups.Add((group[0], group[1], group[2]));
            }
        }

        return _groups.Count(g => g.c1[0] == 't' || g.c2[0] == 't' || g.c3[0] == 't');
    }

    public override object GetAnswer2()
    {
        var result = 0;
        var max = new List<string>();
        foreach (var (c1, c2, c3) in _groups)
        {
            var inGroup = new List<string> { c1, c2, c3 };
            var commons = _neighbors[c1].Intersect(_neighbors[c2]).Intersect(_neighbors[c3]).ToHashSet();
            if (commons.Count + inGroup.Count <= max.Count)
            {
                // we can't reach same size
                continue;
            }
            var sub = MaxGroup(inGroup, commons);
            if (sub.Count > max.Count)
            {
                max = sub;
            }
        }
        
        return string.Join(',', max.Order());
    }

    private List<string> MaxGroup(List<string> inGroup, HashSet<string> commons)
    {
        var result = inGroup;
        // now we need to check if we can add computers
        foreach (var extra in commons)
        {
            var next = inGroup.Append(extra).ToList();

            var sub = MaxGroup(next, commons.Intersect(_neighbors[extra]).ToHashSet());
            if (sub.Count > result.Count)
            {
                result = sub;
                if (inGroup.Count + commons.Count == sub.Count)
                {
                    // we reached max possible size
                    return sub;
                }
            }
        }

        return result;
    }

    private readonly List<(string c1, string c2)> _links = [];
    private HashSet<(string c1, string c2, string c3)> _groups = [];
    private Dictionary<string, HashSet<string>> _neighbors = [];

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var bloc = line.Split('-');
        _links.Add((bloc[0], bloc[1]));
    }
}
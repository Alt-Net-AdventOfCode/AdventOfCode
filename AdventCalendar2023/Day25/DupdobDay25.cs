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

namespace AdventCalendar2023;

public class DupdobDay25 : SolverWithLineParser
{
    private readonly Dictionary<string, List<string>> _graph = new();
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 25;
        dayAutomatonBase.RegisterTestDataAndResult(@"jqt: rhn xhk nvd
rsh: frs pzl lsr
xhk: hfx
cmg: qnr nvd lhk bvb
rhn: xhk bvb hfx
bvb: xhk hfx
pzl: lsr hfx nvd
qnr: nvd
ntq: jqt hfx bvb xhk
nvd: lhk
lsr: lhk
rzs: qnr cmg lsr rsh
frs: qnr lhk lsr", 54, 1);
    }
    
    public override object GetAnswer1()
    {
        foreach (var entry in _graph.ToList())
        {
            foreach (var node in entry.Value)
            {
                if (!_graph.TryGetValue(node, out var nodes))
                {
                    _graph[node] = new List<string>{entry.Key};
                }
                else if (!nodes.Contains(entry.Key))
                {
                    nodes.Add(entry.Key);
                }
            }
        }


        while (true)
        {
            var graphs = _graph.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
            var mergedNodes = graphs.ToDictionary(kvp => kvp.Key, kvp => 1);
            while (graphs.Count > 2)
            {
                var first = graphs.Keys.Random();
                var second = graphs[first].Random();
                // first node will swallow the second one
                // merge nodes
                while(graphs[second].Remove(first)){}
                while(graphs[first].Remove(second)){}
                graphs[first].AddRange(graphs[second]);
                graphs.Remove(second);
                mergedNodes[first] += mergedNodes[second];
                mergedNodes.Remove(second);                    
                // replace link to second
                foreach (var links in graphs.Values)
                {
                    links.Replace(second, first);
                }
            }

            if (graphs.Values.First().Count == 3)
            {
                return mergedNodes.Values.First()*mergedNodes.Values.ElementAt(1);
            }
        }
    }

    public override object GetAnswer2()
    {
        // no need on day 25
        return 0;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var blocs = line.Split(':');
        _graph[blocs[0]] = blocs[1].Split(' ', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
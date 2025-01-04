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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2017;

public partial class DupdobDay07 : SolverWithDataAsLines
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 7;
        automatonBase.RegisterTestDataAndResult("""
                                                pbga (66)
                                                xhth (57)
                                                ebii (61)
                                                havc (66)
                                                ktlj (57)
                                                fwft (72) -> ktlj, cntj, xhth
                                                qoyq (66)
                                                padx (45) -> pbga, havc, qoyq
                                                tknk (41) -> ugml, padx, fwft
                                                jptl (61)
                                                ugml (68) -> gyxo, ebii, jptl
                                                gyxo (61)
                                                cntj (57)
                                                """, "tknk", 1).RegisterTestResult(60, 2); 
    }

    public override object GetAnswer1()
    {
        _root = _nodes.Keys.First(node => !_nodes.Values.Any(p => p.Children.Contains(node)));
        return _root;
    }

    public override object GetAnswer2()
    {
        var scan = _root!;
        var errorValue = 0;
        
        return LookForError(scan, errorValue);
    }

    private int LookForError(string scan, int errorValue)
    {
        var rootNode = _nodes[scan];
        var weights = new Dictionary<string, int>();
        var hits = new Dictionary<int, int>();
        // scan weights of children
        foreach (var child in rootNode.Children)
        {
            var localWeight = TotalWeight(_nodes[child]);
            weights[child] = localWeight;
            hits[localWeight] = hits.GetValueOrDefault(localWeight) + 1;
        }
        // if all children have the same weight, the error in the parent
        if (hits.Count == 1)
        {
            return rootNode.Weight+errorValue;
        }
        // scan the child that has a different weight from the others
        var error = hits.FirstOrDefault(k => k.Value == 1).Key;
        var norm = hits.First(k => k.Value>1).Key;
        return LookForError(weights.First(k => k.Value == error).Key, norm - error);
    }

    private int TotalWeight(Node node) => node.Weight + node.Children.Sum(child => TotalWeight(_nodes[child]));

    private readonly Regex _parser = MyRegex();

    private record Node(string Name, int Weight, List<string> Children);

    private readonly Dictionary<string, Node> _nodes = [];
    private string? _root;

    protected override void ParseLines(string[] lines)
    {
        foreach (var line in lines)
        {
            var match = _parser.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse {0}", line);
                continue;
            }

            var children = match.Groups[3].Success ?  match.Groups[4].Value.Split(',', StringSplitOptions.TrimEntries).ToList() : [];

            var node = new Node(match.Groups[1].Value, int.Parse(match.Groups[2].Value), children);
            _nodes.Add(node.Name, node);
        }
    }

    [GeneratedRegex(@"(\w*)\W\((\d+)\)(\W->\W(.*))?")]
    private static partial Regex MyRegex();
}
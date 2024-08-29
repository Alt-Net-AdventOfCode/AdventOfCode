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

using AoC;
using AoCAlgorithms;

namespace AdventCalendar2023;

public class DupdobDay8 : SolverWithLineParser
{

    private readonly Dictionary<string, (string left, string right)> _graph = new();
    private string _instructions = "";
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 8;
        automatonBase.RegisterTestDataAndResult(@"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)", 2, 1);
        automatonBase.RegisterTestDataAndResult(@"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)", 6, 1);

        automatonBase.RegisterTestDataAndResult(@"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)", 0, 1);
        automatonBase.RegisterTestResult(6, 2);
    }

    public override object GetAnswer1()
    {
        var steps = 0;
        if (!_graph.ContainsKey("AAA"))
        {
            return steps;
        }
        var current = "AAA";
        while (current!="ZZZ")
        {
            current = _instructions[steps++ % _instructions.Length] == 'R' ? _graph[current].right : _graph[current].left;
        }
        return steps;
    }

    public override object GetAnswer2()
    {
        var currents = _graph.Keys.Where(s => s.EndsWith('A')).ToArray();
        var loopLengths = new int[currents.Length];
        var steps = 0;
        var keepOn = true;
        while (keepOn)
        {
            var goRight = _instructions[steps++ % _instructions.Length] == 'R';
            for (var index = 0; index < currents.Length; index++)
            {
                if (loopLengths[index]>0) continue;
                var current = currents[index];
                var nextNode = goRight ? _graph[current].right : _graph[current].left;
                if (nextNode.EndsWith('Z'))
                {
                    loopLengths[index] = steps;
                    keepOn = loopLengths.Any(l => l == 0);
                }
                currents[index] = nextNode;
            }
        }

        return loopLengths.Aggregate(1L, (acc, next) => MathHelper.Lcm(acc, next));
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (index == 0)
        {
            _instructions = line;
            return;
        }

        if (line.Contains('='))
        {
            var blocks = line.Split('=', StringSplitOptions.TrimEntries);
            var name = blocks[0];
            var nodes = blocks[1].Trim('(').Trim(')').Split(',', StringSplitOptions.TrimEntries);
            _graph[name] = (nodes[0], nodes[1]);
        }
    }
}
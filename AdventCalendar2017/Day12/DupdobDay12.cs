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

using System.Security.Cryptography;
using AoC;

namespace AdventCalendar2017;

public class DupdobDay12 : SolverWithDataAsLines
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 12;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                0 <-> 2
                                                1 <-> 1
                                                2 <-> 0, 3, 4
                                                3 <-> 2, 4
                                                4 <-> 2, 3, 6
                                                5 <-> 6
                                                6 <-> 4, 5
                                                """, 6, 1).RegisterTestResult(2,2);
    }

    public override object GetAnswer1()
    {
        // ensure bidirectionality
        foreach (var (program, subs) in _connections)
        {
            foreach (var sub in subs)
            {
                _connections[sub].Add(program);
            }
        }
        return VisitGroup("0").Count;
    }

    private HashSet<string> VisitGroup(string current)
    {
        var visited = new HashSet<string>(_connections.Count);
        var pending = new Queue<string>();
        pending.Enqueue(current);
        while (pending.TryDequeue(out current))
        {
            foreach (var neighbor in _connections[current].Where(neighbor => visited.Add(neighbor)))
            {
                pending.Enqueue(neighbor);
            }
        }

        return visited;
    }

    public override object GetAnswer2()
    {
        var programs = _connections.Keys.ToList();
        var groups = 0;
        while (programs.Count>0)
        {
            var group = VisitGroup(programs[0]);
            groups++;
            programs = programs.Except(group).ToList();
        }

        return groups;
    }

    private readonly Dictionary<string, HashSet<string>> _connections = [];
    protected override void ParseLines(string[] lines)
    {
        foreach (var line in lines)
        {
            var blocs = line.Split("<->", StringSplitOptions.TrimEntries);
            var subPrograms = blocs[1].Split(',', StringSplitOptions.TrimEntries).ToHashSet();
            _connections[blocs[0]] = subPrograms;
        }
    }
}
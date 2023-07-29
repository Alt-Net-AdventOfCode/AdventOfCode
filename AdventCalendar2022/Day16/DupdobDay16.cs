// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2022;

public partial class DupdobDay16 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 16;
        automaton.RegisterTestData(@"Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
Valve BB has flow rate=13; tunnels lead to valves CC, AA
Valve CC has flow rate=2; tunnels lead to valves DD, BB
Valve DD has flow rate=20; tunnels lead to valves CC, AA, EE
Valve EE has flow rate=3; tunnels lead to valves FF, DD
Valve FF has flow rate=0; tunnels lead to valves EE, GG
Valve GG has flow rate=0; tunnels lead to valves FF, HH
Valve HH has flow rate=22; tunnel leads to valve GG
Valve II has flow rate=0; tunnels lead to valves AA, JJ
Valve JJ has flow rate=21; tunnel leads to valve II");
        automaton.RegisterTestResult(1651);
        automaton.RegisterTestResult(1707,2);
    }

    private void CreateNetwork(string valveId, IEnumerable<string> wanted)
    {
        var valve = _valves[valveId];
        var toFind = new List<string>(wanted);
        toFind.Remove(valveId);
        var pending = new List<Valve>();
        var shortestDist = new Dictionary<string, int>
        {
            [valve.Name] = 0
        };
        pending.Add(valve);
        while (pending.Count>0)
        {
            var nextValve = valve;
            var shortestDistance = int.MaxValue;
            foreach (var temp in pending.Where(temp => shortestDist[temp.Name] < shortestDistance))
            {
                shortestDistance = shortestDist[temp.Name];
                nextValve = temp;
            }

            pending.Remove(nextValve);
            shortestDistance++;
            foreach (var neighbor in nextValve.Next.Where(neighbor => !shortestDist.ContainsKey(neighbor)))
            {
                shortestDist[neighbor] = shortestDistance;
                var item = _valves[neighbor];
                if (toFind.Contains(neighbor))
                {
                    toFind.Remove(neighbor);
                    valve.Network.Add((item, shortestDistance));
                }
                pending.Add(item);
            }
            if (toFind.Count == 0)
            {
                break;
            }
        }
    }

    public override object GetAnswer1()
    {
        var valuedNodes = (from valve in _valves.Values where valve.Flow > 0 select valve.Name).ToList();
        // optimize network
        CreateNetwork("AA", valuedNodes);
        foreach (var node in valuedNodes)
        {
            CreateNetwork(node, valuedNodes);
        }

        return Search(_valves["AA"], 0, 30, valuedNodes);
    }

    private static int Search(Valve valve, int maxScore, int time, ICollection<string> opened)
    {
        if (valve.Flow>0)
        {
            // we open
            time--;
            maxScore += time * valve.Flow;
        }

        if (time <= 1 || opened.Count==0)
        {
            // we can't improve the score
            return maxScore;
        }
        
        var currentScore = maxScore;
        // we move to each next tunnel
        foreach (var (cave, distance) in valve.Network)
        {
            if (!opened.Contains(cave.Name))
            {
                continue;
            }
            var cloned = new List<string>(opened);
            cloned.Remove(cave.Name);
            var subScore = Search(cave, currentScore, time - distance, cloned);
            if (subScore > maxScore)
            {
                maxScore = subScore;
            }
        }
        // restore state
        return maxScore;
    }

    public override object GetAnswer2() 
    {
        var valuedNodes = (from valve in _valves.Values where valve.Flow > 0 select valve.Name).ToList();
        return Search2((_valves["AA"], 26),(_valves["AA"], 26), 0, valuedNodes);
    }
    
    private static int Search2((Valve valve, int time) me, (Valve valve, int time) elephant, int maxScore,
        ICollection<string> opened)
    {

        if (me.valve.Flow>0)
        {
            // we open
            me.time--;
            maxScore += me.time * me.valve.Flow;
        }

        if (me.time < elephant.time)
        {
            // elephant moves, we invert 
            (elephant, me) = (me, elephant);
        }
        
        if (me.time <= 1|| opened.Count == 0)
        {
            // we can't improve the score
            return maxScore;
        }
        
        var currentScore = maxScore;
        // we move to each next tunnel
        foreach (var (cave, distance) in me.valve.Network)
        {
            if (!opened.Contains(cave.Name))
            {
                continue;
            }
            var cloned = new List<string>(opened);
            cloned.Remove(cave.Name);
            var subScore = Search2((cave, me.time-distance), elephant, currentScore, cloned);
            if (subScore > maxScore)
            {
                maxScore = subScore;
            }
        }
        // restore state
        return maxScore;
    }

    private class Valve
    {
        public string Name = string.Empty;
        public int Flow;
        public List<string> Next = new();
        public readonly List<(Valve next, int cost)> Network = new();
    }

    private readonly Dictionary<string, Valve> _valves = new();
    private readonly Regex _parser = MyRegex();
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _parser.Match(line);
        if (match.Success)
        {
            var valve = new Valve
            {
                Name = match.Groups[1].Value,
                Flow = int.Parse(match.Groups[2].Value),
                Next = match.Groups[3].Value.Split(',').Select(s => s.Trim()).ToList()
            };
            _valves[valve.Name] = valve;
        }
        else
        {
            Console.WriteLine("failed to parse "+line);
        }
    }

    [GeneratedRegex("Valve (..) has flow rate=(\\d+); tunnels? leads? to valves? (.*)")]
    private static partial Regex MyRegex();
}
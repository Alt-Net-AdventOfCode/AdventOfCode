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
using AoCAlgorithms;

namespace AdventCalendar2022;

public partial class DupdobDay19 : SolverWithLineParser
{
    private const int Ore = 0;
    private const int Clay = 1;
    private const int Obsidian = 2;
    private const int Geode = 3;

    private readonly List<Dictionary<int, int[]>> _blueprints = new();
    private readonly int[] _maxRobots = {0, 0, 0, int.MaxValue};

    private readonly Regex _parser =
        MyRegex();

    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 19;
        dayAutomaton.AddExample(
            @"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.");
        dayAutomaton.RegisterTestResult(33);
    }

    public override object GetAnswer1() => _blueprints.Select((t, i) => (i + 1) * MaxOfGeodes(t,24)).Sum();

    private int MaxOfGeodes(Dictionary<int,int[]> blueprint, int time)
    {
        var seen = new HashSet<State>();
        var priority = new PriorityQueue<State, int>();
        var newState = new State(blueprint, time);
        priority.Enqueue(newState, 0);
        var maxGeode = 0;
        while (priority.Count > 0)
        {
            foreach (var state in priority.Dequeue().EnumerateNext(_maxRobots))
            {
                if (!seen.Add(state) || maxGeode > state.MaximumGeodesAtEnd())
                {
                    // already seen or no chance to beat the max
                    continue;
                }
                maxGeode = Math.Max(maxGeode, state.GeodesAtEnd());
                priority.Enqueue(state, -state.GeodesAtEnd());
            }
        }

        return maxGeode;
    }

    public override object GetAnswer2() => MaxOfGeodes(_blueprints[0], 32) * MaxOfGeodes(_blueprints[1], 32) * MaxOfGeodes(_blueprints[2], 32);

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _parser.Match(line);
        if (!match.Success)
        {
            Console.WriteLine($"Failed to parse {line}.");
            return;
        }

        var blueprint = new Dictionary<int, int[]>
        {
            [Ore] = new[] { int.Parse(match.Groups[2].Value), 0, 0 },
            [Clay] = new[] { int.Parse(match.Groups[3].Value), 0, 0 },
            [Obsidian] = new[] { int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), 0 },
            [Geode] = new[] { int.Parse(match.Groups[6].Value), 0, int.Parse(match.Groups[7].Value) }
        };
        
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                _maxRobots[j] = Math.Max(_maxRobots[j], blueprint[i][j]);
            }
        }
        _blueprints.Add(blueprint);
    }

    [GeneratedRegex(
        "Blueprint (\\d+): Each ore robot costs (\\d+) ore. Each clay robot costs (\\d+) ore. Each obsidian robot costs (\\d+) ore and (\\d+) clay. Each geode robot costs (\\d+) ore and (\\d+) obsidian.")]
    private static partial Regex MyRegex();

    private class State : IEquatable<State>
    {
        private readonly Dictionary<int, int[]> _bluePrint;
        private readonly int[] _robots = new int [4];
        private readonly int[] _stocks = new int [4];
        private readonly int _time;

        public State(Dictionary<int, int[]> bluePrint, int time)
        {
            _bluePrint = bluePrint;
            _robots[Ore] = 1;
            _time = time;
        }
        
        private State(State duplicate, int addedRobot, int waitTime)
        {
            var cost = duplicate._bluePrint[addedRobot];
            _bluePrint = duplicate._bluePrint;
            for (var i = 0; i < 3; i++) _stocks[i] = duplicate._stocks[i] - cost[i] + duplicate._robots[i]*waitTime;
            _stocks[Geode] = duplicate._stocks[Geode] + duplicate._robots[Geode] * waitTime;
            _robots = duplicate._robots.ToArray();
            _robots[addedRobot]++;
            _time = duplicate._time - waitTime;
        }

        public int GeodesAtEnd() => _stocks[Geode] + _robots[Geode] * _time;
        public int MaximumGeodesAtEnd() => _stocks[Geode]+(_robots[Geode] + _time) * (_robots[Geode] + _time - 1) / 2;

        public IEnumerable<State> EnumerateNext(int[] maxRobots)
        {
            for (var robot = Geode; robot >= Ore; robot--)
            {
                var waitTime = 0;
                for (var j = 2; j >= 0; j--)
                {
                    if (_bluePrint[robot][j] <= _stocks[j]) continue;
                    // we lack resources
                    if (_robots[j] == 0)
                    {
                        // no producer, skip this robot, we can't build it
                        waitTime = int.MaxValue;
                        break;
                    }
                    waitTime = Math.Max(waitTime,
                        MathHelper.RoundedUpDivision(_bluePrint[robot][j] - _stocks[j], _robots[j]));
                }

                if (waitTime >= _time-1)
                {
                    // no time to build it.
                    continue;
                }

                var next = new State(this, robot, waitTime+1);
                if (maxRobots.Where((t, i) => next._robots[i] > t).Any())
                {
                    continue;
                }
                yield return next;
            }
        }

        public bool Equals(State? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (_time != other._time)
            {
                return false;
            }
            for (var i = 0; i < 4; i++)
            {
                if (_robots[i] != other._robots[i] || _stocks[i] != other._stocks[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((State)obj);
        }

        public override int GetHashCode()
        {
            var hash = _time;
            const int store = 25;
            var multiplier = store;
            foreach (var t in _robots)
            {
                hash += t * multiplier;
                multiplier *= store;
            }
            foreach (var t in _stocks)
            {
                hash += t * multiplier;
                multiplier *= store;
            }

            return hash;
        }

        public static bool operator ==(State? left, State? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(State? left, State? right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"T:{_time};R: {string.Join(',', _robots)}/S: {string.Join(',',_stocks)}";
        }
    }
}
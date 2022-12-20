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

public partial class DupdobDay19: SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 19;
        automaton.RegisterTestData(@"Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
Blueprint 2: Each ore robot costs 2 ore. Each clay robot costs 3 ore. Each obsidian robot costs 3 ore and 8 clay. Each geode robot costs 3 ore and 12 obsidian.");
        automaton.RegisterTestResult(33);
    }

    private class State
    {
        private readonly int[] _stocks = new int [4];
        private readonly int[] _robots = new int [4];
        private readonly Dictionary<int, int[]> _bluePrint;
        private int _time;

        public State(Dictionary<int, int[]> bluePrint)
        {
            _bluePrint = bluePrint;
            _robots[Ore] = 1; 
            _time = 19;
        }

        private State(State duplicate, int addedRobot)
        {
            var cost = duplicate._bluePrint[addedRobot];
            _bluePrint = duplicate._bluePrint;
            for (var i = 0; i < 3; i++)
            {
                _stocks[i] = duplicate._stocks[i] - cost[i];
            }
            _robots = duplicate._robots.ToArray();
            _robots[addedRobot]++;
            _time = duplicate._time-1;
        }

        private State(State duplicate)
        {
            _bluePrint = duplicate._bluePrint;
            _stocks = duplicate._stocks.ToArray();
            _robots = duplicate._robots.ToArray();
            _time = duplicate._time;
        }

        private State? BuildRobot(int kind)
        {
            var cost = _bluePrint[kind];
            if (cost[Ore] > _stocks[Ore] || cost[Clay] > _stocks[Clay] ||
                cost[Obsidian] > _stocks[Obsidian])
            {
                return null;
            }
            return new State(this, kind);
        }

        private void Collect()
        {
            for(var i=0; i<4; i++)
            {
                _stocks[i] += _robots[i];
            }
        }
        
       public int MaximizeGeode()
       {
           var maxScore = 0;
           while (_time>0)
           {
               for(var i=3; i>=0; i--)
               {
                   var nextState = BuildRobot(i);
                   if (nextState == null)
                   {
                       continue;
                   }
                   nextState.Collect();
                   maxScore = Math.Max(maxScore, nextState.MaximizeGeode());
               }
               Collect();
               _time--;
           }

           return Math.Max(maxScore, _stocks[Geode]);
       }

       public int FastestGeodeRobot()
       {
           // fastest time to get one geode robot.
           // try
           var fastestTime = FastestObsidianRobot() + _bluePrint[Geode][Obsidian];
           return fastestTime;
       }

       private int FastestObsidianRobot()
       {
           var fastestTime = FastestClayRobot() + _bluePrint[Obsidian][Clay];
           return fastestTime;
       }

       private int FastestClayRobot(int target = 1)
       {
           var fastestTime = _bluePrint[Clay][Ore];
           return fastestTime;
       }

       private State FastestOreStock(int target = 1)
       {
           var state = new State(this);
           var waitTime = MathHelper.RoundedUpDivision((target - _stocks[Ore]), _robots[0]);
           for (var i = 0; i < waitTime; i++)
           {
               state.Collect();               
           }
           if (waitTime < 2)
           {
                // can't go faster
                return state;
           }

           return state;
       }
    }
    
    public override object GetAnswer1()
    {
        var score = 0;
        for (var i = 0; i < _blueprints.Count; i++)
        {   
            var state = new State(_blueprints[i]);
            var max = state.MaximizeGeode();
            score += (i + 1) * max;
        }

        return score;
    }

    public override object GetAnswer2()
    {
        throw new NotImplementedException();
    }

    private readonly Regex _parser =
        MyRegex();

    private const int Ore = 0;
    private const int Clay = 1;
    private const int Obsidian = 2;
    private const int Geode = 3;
    
    private readonly List<Dictionary<int, int[]>> _blueprints = new(); 
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
            [Ore] = new[] {int.Parse(match.Groups[2].Value), 0, 0},
            [Clay] = new [] {int.Parse(match.Groups[3].Value), 0, 0},
            [Obsidian] = new []{int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), 0},
            [Geode] = new []{int.Parse(match.Groups[6].Value), 0, int.Parse(match.Groups[7].Value)}
        };
        _blueprints.Add(blueprint);
    }

    [GeneratedRegex("Blueprint (\\d+): Each ore robot costs (\\d+) ore. Each clay robot costs (\\d+) ore. Each obsidian robot costs (\\d+) ore and (\\d+) clay. Each geode robot costs (\\d+) ore and (\\d+) obsidian.")]
    private static partial Regex MyRegex();
}
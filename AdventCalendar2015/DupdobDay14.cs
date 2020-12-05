using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace AdventCalendar2015
{
    public class DupdobDay14
    {
        // 2072 too low
        // 2176 TOO LOW
        
        public void Parse(string input = Input)
        {
            var parser = new Regex("(\\w+) can fly (\\d+) km\\/s for (\\d+) seconds, but then must rest for (\\d+) seconds.");
            foreach (var line in input.Split('\n'))
            {
                var match = parser.Match(line);
                if (!match.Success)
                {
                    Console.WriteLine("Failed to parse {0}.", line);
                    continue;
                }
                var speed = new DeerSpeed();
                speed.Speed = int.Parse(match.Groups[2].Value);
                speed.RunTime = int.Parse(match.Groups[3].Value);
                speed.RestTime = int.Parse(match.Groups[4].Value);

                _speeds[match.Groups[1].Value] = speed;
            }
        }

        public object? Compute1()
        {
            var score = new Dictionary<string, int>();
            foreach (var name in _speeds.Keys)
            {
                score[name] = 0;
            }

            for (var i = 1; i <= 2503; i++)
            {
                var maxDist = int.MinValue;
                var leader = string.Empty;
                foreach (var entry in _speeds)
                {
                    var computeDistance = entry.Value.ComputeDistance(i);
                    if (computeDistance > maxDist)
                    {
                        maxDist = computeDistance;
                        leader = entry.Key;
                    }
                }

                score[leader]++;
            }

            return score.Values.Max();
        }

        public object? Compute2()
        {
            return _speeds.Values.Select(x => x.ComputeDistance(2503)).Max();
        }
        
        private struct DeerSpeed
        {
            public int Speed;
            public int RunTime;
            public int RestTime;

            public int ComputeDistance(int time)
            {
                // compute the distance performed with full run and rest cycles
                var remainingTime = time % (RunTime + RestTime);
                var distance = ((time-remainingTime)/ (RunTime + RestTime))*Speed*RunTime;
                // compute the rest.
                var min = Math.Min(remainingTime, RunTime);
                distance += min * Speed;
                return distance;
            }
        }
        
        private readonly Dictionary<string, DeerSpeed> _speeds = new Dictionary<string, DeerSpeed>();
        private const string Input = @"Vixen can fly 8 km/s for 8 seconds, but then must rest for 53 seconds.
Blitzen can fly 13 km/s for 4 seconds, but then must rest for 49 seconds.
Rudolph can fly 20 km/s for 7 seconds, but then must rest for 132 seconds.
Cupid can fly 12 km/s for 4 seconds, but then must rest for 43 seconds.
Donner can fly 9 km/s for 5 seconds, but then must rest for 38 seconds.
Dasher can fly 10 km/s for 4 seconds, but then must rest for 37 seconds.
Comet can fly 3 km/s for 37 seconds, but then must rest for 76 seconds.
Prancer can fly 9 km/s for 12 seconds, but then must rest for 97 seconds.
Dancer can fly 37 km/s for 1 seconds, but then must rest for 36 seconds.";
    }
}
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2015;

[Day(14)]
public class DupdobDay14: SolverWithIntParameter
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        
    }

    protected override void Parse(string data)
    {
        var parser = new Regex(@"(\w+) can fly (\d+) km\/s for (\d+) seconds, but then must rest for (\d+) seconds.", RegexOptions.Compiled);
        foreach (var line in data.SplitLines())
        {
            var match = parser.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse line: {0}", line);
                continue;
            }

            var name = match.Groups[1].Value;
            var speed = int.Parse(match.Groups[2].Value);
            var time = int.Parse(match.Groups[3].Value);
            var rest = int.Parse(match.Groups[4].Value);

            _reindeerData.Add(name, (speed, time, rest));
        }
    }
    
    private readonly Dictionary<string, (int speed, int time, int rest)> _reindeerData = new();

    [Example(1, @"
Comet can fly 14 km/s for 10 seconds, but then must rest for 127 seconds.
Dancer can fly 16 km/s for 11 seconds, but then must rest for 162 seconds.
", 1120, 1000)]
    protected override object GetAnswer1(int totalTime = 2503)
    {
        var maxDistance = 0;
        foreach (var (speed, time, rest) in _reindeerData.Values)
        {
            var cycleCount = totalTime / (time + rest);
            var remainingTime = totalTime % (time + rest);
            var distance = (cycleCount * time + Math.Min(remainingTime, time)) * speed;
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        return maxDistance;
    }

    [ReuseExample(1, 689)]
    protected override object GetAnswer2(int time = 2503)
    {
        var scores = new Dictionary<string, int>();
        foreach (var name in _reindeerData.Keys)
        {
            scores[name] = 0;
        }
        var deers = new Dictionary<string, (bool running, int time, int distance)>();
        var maxDist = -1;
        foreach (var (name, data) in _reindeerData)
        {
            deers[name] = (true, data.time, 0);
        }
        for (var t = 0; t < time; t++)
        {
            // deal with runningDeers
            var winners = new List<string>();
            foreach (var (name, data) in deers)
            {
                if (data.running)
                {
                    var newDist = data.distance + _reindeerData[name].speed;
                    deers[name] = (true, data.time - 1, newDist);
                    if (deers[name].time == 0)
                    {
                        deers[name] = (false, _reindeerData[name].rest, newDist);
                    }
                }
                else
                {
                    deers[name] = (false, data.time - 1, data.distance);
                    if (deers[name].time == 0)
                    {
                        deers[name] = (true, _reindeerData[name].time, data.distance);
                    }
                }
                if (deers[name].distance > maxDist)
                {
                    maxDist = deers[name].distance;
                    winners = [name];
                }
                else if (deers[name].distance == maxDist)
                {
                    winners.Add(name);
                }
            }

            foreach (var winner in winners)
            {
                scores[winner]++;
            }
        }

        return scores.Values.Max();
    }
}
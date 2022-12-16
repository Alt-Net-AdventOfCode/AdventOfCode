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

public partial class DupdobDay15 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 15;
        automaton.RegisterTestData(@"10
Sensor at x=2, y=18: closest beacon is at x=-2, y=15
Sensor at x=9, y=16: closest beacon is at x=10, y=16
Sensor at x=13, y=2: closest beacon is at x=15, y=3
Sensor at x=12, y=14: closest beacon is at x=10, y=16
Sensor at x=10, y=20: closest beacon is at x=10, y=16
Sensor at x=14, y=17: closest beacon is at x=10, y=16
Sensor at x=8, y=7: closest beacon is at x=2, y=10
Sensor at x=2, y=0: closest beacon is at x=2, y=10
Sensor at x=0, y=11: closest beacon is at x=2, y=10
Sensor at x=20, y=14: closest beacon is at x=25, y=17
Sensor at x=17, y=20: closest beacon is at x=21, y=22
Sensor at x=16, y=7: closest beacon is at x=15, y=3
Sensor at x=14, y=3: closest beacon is at x=15, y=3
Sensor at x=20, y=1: closest beacon is at x=15, y=3");
        automaton.RegisterTestResult(26);
        automaton.RegisterTestResult(56000011L, 2);
    }

    public override object GetAnswer1()
    {
        var referenceLine = _referenceLine;
        var segments = GetEmptySegmentForLine(referenceLine);
        // scan all intermediate points
        var score = segments.Sum(s=>LackOfBeacons(s.left, s.right, _referenceLine));
        return score;
    }

    private List<(int left, int right)> GetEmptySegmentForLine(int referenceLine)
    {
        var segments = new List<(int left, int right)>();
        for (var i = 0; i < _sensors.Count; i++)
        {
            var sensor = _sensors[i];
            var dist = _manhattanDistances[i] - Math.Abs(sensor.y - referenceLine);
            if (dist < 0)
            {
                // ths sensor won't detect any beacon
                continue;
            }

            (int left, int right) segment = (sensor.x - dist, sensor.x + dist);
            for (var j = 0; j < segments.Count; j++)
            {
                if (segments[j].left > segment.right || segments[j].right < segment.left) continue;
                // they intersect
                segment.left = Math.Min(segment.left, segments[j].left);
                segment.right = Math.Max(segment.right, segments[j].right);
                segments.RemoveAt(j);
                j = -1;
            }

            segments.Add(segment);
        }

        return segments;
    }

    private int LackOfBeacons(int left, int right, int segmentY)
    {
        return right-left+1-_beacons.Count(beacon => beacon.y == segmentY && beacon.x>=left && beacon.x<=right);
    }
    public override object GetAnswer2()
    {
        (int x, int y) distress = (-1, -1);
        var max = _referenceLine == 2000000 ? 4000000 : 20;
        for (var y = 0; y < max; y++)
        {
            var segments = GetEmptySegmentForLine(y);
            foreach (var segment in segments)
            {
                if (segment.left <= 0 &&  segment.right >= max )
                {
                    // no beacon here
                    continue;
                }
                // we have found the line
                distress.y = y;
                var right = 0;
                foreach (var sub in segments)
                {
                    if (sub.right < max && sub.right>=0)
                    {
                        right = Math.Max(right, sub.right + 1);
                    }
                }

                distress.x = right;
                break;
            }

            if (distress.y >= 0)
            {
                break;
            }
        }

        return distress.x * 4000000L + distress.y;
    }

    private readonly List<(int x, int y)> _sensors = new();
    private readonly HashSet<(int x, int y)> _beacons = new();
    private readonly List<int> _manhattanDistances = new();
    private int _referenceLine = 2000000;
    
    private readonly Regex _parser = MyRegex();
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _parser.Match(line);
        if (match.Success)
        {
            (int x, int y) sensor = (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            _sensors.Add(sensor);
            (int x, int y) beacon = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));
            _beacons.Add(beacon);
            _manhattanDistances.Add(Math.Abs(sensor.x-beacon.x)+Math.Abs(sensor.y - beacon.y));
        }
        else
        {
            if (int.TryParse(line, out var scannedLine))
            {
                _referenceLine = scannedLine;
            }
        }
    }

    [GeneratedRegex("Sensor at x=(-?\\d*), y=(-?\\d*): closest beacon is at x=(-?\\d*), y=(-?\\d*)")]
    private static partial Regex MyRegex();
}
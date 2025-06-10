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

[Day(9)]
public class DupdobDay09: SolverWithDataAsLines
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    [Example(1, """
             London to Dublin = 464
             London to Belfast = 518
             Dublin to Belfast = 141
             """, 605)]
    public override object GetAnswer1() => _distances.Keys.Min( start => MinTravel(0, start, _toVisit-start));

    private readonly Dictionary<(long, long), int> _shortestPast = [];
    private int MinTravel(int soFar, long current, long toVisit)
    {
        if (toVisit == 0)
        {
            return soFar;
        }  
        
        if (_shortestPast.TryGetValue((current, toVisit), out var minDistance))
        {
            return minDistance + soFar;
        }

        minDistance = int.MaxValue;
        foreach (var (to, distance) in _distances[current])
        {
            if ((toVisit & to) == 0)
            {
                continue;
            }

            minDistance = Math.Min(minDistance, MinTravel(distance, to, toVisit - to));
        }

        _shortestPast[(current, toVisit)] = minDistance;
        return minDistance + soFar;
    }
    
    private readonly Dictionary<(long, long), int> _longestPast = [];
    
    private int MaxTravel(int soFar, long current, long toVisit)
    {
        if (toVisit == 0)
        {
            return soFar;
        }  
        
        if (_longestPast.TryGetValue((current, toVisit), out var maxDistance))
        {
            return maxDistance + soFar;
        }

        maxDistance = int.MinValue;
        foreach (var (to, distance) in _distances[current])
        {
            if ((toVisit & to) == 0)
            {
                continue;
            }

            maxDistance = Math.Max(maxDistance, MaxTravel(distance, to, toVisit - to));
        }

        _longestPast[(current, toVisit)] = maxDistance;
        return maxDistance + soFar;
    }

    [ReuseExample(1, 982)]
    public override object GetAnswer2() => _distances.Keys.Max( start => MaxTravel(0, start, _toVisit-start));

    private readonly Regex _parser = new(@"(\w+) to (\w+) = (\d+)", RegexOptions.Compiled);
    private readonly Dictionary<long, Dictionary<long, int>> _distances = [];
    private readonly Dictionary<string, long> _encoding = [];
    private long _toVisit;

    protected override void ParseLines(string[] lines)
    {
        foreach (var line in lines)
        {
            var match = _parser.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse {0}.", line);
                continue;
            }

            var from = EncodeCity(match.Groups[1].Value);
            var distance = int.Parse(match.Groups[3].Value);
            var to = EncodeCity(match.Groups[2].Value);
            var distances = _distances.GetValueOrDefault(from) ?? [];
            distances[to] = distance;
            _distances[from] = distances;
            distances = _distances.GetValueOrDefault(to) ?? [];
            distances[from] = distance;
            _distances[to] = distances;
        }
        _toVisit = _distances.Keys.Aggregate(0L, (seed, next) => next|seed);
    }

    private long EncodeCity(string city)
    {
        if (_encoding.TryGetValue(city, out var code))
        {
            return code;
        }

        if (_encoding.Count == 64)
        {
            throw new ApplicationException("More than 64 destinations");
        }
        return _encoding[city] = 1 << _encoding.Count;
    }
}
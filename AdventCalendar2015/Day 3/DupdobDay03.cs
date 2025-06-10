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

using System.Collections.Generic;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(3)]
public class DupdobDay03 : SolverWithParser
{
    private const string Directions = ">v<^";
    private readonly (int dx, int dy)[] _vectors = [(1, 0), (0, 1), (-1, 0), (0, -1)];
    private string _moves;

    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    protected override void Parse(string data) => _moves = data;

    [Example("^>v<", 4)]
    public override object GetAnswer1()
    {
        var hits = new Dictionary<(int x, int y), int>();
        
        (int x, int y) start = (0, 0);
        hits[start] = 1;
        foreach (var vector in _moves.Select(move => _vectors[Directions.IndexOf(move)]))
        {
            start = (start.x + vector.dx, start.y + vector.dy);
            hits[start] = 1 + hits.GetValueOrDefault(start);
        }
        
        return hits.Count;
    }

    [Example("^>v<", 3)]
    public override object GetAnswer2()
    {
        var hits = new Dictionary<(int x, int y), int>();
        
        (int x, int y) startSanta = (0, 0);
        (int x, int y) startRobot = startSanta;
        hits[startSanta] = 1;
        hits[startRobot] = 1;
        foreach (var vector in _moves.Select(move => _vectors[Directions.IndexOf(move)]))
        {
            startSanta = (startSanta.x + vector.dx, startSanta.y + vector.dy);
            hits[startSanta] = 1 + hits.GetValueOrDefault(startSanta);
            (startSanta, startRobot) = (startRobot, startSanta);
        }
        
        return hits.Count;
    }
}
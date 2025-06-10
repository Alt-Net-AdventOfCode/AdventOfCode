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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay4 : SolverWithLineParser
{
    private readonly List<(int begin, int end)> _firstElves = new();
    private readonly List<(int begin, int end)> _secondElves = new();
    
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 4;
        dayAutomaton.RegisterTestDataAndResult(@"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8", 2, 1);        
        dayAutomaton.RegisterTestDataAndResult(@"2-4,6-8
2-3,4-5
5-7,7-9
2-8,3-7
6-6,4-6
2-6,4-8", 4, 2);
    }

    public override object GetAnswer1()
    {
        var count = 0;
        for (var i = 0; i <_firstElves.Count; i++)
        {
            if (_firstElves[i].begin >= _secondElves[i].begin && _firstElves[i].end <= _secondElves[i].end)
            {
                count++;
            }
            else if (_firstElves[i].begin <= _secondElves[i].begin && _firstElves[i].end >= _secondElves[i].end)
            {
                count++;
            }
        }

        return count;
    }

    public override object GetAnswer2()
    {
        var count = 0;
        for (var i = 0; i <_firstElves.Count; i++)
        {
            if (_firstElves[i].begin <= _secondElves[i].end && _secondElves[i].begin <= _firstElves[i].end)
            {
                count++;
            }
        }

        return count;
        
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        var blocs = line.Split(',');
        var ends = blocs[0].Split('-').Select(int.Parse).ToList();
        _firstElves.Add((ends[0], ends[1]));
        ends = blocs[1].Split('-').Select(int.Parse).ToList();
        _secondElves.Add((ends[0], ends[1]));
    }
}
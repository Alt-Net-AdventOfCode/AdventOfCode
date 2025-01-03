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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay05 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 5;
        automatonBase.RegisterTestDataAndResult("""
                                                0
                                                3
                                                0
                                                1
                                                -3
                                                """, 5, 1).RegisterTestResult(10, 2);
    }

    public override object GetAnswer1()
    {
        var maze = _steps.ToList();
        var steps = 0;
        for (var i = 0; i < maze.Count; steps++)
        {
            var current = i;
            i += maze[i];
            maze[current]++;
        }

        return steps;
    }

    public override object GetAnswer2()
    {
        var maze = _steps.ToList();
        var steps = 0;
        for (var i = 0; i < maze.Count; steps++)
        {
            var current = i;
            i += maze[i];
            if (maze[current] >= 3)
            {
                maze[current]--;
            }
            else
            {
                maze[current]++;
            }
        }

        return steps;
    }

    private readonly List<int> _steps = [];
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _steps.Add(int.Parse(line));
    }
}
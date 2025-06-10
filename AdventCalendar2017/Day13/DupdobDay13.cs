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

public class DupdobDay13 : SolverWithDataAsLines
{
    private readonly List<(int offset, int length)> _scanners = [];

    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 13;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                0: 3
                                                1: 2
                                                4: 4
                                                6: 4
                                                """, 24, 1).RegisterTestResult(10,2);
    }

    public override object GetAnswer1() => _scanners.Sum(p => p.offset % ((p.length-1)*2) == 0 ? p.offset * p.length : 0);

    public override object GetAnswer2()
    {
        var delay = 1L;
        while (delay < 100000000)
        {
            if (_scanners.All(p => (p.offset+delay) % ((p.length - 1) * 2) != 0))
            {
                // we pass
                return delay;
            }
            // we will get caught, wait 1 ps
            delay++;
        }

        return 0;
    }

    protected override void ParseLines(string[] lines)
    {
        foreach (var line in lines)
        {
            var parts = line.Split(':', StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
            _scanners.Add((parts[0], parts[1]));
        }
    }
}
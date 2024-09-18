// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2023;

public class DupdobDay6 : SolverWithLineParser
{
    private List<long> _times = new();
    private List<long> _distances = new();
    private long _time;
    private long _distance;
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 6;
        automatonBase.RegisterTestDataAndResult(@"Time:      7  15   30
Distance:  9  40  200", 288, 1);
        automatonBase.RegisterTestResult(71503, 2);
    }

    public override object GetAnswer1()
    {
        var result = 1L;
        for (var i = 0; i < _times.Count; i++)
        {
            var distance = _distances[i];
            var time = _times[i];
            result *= NumberOfPossibilities(distance, time);
        }

        return result;
    }

    private static long NumberOfPossibilities(long distance, long time)
    {
        var minSpeed = distance / (time - 1);
        while ((time - minSpeed) * minSpeed <= distance)
        {
            minSpeed++;
        }

        var maxSpeed = time - 1;
        var a = minSpeed;
        var b = maxSpeed;
        for (;;)
        {
            var attempt = (time - maxSpeed) * maxSpeed;
            if (attempt <= distance)
            {
                if (Math.Abs(b - a) == 1)
                {
                    maxSpeed = a;
                    break;
                }

                b = maxSpeed;
                maxSpeed = (a + b) / 2;
            }
            else
            {
                if (Math.Abs(b - a) == 1)
                {
                    if (attempt <= distance)
                    {
                        maxSpeed = a;
                    }

                    break;
                }

                a = maxSpeed;
                maxSpeed = (a + b) / 2;
            }
        }

        return (maxSpeed - minSpeed + 1);
    }

    public override object GetAnswer2()
    {
        return NumberOfPossibilities(_distance, _time);
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (line.StartsWith("Time"))
        {
            _times = line.Split(':')[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse)
                .ToList();
            _time = long.Parse(line.Split(':')[1].Replace(" ", null));
        } else if (line.StartsWith("Distance"))
        {
            _distances = line.Split(':')[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse)
                .ToList();
            _distance = long.Parse(line.Split(':')[1].Replace(" ", null));
        }
    }
}
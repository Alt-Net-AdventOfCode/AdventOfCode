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

public class DupdobDay11 : SolverWithDataAsLines
{
    private string[]? _steps;

    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 11;
        dayAutomatonBase.RegisterTestDataAndResult("ne,ne,ne", 3, 1);
        dayAutomatonBase.RegisterTestDataAndResult("ne,ne,sw,sw", 0, 1);
        dayAutomatonBase.RegisterTestDataAndResult("ne,ne,s,s", 2, 1);
        dayAutomatonBase.RegisterTestDataAndResult("se,sw,se,sw,sw", 3, 1);
    }

    private int _maxDistance;

    public override object GetAnswer1()
    {
        if (_steps == null)
        {
            return null;
        }
        
        Dictionary<string, (int dx, int dy)> vectors = [];
        vectors["sw"] = (1, 1);
        vectors["s"] = (0, 2);
        vectors["se"] = (-1, 1);
        vectors["ne"] = (-1, -1);
        vectors["n"] = (0, -2);
        vectors["nw"] = (1, -1);
        (int x, int y) pos = (0, 0);
        _maxDistance = 0;
        var distance = 0;
        foreach (var s in _steps)
        {
            pos = (pos.x + vectors[s].dx, pos.y + vectors[s].dy);
            distance = (Math.Abs(pos.x) + Math.Abs(pos.y)) / 2;
            _maxDistance = Math.Max(_maxDistance, distance);
        }

        return distance;
    }

    public override object GetAnswer2()
    {
        return _maxDistance;
    }

    protected override void ParseLines(string[] lines)
    {
        _steps = lines[0].Split(',', StringSplitOptions.TrimEntries).ToArray();
    }
}
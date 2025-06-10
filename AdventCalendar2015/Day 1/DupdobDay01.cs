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

using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(1)]
public class DupdobDay01: SolverWithParser 
{
    private int _depth;
    private int _basementStep = 0;

    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    protected override void Parse(string data)
    {
        _depth = 0;
        for (var i = 0; i < data.Length; i++)
        {
            _depth += data[i] == ')' ? -1 : 1;
            if (_depth == -1 && _basementStep == 0)
            {
                _basementStep = i+1;
            }
        }
    }

    [Example("(())", 0)]
    [Example("(()(()(", 3)]
    public override object GetAnswer1() => _depth;

    public override object GetAnswer2() => _basementStep;
}
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

public class DupdobDay01 : SolverWithLineParser
{
    private string _line = null!;

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 1;
        automatonBase.RegisterTestDataAndResult("1122", 3, 1);
        automatonBase.RegisterTestDataAndResult("91212129", 9, 1);
        automatonBase.RegisterTestDataAndResult("1212", 6, 2);
        automatonBase.RegisterTestDataAndResult("12131415", 4, 2);
    }

    public override object GetAnswer1() => _line.Where((t, i) => t == _line[(i + 1) % _line.Length]).Aggregate(0L, (current, t) => current + (t - '0'));

    public override object GetAnswer2() => _line.Where((t, i) => t == _line[(i + _line.Length/2) % _line.Length]).Aggregate(0L, (current, t) => current + (t - '0'));
    
    protected override void ParseLine(string line, int index, int lineCount) => _line = line;
}
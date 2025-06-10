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

public class DupdobDay09 : SolverWithDataAsLines
{
    private string _line = null!;
    private int _garbageLength;

    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 9;
        dayAutomatonBase.RegisterTestDataAndResult("{{{}}}", 6, 1);
        dayAutomatonBase.RegisterTestDataAndResult("{{<!!>},{<!!>},{<!!>},{<!!>}}", 9, 1);
        dayAutomatonBase.RegisterTestDataAndResult("{{{},{},{{}}}}", 16, 1);
        dayAutomatonBase.RegisterTestDataAndResult("{<{},{},{{}}>}", 1, 1);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        var inGarbage = false;
        var groupDepth = 0;
        _garbageLength = 0;
        for (var i=0; i<_line.Length; i++)
        {
            var symbol = _line[i];
            if (symbol == '!')
            {
                i++;
                continue;
            }
            if (inGarbage)
            {
                if (symbol == '>')
                {
                    inGarbage = false;
                }
                else
                {
                    _garbageLength++;
                }
            }
            else
            {
                switch (symbol)
                {
                    case '<':
                        inGarbage = true;
                        break;
                    case '{':
                        groupDepth++;
                        break;
                    case '}':
                        if (groupDepth > 0)
                        {
                            result += groupDepth;
                            groupDepth--;
                        }
                        break;
                }
            }
        }

        return result;
    }

    public override object GetAnswer2()
    {
        return _garbageLength;
    }

    protected override void ParseLines(string[] lines)
    {
        _line = lines[0];
    }
}
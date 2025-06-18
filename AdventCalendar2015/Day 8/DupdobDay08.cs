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

[Day(8)]
public class DupdobDay08: SolverWithParser
{   
    private string[] _lines;
    

    [Example(1, """
             ""
             "abc"
             "aaa\"aaa"
             "\x27"
             """, 12)]
    public override object GetAnswer1() => _lines.Sum(CalcDiff);

    private static int CalcDiff(string arg) => arg.Length - CountLetter(arg);

    private static int CountLetter(string arg)
    {
        var countLetter = 0;
        for (var i = 1; i < arg.Length - 1; i++)
        {
            if (arg[i] == '\\')
            {
                countLetter++;
                if (arg[i + 1] == 'x')
                {
                    i += 3;
                }
                else
                {
                    // skip the nextone
                    i++;
                }
            }
            else
            {
                countLetter++;
            }
        }

        return countLetter;
    }

    private static int CalcDiff2(string arg) => arg.Length + 2 + arg.Count(t => t is '\\' or '"') - arg.Length;

    [ReuseExample(1, 19)]
    public override object GetAnswer2() => _lines.Sum(CalcDiff2);

    protected override void Parse(string data) => _lines = data.SplitLines();
}
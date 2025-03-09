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

using System.Text;
using AoC;

namespace AdventCalendar2015;

[Day(10)]
public class DupdobDay10: SolverWithParser
{
    private string _input;
    public override void SetupRun(Automaton _)
    {
    }

    protected override void Parse(string data)
    {
        _input = data;
    }

    public override object GetAnswer1()
    {
        var temp = _input;
        for (var i = 0; i < 40; i++)
        {
            temp = Transform(temp);
        }

        return temp.Length;
    }

    [UnitTest("11", "1")]
    [UnitTest("111221", "1211")]
    private static string Transform(string input)
    {
        var builder = new StringBuilder(input.Length * 2);
        var lastChar = input[0];
        var count = 1;
        for (var i = 1; i < input.Length; i++)
        {
            if (lastChar == input[i])
            {
                count++;
            }
            else
            {
                builder.Append(count).Append(lastChar);
                lastChar = input[i];
                count = 1;
            }
        }

        builder.Append(count).Append(lastChar);
        return builder.ToString();
    }
    
    public override object GetAnswer2()
    {
        var temp = _input;
        for (var i = 0; i < 50; i++)
        {
            temp = Transform(temp);
        }

        return temp.Length;
    }
}
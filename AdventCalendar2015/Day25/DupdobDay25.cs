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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2015;

[Day(25)]
public partial class DupdobDay25: SolverWithParser
{
    private int _col;
    private int _row;

    protected override void Parse(string data)
    {
        var match = MyRegex();
        var m = match.Match(data);
        if (!m.Success)
        {
            throw new System.Exception("Invalid input format");
        }
        _row = int.Parse(m.Groups[1].Value);
        _col = int.Parse(m.Groups[2].Value);
    }

    [Example(1, "To continue, please consult the code grid in the manual.  Enter the code at row 2, column 1.", 31916031)]
    [Example(2, "To continue, please consult the code grid in the manual.  Enter the code at row 6, column 6.", 27995004)]
    public override object GetAnswer1()
    {
        // convert row and column to a sequential position
        var start = _row + _col - 1;
        var index = (start * (start - 1)) / 2 + _col;
        long seed = 20151125L;
        for (var i = 1; i < index; i++)
        {
            seed = (seed * 252533) % 33554393;
        }

        return seed;
    }

    public override object GetAnswer2()
    {
        return null;
    }

    [GeneratedRegex(@"To continue, please consult the code grid in the manual.  Enter the code at row (\d+), column (\d+).")]
    private static partial Regex MyRegex();
}
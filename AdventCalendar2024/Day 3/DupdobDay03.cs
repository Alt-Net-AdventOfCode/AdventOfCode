// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2024;

public partial class DupdobDay03 : SolverWithParser
{
    private string _formula = string.Empty;
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 3;
        automatonBase.RegisterTestDataAndResult(@"xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))", 161, 1);
        automatonBase.RegisterTestDataAndResult(@"xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))",48, 2);
    }

    public override object GetAnswer1() => Compute(_formula);

    private static long Compute(string formula)
    {
        var mulMatch = MyRegex();
        var mul = mulMatch.Matches(formula);
        var result = 0L;
        foreach (Match match in mul)
        {
            result += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
        }
        return result;
    }

    public override object GetAnswer2()
    {
        var correctedFormula=string.Empty;
        // remove don't part
        var index = 0;
        for(;;)
        {
            var current = index;
            index = _formula.IndexOf("don't()", index, StringComparison.InvariantCulture);
            if (index >= 0)
            {
                correctedFormula += _formula.Substring(current, index-current);
                index += 5;
                // need to find a 'do'
                index = _formula.IndexOf("do()", index, StringComparison.InvariantCulture);
                if (index >= 0)
                {
                    index += 3;
                }
            }
            else
            {
                correctedFormula += _formula[current..];
                break;
            }
        }
        return Compute(correctedFormula);
    }

    protected override void Parse(string data)
    {
        _formula = data;
    }

    [GeneratedRegex(@"mul\((\d*),(\d*)\)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
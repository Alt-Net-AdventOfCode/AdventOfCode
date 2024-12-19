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

using AoC;

namespace AdventCalendar2024;

public class DupdobDay19 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 19;
        automatonBase.RegisterTestDataAndResult("""
                                                r, wr, b, g, bwu, rb, gb, br
                                                
                                                brwrr
                                                bggr
                                                gbbr
                                                rrbgbr
                                                ubwu
                                                bwurrg
                                                brgr
                                                bbrgwb
                                                """, 6, 1);
        automatonBase.RegisterTestResult(16, 2);
    }

    public override object GetAnswer1() => _patterns.Count(IsPossible);

    private bool IsPossible(string pattern) => string.IsNullOrEmpty(pattern) || _towels.Where(pattern.StartsWith).Any(towel => IsPossible(pattern[towel.Length..]));

    public override object GetAnswer2() => _patterns.Sum(CountVariants);


    private readonly Dictionary<string, long> _countOfVariants = []; 
    private long CountVariants(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return 1L;
        }

        if (_countOfVariants.TryGetValue(pattern, out var count))
        {
            return count;
        }

        return _countOfVariants[pattern] = _towels.Sum(t => pattern.StartsWith(t) ? CountVariants(pattern[t.Length ..]) : 0);
    }

    private List<string> _towels = null!;
    private readonly List<string> _patterns = [];
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (index == 0)
        {
            _towels = line.Split(',', StringSplitOptions.TrimEntries).ToList();
        }
        else if (!string.IsNullOrEmpty(line))
        {
            _patterns.Add(line);
        }
    }
}
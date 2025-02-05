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

public class DupdobDay02 : SolverWithBlockParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 2;
        automatonBase.RegisterTestDataAndResult("""
                                                5 1 9 5
                                                7 5 3
                                                2 4 6 8
                                                """, 18, 1);
        automatonBase.RegisterTestDataAndResult("""
                                                5 9 2 8
                                                9 4 7 3
                                                3 8 6 5
                                                """, 9, 2);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        foreach (var line in _matrix)
        {
            result += line.Max() - line.Min();
        }

        return result;
    }

    public override object GetAnswer2()
    {
        var result = 0;
        foreach (var line in _matrix)
        {
            for (var i = 0; i < line.Count-1; i++)
            {
                for (var j = i + 1; j < line.Count; j++)
                {
                    if (line[i] % line[j] == 0)
                    {
                        result += line[i] / line[j];
                        break;
                    }

                    if (line[j] % line[i] == 0)
                    {
                        result += line[j] / line[i];
                    }
                }
            }
        }

        return result;
    }

    private readonly List<List<int>> _matrix = [];
    protected override void ParseBlock(List<string> block, int blockIndex)
    {
        foreach (var line in block)
        {
            var separator = ' ';
            if (line.Contains('\t'))
            {
                separator = '\t';   
            }
            _matrix.Add(line.Split(separator, StringSplitOptions.TrimEntries).Select(int.Parse).ToList());
        }
    }
}
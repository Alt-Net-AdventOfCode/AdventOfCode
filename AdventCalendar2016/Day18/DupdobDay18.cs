// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
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
using System.Text;
using AoC;

namespace AdventCalendar2016;

public class DupdobDay18 : SolverWithLineParser
{
    private int _rowCount;
    private string _firstLine;

    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 18;
        automaton.AddExample(".^^.^.^^^^");
        automaton.RegisterTestResult(38);
    }   

    public override object GetAnswer1() => CountEmptyCells(_rowCount);

    private object CountEmptyCells(int rowCount)
    {
        var line = "." + _firstLine + ".";
        var nextLine = new StringBuilder(line.Length);
        var emptyCells = _firstLine.Count(c => c == '.');
        for (var i = 0; i < rowCount - 1; i++)
        {
            nextLine.Append('.');
            for (var j = 1; j < line.Length - 1; j++)
            {
                if ((line[j - 1] == line[j] && line[j] != line[j + 1]) ||
                    (line[j - 1] != line[j] && line[j] == line[j + 1]))
                {
                    nextLine.Append('^');
                }
                else
                {
                    nextLine.Append('.');
                    emptyCells++;
                }
            }

            nextLine.Append('.');
            line = nextLine.ToString();
            nextLine.Clear();
        }

        return emptyCells;
    }

    public override object GetAnswer2() => CountEmptyCells(400000);

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        _rowCount = line == ".^^.^.^^^^" ? 10 : 40;

        _firstLine = line;
    }
}
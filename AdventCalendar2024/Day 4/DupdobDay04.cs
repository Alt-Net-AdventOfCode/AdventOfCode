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

using System.Text;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay04 : SolverWithLineParser
{
    private readonly List<string> _letters = [];
    
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 4;
        dayAutomatonBase.RegisterTestDataAndResult(@"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX", 18, 1);
        dayAutomatonBase.RegisterTestResult(9, 2);
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _letters.Add(line);
    }
    
    public override object GetAnswer1()
    {
        var result = 0;
        var width = _letters[0].Length;
        var height = _letters.Count;

        // horizontal
        foreach (var line in _letters)
        {
            result += CountXmas(line);
            result += CountXmas(Reverse(line));
        }
        // vertical
        for (var i = 0; i < width; i++)
        {
            var lineBuilder = new StringBuilder(height);
            foreach (var row in _letters)
            {
                lineBuilder.Append(row[i]);
            }
            var line = lineBuilder.ToString();
            result += CountXmas(line);
            result += CountXmas(Reverse(line));
        }
        // diagonals
        for (var i = 3; i < width; i++)
        {
            var lineBuilder = new StringBuilder(height);
            for (var j = 0; j <= i; j++)
            {
                lineBuilder.Append(_letters[i-j][j]);
            }
            var line = lineBuilder.ToString();
            result += CountXmas(line);
            result += CountXmas(Reverse(line));
        }
        for (var i = 3; i < width-1; i++)
        {
            var lineBuilder = new StringBuilder(height);
            for (var j = 0; j <= i; j++)
            {
                lineBuilder.Append(_letters[height-1-j][width-1-i+j]);
            }
            var line = lineBuilder.ToString();
            result += CountXmas(line);
            result += CountXmas(Reverse(line));
        }

        for (var i = 3; i < width; i++)
        {
            var lineBuilder = new StringBuilder(height);
            for (var j = 0; j <= i; j++)
            {
                lineBuilder.Append(_letters[i-j][width-1-j]);
            }
            var line = lineBuilder.ToString();
            result += CountXmas(line);
            result += CountXmas(Reverse(line));
        }
        for (var i = 3; i < width-1; i++)
        {
            var lineBuilder = new StringBuilder(height);
            for (var j = 0; j <= i; j++)
            {
                lineBuilder.Append(_letters[height-1-i+j][j]);
            }
            var line = lineBuilder.ToString();
            result += CountXmas(line);
            result += CountXmas(Reverse(line));
        }
        
        return result;
    }   

    private string Reverse(string line)
    {
        return new string(line.Reverse().ToArray());
    }

    private static int CountXmas(string line)
    {
        var index = 0;
        var result = 0;
        do
        {
            index = line.IndexOf("XMAS", index, StringComparison.Ordinal);
            if (index < 0) continue;
            index += 4;
            result++;
        } while (index>=0);

        return result;
    }

    public override object GetAnswer2()
    {
        var result = 0;
        var width = _letters[0].Length;
        var height = _letters.Count;
        for (var i = 1; i < height - 1; i++)
        {
            for (var j = 1; j < width - 1; j++)
            {
                if (_letters[i][j] != 'A')
                    continue;
                if ((_letters[i - 1][j - 1] == 'M' && _letters[i + 1][j + 1] == 'S' ||
                     _letters[i - 1][j - 1] == 'S' && _letters[i + 1][j + 1] == 'M')
                    && (_letters[i - 1][j + 1] == 'M' && _letters[i + 1][j - 1] == 'S' ||
                        _letters[i - 1][j + 1] == 'S' && _letters[i + 1][j - 1] == 'M'))
                    result++;
            }
        }
        return result;
    }
    
}
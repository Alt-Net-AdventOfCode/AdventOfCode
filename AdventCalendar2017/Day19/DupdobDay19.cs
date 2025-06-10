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

namespace AdventCalendar2017;

public class DupdobDay19: SolverWithDataAsLines
{
    private string[] _lines= [];
    private int _width;
    private int _height;

    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 19;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                    |          
                                                    |  +--+    
                                                    A  |  C    
                                                F---|----E|--+ 
                                                    |  |  |  D 
                                                    +B-+  +--+ 

                                                """, "ABCDEF", 1).RegisterTestResult(38,2);
    }

    private record Vector(int Dy, int Dx);

    private readonly Vector[] _vectors = [new(1, 0), new(0, -1), new(-1, 0), new(0, 1)];
    private int _path;

    public override object GetAnswer1()
    {
        var direction = 0;
        var (y, x) = (0, _lines[0].IndexOf('|'));
        var path = new StringBuilder();
        _path = 0;
        for (;;)
        {
            (y, x) = (y + _vectors[direction].Dy, x + _vectors[direction].Dx);
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                _path++;
                break;
            }
            var nextCell = _lines[y][x];
            _path++;
            if (nextCell is >= 'A' and <= 'Z')
            {
                path.Append(nextCell);
            }
            else if (nextCell == '+')
            {
                if (direction % 2 == 0)
                {
                    if (x > 0 && IsHorizontalPath(_lines[y][x - 1]))
                    {
                        // we must turn to the left
                        direction = 1;
                    }
                    else if (x < _width - 1 && IsHorizontalPath(_lines[y][x + 1]))
                    {
                        direction = 3;
                    }
                }
                else
                {
                    if (y > 0 && IsVerticalPath(_lines[y - 1][x]))
                    {
                        direction = 2;
                    }
                    else if (y < _height - 1 && IsVerticalPath(_lines[y + 1][x]))
                    {
                        direction = 0;
                    }
                }
            }
            else if (nextCell == ' ')
            {
                break;
            }
        }

        return path.ToString();

        bool IsHorizontalPath(char car) => car is '-' or >= 'A' and <= 'Z';
        bool IsVerticalPath(char car) => car is '|' or >= 'A' and <= 'Z';
    }

    public override object GetAnswer2()
    {
        return _path;
    }

    protected override void ParseLines(string[] lines)
    {
        _lines = lines;
        _width = lines[0].Length;
        _height = lines.Length;
    }
}
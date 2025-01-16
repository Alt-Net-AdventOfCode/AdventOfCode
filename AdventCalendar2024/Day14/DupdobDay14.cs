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

public partial class DupdobDay14 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 14;
        automatonBase.AddExample("""
                                                p=0,4 v=3,-3
                                                p=6,3 v=-1,-3
                                                p=10,3 v=-1,2
                                                p=2,0 v=2,-1
                                                p=0,0 v=1,3
                                                p=3,0 v=-2,-2
                                                p=7,6 v=-1,-3
                                                p=3,0 v=-1,-2
                                                p=9,3 v=2,3
                                                p=7,3 v=-1,2
                                                p=2,4 v=2,-3
                                                p=9,5 v=-3,-3
                                                """).WithParameters(7,11);
        automatonBase.RegisterTestResult(12);
    }

    public override object GetAnswer1()
    {
        if (ExtraParameters.Length != 0)
        {
            _width = ExtraParameters[0];
            _height = ExtraParameters[1];
        }
        var quadrants = new int[4];
        foreach (var guard in _guards)
        {
            var x= ((guard.X+100*guard.Dx) % _width + _width)%_width;
            var y = ((guard.Y+100*guard.Dy) % _height + _height)%_height;
            var quadrant = 0;
            if (x>(_width-1)/2)
            {
                quadrant += 2;
            }
            else if (x == (_width-1)/2)
            {
                continue;
            }

            if (y > (_height - 1) / 2)
            {
                quadrant += 1;
            }
            else if (y == (_height - 1) / 2)
            {
                continue;
            }
            quadrants[quadrant]++;
        }
        return quadrants[0]*quadrants[1]*quadrants[2]*quadrants[3];
    }

    public override object GetAnswer2()
    {
        var turn = 0;
        var currentGuards = _guards;
        while (true)
        {
            var nextGuard = new List<Guard>(currentGuards.Count);
            var pixels = new HashSet<(int x, int y)>(nextGuard.Count);
            var unique = true;
            foreach (var guard in currentGuards)
            {
                var x= ((guard.X + guard.Dx) % _width + _width)%_width;
                var y = ((guard.Y + guard.Dy) % _height + _height)%_height;

                nextGuard.Add(guard with { X = x, Y = y });
                unique &= pixels.Add((x, y));
            }
            currentGuards = nextGuard;
            turn++;        
            if (unique)
            {
                Console.Clear();
                for (var y = 0; y < _height; y++)
                {
                    for (var x = 0; x < _width; x++)
                    {
                        Console.Write(pixels.Contains((x, y)) ? '#' : '.');
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
                return turn;
            }
                
        }
    }

    private readonly Regex _parser = MyRegex();

    private record Guard(int X, int Y, int Dx, int Dy);
    private readonly List<Guard> _guards = [];
    private int _width = 101;
    private int _height = 103;
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        
        var match = _parser.Match(line);
        if (!match.Success)
        {
            Console.WriteLine("Failed to parse {0}, line)");
            return;
        }
        _guards.Add(new Guard(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value),
            int.Parse(match.Groups[4].Value)));
    }

    [GeneratedRegex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
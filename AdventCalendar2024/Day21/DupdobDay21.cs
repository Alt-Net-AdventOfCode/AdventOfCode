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

public class DupdobDay21 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 21;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                029A
                                                980A
                                                179A
                                                456A
                                                379A
                                                """, 126384, 1);
        dayAutomatonBase.RegisterTestResult(248566068436630, 2);
    }

    public override object GetAnswer1()
    {
        var score = 0;
        foreach (var code in _codes)
        {
            var third = GetShortestPastToCode(GetShortestPastToCode(GetShortestPastToCode(code, _numericKeypad), _directionalKeypad), _directionalKeypad);
            score += ExtractNumber(code) * third.Length;
        }

        return score;
    }

    private int ExtractNumber(string code)
    {
        var result = 0;
        foreach (var digit in code)
        {
            if (digit is >= '0' and <= '9')
            {
                result = result * 10 + digit - '0';
            }
        }

        return result;
    }

    private record Position(int Y, int X);

    private readonly Dictionary<char, Position> _numericKeypad =new()
    {
        ['7'] = new Position(3,0), ['8'] = new Position(3,1), ['9'] = new Position(3,2),
        ['4'] = new Position(2,0), ['5'] = new Position(2,1), ['6'] = new Position(2,2),  
        ['1'] = new Position(1,0), ['2'] = new Position(1,1), ['3'] = new Position(1,2),
        [' '] = new Position(0,0), ['0'] = new Position(0,1), ['A'] = new Position(0, 2),
    };

    private readonly Dictionary<char, Position> _directionalKeypad =new()
    {
        [' '] = new Position(1,0), ['^'] = new Position(1, 1), ['A'] = new Position(1, 2),
        ['<'] = new Position(0,0), ['v'] = new Position(0,1), ['>'] = new Position(0,2),
    };
    
    private static string GetShortestPastToCode(string code, Dictionary<char, Position> keypad) => string.Join(null,GetShortestSteps(code, keypad));

    private static List<string> GetShortestSteps(string code, Dictionary<char, Position> keypad)
    {
        {
            var current = keypad['A'];
            // forbidden position
            var forbidden = keypad[' '];
            var steps = new List<string>();
            foreach (var nextPosition in code.Select(nextDigit => keypad[nextDigit]))
            {
                var resultingCode = new StringBuilder(6);
                if (nextPosition != current)
                {
                    if (nextPosition.X == forbidden.X && current.Y == forbidden.Y 
                        || (nextPosition.X > current.X && (current.X!=forbidden.X || nextPosition.Y!= forbidden.Y )))
                    {
                        var diff = nextPosition.Y - current.Y;
                        resultingCode.Append(diff > 0 ? '^' : 'v', Math.Abs(diff));
                        diff = nextPosition.X - current.X;
                        resultingCode.Append(diff > 0 ? '>' : '<', Math.Abs(diff));
                    }
                    else
                    {
                        var diff = nextPosition.X - current.X;
                        resultingCode.Append(diff > 0 ? '>' : '<', Math.Abs(diff));
                        diff = nextPosition.Y - current.Y;
                        resultingCode.Append(diff > 0 ? '^' : 'v', Math.Abs(diff));
                    }
                    current = nextPosition;
                }

                resultingCode.Append('A');
                steps.Add(resultingCode.ToString());
            }

            return steps;
        }
    }

    public override object GetAnswer2()
    {
        var score = 0L;

        foreach (var problem in _codes)
        {
            var next = GetShortestPastToCode(problem, _numericKeypad);
            var moves = new Dictionary<string, long>{ [next] = 1 };
            
            // transform it 25 times
            for (var i = 0; i < 25; i++)
            {
                {
                    var nextPaths = new Dictionary<string, long>();
                    foreach (var (path, count) in moves)
                    {
                        var steps = GetShortestSteps(path, _directionalKeypad);
                        foreach (var step in steps)
                        {
                            nextPaths[step] = nextPaths.GetValueOrDefault(step) + count;
                        }
                    }

                    moves = nextPaths;
                }
            }

            var entree = ExtractNumber(problem);
            var weight = moves.Sum(kv => kv.Key.Length*kv.Value);
            score += entree * weight;
        }
        return score;
    }
    
    private readonly List<string> _codes = [];
    
    protected override void ParseLine(string line, int index, int lineCount) => _codes.Add(line);
}
// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC;
using AoCAlgorithms;

namespace AdventCalendar2016;

public partial class DupdobDay21 : SolverWithLineParser
{
    private string _initialCode;
    private readonly Regex _positionSwap = MyRegex();
    private readonly Regex _letterSwap = MyRegex1();
    private readonly Regex _rotationLeft = MyRegex2();
    private readonly Regex _rotationRight = MyRegex4();
    private readonly Regex _move = MyRegex3();
    private readonly Regex _reverse = MyRegex5();
    private readonly Regex _rotateOnLetter = MyRegex6();
    private readonly List<string> _lines = new();
    private  string _scrambledCode;
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 21;
        automatonBase.RegisterTest(@"swap position 4 with position 0
swap letter d with letter b
reverse positions 0 through 4
rotate left 1 step
move position 1 to position 4
move position 3 to position 0
rotate based on position of letter b
rotate based on position of letter d").Answer1("decab");
        /* no example provided for part 2
         and the example for part 1 is harder to reverse than the provided input.
         Odd length code imply the rotate based on position can be reversed in two different ways, while even length 
         ensure there is only one correct way to reverse it.  
        */
    }

    public override object GetAnswer1()
    {
        if (IsTest)
        {
            _initialCode = !IsTest ? "abcdefgh" : "abcde";
            _scrambledCode = !IsTest ? "fbgdceah" : "abcde";
        }
        
        var status = _initialCode.ToArray();
        return Scramble(status, true);
    }

    private object Scramble(char[] status, bool log)
    {
        // dir is one if we scramble, -1 if we descramble
        var dir = 1;
        foreach (var line in _lines)
        {
            if (log)
            {
                Console.WriteLine($"[{new string(status)}]");
                Console.Write($"'{line}' => ");
            }
            var match = _positionSwap.Match(line);
            if (match.Success)
            {
                status = SwapPositions(status, match.GetInt(1), match.GetInt(2));
                continue;
            }

            match = _letterSwap.Match(line);
            if (match.Success)
            {
                status = SwapLetters(status, match.Groups[1].Value[0], match.Groups[2].Value[0]);
                continue;
            }

            match = _rotationLeft.Match(line);
            if (match.Success)
            {
                status = Rotate(status, match.GetInt(1)*dir);
                continue;
            }

            match = _rotationRight.Match(line);
            if (match.Success)
            {
                status = Rotate(status, -match.GetInt(1)*dir);
                continue;
            }

            match = _rotateOnLetter.Match(line);
            if (match.Success)
            {
                status = RotateOnLetter(status, match.Groups[1].Value[0], dir);
                continue;
            }

            match = _move.Match(line);
            if (match.Success)
            {
                status = Move(status, match.GetInt(1), match.GetInt(2));
            }

            match = _reverse.Match(line);
            if (match.Success)
            {
                status = Reverse(status, match.GetInt(1), match.GetInt(2));
            }
        }

        if (log)
        {
            Console.WriteLine($"Answer is: [{new string(status)}]");
        }
        return new string(status);
    }

    private static char[] RotateOnLetter(char[] status, char letter, int dir)
    {
        var index = FindIndex(status, letter);
        if (dir>0)
        {
            if (index >= 4)
            {
                index++;
                index %= status.Length;
            }
            index++;
        }
        else
        {
            // deduct letter original's position
            if (index % 2 == 1)
            {
                // if it is odd, it was early in the string
                index = (index+1)/2;
            }
            else
            {
                // it was early in the string
                if (index/2>4)
                {
                    index = index / 2 + 1;
                }
                else
                {
                    // there has been a wrap around
                    index += status.Length;
                    // parity may have changed
                    if (index % 2 == 1)
                    {
                        index = (index + 1) / 2;
                    }
                    else
                    {
                        index = index/ 2 + 1;
                    }
                }
            }
        }
        status = Rotate(status, -index*dir);
        return status;
    }

    private static int FindIndex(IReadOnlyList<char> status, char letter)
    {
        var index = 0;
        for (var i = 0; i < status.Count; i++)
        {
            if (status[i] != letter)
            {
                continue;
            }

            index = i;
            break;
        }

        return index;
    }

    private static char[] Reverse(char[] status, int a, int b)
    {
        return status[..a].Union(status[a..(b + 1)].Reverse()).Union(status[(b + 1)..]).ToArray();
    }

    private static char[] Move(char[] status, int a, int b)
    {
        return (a < b
            ? status[..a].Union(status[(a + 1)..(b + 1)]).Append(status[a]).Union(status[(b + 1)..])
            : status[..b].Append(status[a]).Union(status[b..a]).Union(status[(a + 1)..])).ToArray();
    }

    private static char[] Rotate(char[] status, int index)
    {
        if (index < 0)
        {
            index += status.Length;
        }
        else
        {
            index %= status.Length;
        }
        return status[index..].Union(status[..index]).ToArray();
    }

    private static char[] SwapLetters(IReadOnlyList<char> status, char a, char b)
    {
        var result = status.ToArray();
        for (var i = 0; i < status.Count; i++)
        {
            result[i] = status[i] == a ? b : status[i] == b ? a : status[i];
        }

        return result;
    }

    private static char[] SwapPositions(IReadOnlyList<char> status, int a, int b)
    {
        var result = status.ToArray();
        (result[a], result[b]) = (status[b], status[a]);
        return result;
    }

    public override object GetAnswer2()
    {
        var status = _scrambledCode.ToArray();
        var invertedLines = new List<string>(_lines);
        var dir = -1;
        invertedLines.Reverse();
        // perform instructions in reverse order
        // and with reverse operations
        foreach (var line in invertedLines)
        {
            Console.WriteLine($"[{new string(status)}]");
            Console.Write($"'{line}' => ");

            var match = _positionSwap.Match(line);
            if (match.Success)
            {
                // swap works the same both ways
                status = SwapPositions(status, match.GetInt(1), match.GetInt(2));
                continue;
            }

            match = _letterSwap.Match(line);
            if (match.Success)
            {
                // letter swap works the same both ways
                status = SwapLetters(status, match.Groups[1].Value[0], match.Groups[2].Value[0]);
                continue;
            }

            match = _rotationLeft.Match(line);
            if (match.Success)
            {
                // rotate right instead of left
                status = Rotate(status, -match.GetInt(1));
                continue;
            }
            
            match = _rotationRight.Match(line);
            if (match.Success)
            {
                // rotate left instead of right
                status = Rotate(status, match.GetInt(1));
                continue;
            }

            match = _rotateOnLetter.Match(line);
            if (match.Success)
            {
                status = RotateOnLetter(status, match.Groups[1].Value[0], dir);
                continue;
            }

            match = _move.Match(line);
            if (match.Success)
            {
                // must make reverse move (TBC)
                status = Move(status, match.GetInt(2), match.GetInt(1));
            }
            
            match = _reverse.Match(line);
            if (match.Success)
            {
                // reverse is same op
                status = Reverse(status, match.GetInt(1), match.GetInt(2));
            } 
        }
        var answer = new string(status);
        Console.WriteLine($"[{answer}]");

        if (Scramble(status, false).ToString() != _scrambledCode)
        {
            Console.WriteLine($"Answer is invalid: [{answer}]");
            return null;
        }

        return new string(status);
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        _lines.Add(line);
    }

    [GeneratedRegex("swap position (\\d+) with position (\\d+)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
    [GeneratedRegex("swap letter (\\w) with letter (\\w)", RegexOptions.Compiled)]
    private static partial Regex MyRegex1();
    [GeneratedRegex("rotate left (\\d+) step", RegexOptions.Compiled)]
    private static partial Regex MyRegex2();
    [GeneratedRegex("move position (\\d+) to position (\\d+)", RegexOptions.Compiled)]
    private static partial Regex MyRegex3();
    [GeneratedRegex("rotate right (\\d+) step", RegexOptions.Compiled)]
    private static partial Regex MyRegex4();
    [GeneratedRegex("reverse positions (\\d+) through (\\d+)", RegexOptions.Compiled)]
    private static partial Regex MyRegex5();
    [GeneratedRegex("rotate based on position of letter (\\w)", RegexOptions.Compiled)]
    private static partial Regex MyRegex6();
}
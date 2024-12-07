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

public class DupdobDay07 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 7;
        automatonBase.RegisterTestDataAndResult("""
                                                190: 10 19
                                                3267: 81 40 27
                                                83: 17 5
                                                156: 15 6
                                                7290: 6 8 6 15
                                                161011: 16 10 13
                                                192: 17 8 14
                                                21037: 9 7 18 13
                                                292: 11 6 16 20
                                                """, 3749, 1);
        automatonBase.RegisterTestResult(11387, 2);
    }

    public override object GetAnswer1()
    {
        var result = 0L;
        foreach (var quiz in _quizzes)
        {
            var (value, operands) = quiz;
            if (Solve(value, operands, 1, operands[0]))
            {
                result+=value;
            }
        }
        return result;
    }

    private bool Solve(long value, List<long> operands, int index, long current)
    {
        if (index == operands.Count)
        {
            return current == value;
        }

        if (current > value)
        {
            return false;
        }

        return Solve(value, operands, index + 1, current + operands[index])
               || Solve(value, operands, index + 1, current * operands[index]);
    }

    public override object GetAnswer2()
    {
        var result = 0L;
        foreach (var quiz in _quizzes)
        {
            var (value, operands) = quiz;
            if (Solve2(value, operands, 1, operands[0]))
            {
                result+=value;
            }
        }
        return result;
    }

    private bool Solve2(long value, List<long> operands, int index, long current)
    {
        if (index == operands.Count)
        {
            return current == value;
        }

        if (current > value)
        {
            return false;
        }

        return Solve2(value, operands, index + 1, current + operands[index])
               || Solve2(value, operands, index + 1, current * operands[index])
               || Solve2(value, operands, index + 1, Concatenate(current, operands[index]));
    }

    private long Concatenate(long current, long operand)
    {
        return long.Parse(current.ToString() + operand.ToString());
    }

    private readonly List<(long result, List<long> operands)> _quizzes = [];
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var parts = line.Split(":");
        var result = long.Parse(parts[0]);
        var operands = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
        _quizzes.Add((result, operands));
    }
}
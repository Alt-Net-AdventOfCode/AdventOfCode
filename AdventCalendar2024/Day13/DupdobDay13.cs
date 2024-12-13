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
using System.Xml.Resolvers;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay13 : SolverWithParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 13;
        automatonBase.RegisterTestDataAndResult("""
                                                Button A: X+94, Y+34
                                                Button B: X+22, Y+67
                                                Prize: X=8400, Y=5400
                                                """, 280, 1);
        automatonBase.RegisterTestDataAndResult("""
                                                Button A: X+17, Y+86
                                                Button B: X+84, Y+37
                                                Prize: X=7870, Y=6450
                                                """, 200, 1);
        automatonBase.RegisterTestDataAndResult("""
                                                Button A: X+94, Y+34
                                                Button B: X+22, Y+67
                                                Prize: X=8400, Y=5400

                                                Button A: X+26, Y+66
                                                Button B: X+67, Y+21
                                                Prize: X=12748, Y=12176

                                                Button A: X+17, Y+86
                                                Button B: X+84, Y+37
                                                Prize: X=7870, Y=6450

                                                Button A: X+69, Y+23
                                                Button B: X+27, Y+71
                                                Prize: X=18641, Y=10279
                                                """, 480, 1);
    }

    private record Button(long X, long Y);
    private record Prize(long X, long Y);
    
    private record Problem(Button A, Button B, Prize P);
    
    private readonly List<Problem> _claws = new();
    
    public override object GetAnswer1()
    {
        var neededTokens = 0;
        foreach (var claw in _claws)
        {
            neededTokens += BruteForce(claw);
        }
        return neededTokens;
    }

    private int BruteForce(Problem claw)
    {   
        var neededTokens = int.MaxValue;

        for (var a = 0; a < 101; a++)
        {
            for (var b = 0; b < 101; b++)
            {
                if (claw.A.X * a + claw.B.X * b == claw.P.X && claw.A.Y * a + claw.B.Y * b == claw.P.Y)
                {
                    neededTokens = Math.Min(neededTokens, 3 * a + b);
                    break;
                }
            }
        }
        return neededTokens == int.MaxValue ? 0 : neededTokens;
    }
    
    private long Solve(Problem claw)
    {
        var num = claw.P.X * claw.B.Y - claw.B.X * claw.P.Y;
        var d = claw.A.X * claw.B.Y - claw.B.X * claw.A.Y;
        var a = num / d;
        var b = (claw.P.X - a * claw.A.X) / claw.B.X;
        
        if (claw.A.X*a + claw.B.X*b != claw.P.X || claw.A.Y*a + claw.B.Y*b != claw.P.Y)
        {
            return 0;
        }
        return a*3+b;
    }

    public override object GetAnswer2()
    {
        var neededTokens = 0L;
        foreach (var claw in _claws)
        {
            var modifiedClaw = claw with { P = new Prize(claw.P.X+10000000000000, claw.P.Y+10000000000000) };
            neededTokens += Solve(modifiedClaw);
        }
        return neededTokens;
    }

    protected override void Parse(string data)
    {
        var buttonRegex = new Regex("Button (.*): X([+-]\\d+), Y([+-]\\d+)");
        var prizeRegex = new Regex("Prize: X=([+-]?\\d+), Y=([+-]?\\d+)");
        var lines = data.Split('\n'); 
        // we discard the last line if it is empty (trailing newline), but we keep any intermediate newlines
        if (lines[^1].Length == 0) lines = lines[..^1];
        for (var i = 0; i < lines.Length; i += 4)
        {
            var match = buttonRegex.Match(lines[i]);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse Button line:{0}", lines[i]);
                continue;
            }
            var buttonA = new Button(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            match = buttonRegex.Match(lines[i+1]);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse Button line:{0}", lines[i+1]);
                continue;
            }
            var buttonB = new Button(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            match = prizeRegex.Match(lines[i+2]);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse Prize line:{0}", lines[i+1]);
                continue;
            }
            var prize = new Prize(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            _claws.Add(new Problem(buttonA, buttonB, prize));
        }
    }

}
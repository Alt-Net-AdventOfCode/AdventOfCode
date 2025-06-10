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

using System.Text;
using AoC;

namespace AdventCalendar2022;

public class DupdobDay10 : SolverWithLineParser
{
    private struct Cpu
    {
        public int X;
        public int Cycle;
    }

    private readonly List<Func<Cpu, Cpu>> _program = new();
    private static Cpu AddX(Cpu cpu, int dx)
    {
        var newCpu = new Cpu();
        newCpu.X += cpu.X + dx;
        newCpu.Cycle = cpu.Cycle + 2;
        return newCpu;
    }

    private static Cpu Noop(Cpu cpu)
    {
        var newCpu = cpu with { Cycle = cpu.Cycle + 1 };
        return newCpu;
    }
    
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 10;
        // ReSharper disable  StringLiteralTypo
        dayAutomaton.AddExample(@"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop");
        dayAutomaton.RegisterTestResult(13140L);
        dayAutomaton.RegisterTestResult("ok", 2);
    }

    public override object GetAnswer1()
    {
        var score = 0L;
        var cpu = new Cpu() {X = 1, Cycle = 1};
        var nexStep = 20;
        foreach (var action in _program)
        {
            var oldCpu = cpu;
            cpu = action(oldCpu);
            if (cpu.Cycle >= nexStep)
            {
                score += (cpu.Cycle == nexStep ? cpu.X : oldCpu.X) * nexStep;
                nexStep += 40;
            }   
        }

        return score;
    }

    public override object GetAnswer2()
    {
        var cpu = new Cpu() {X = 1, Cycle = 1};
        var crt = new StringBuilder(260);
        var cursor = 0;
        foreach (var action in _program)
        {
            var oldCpu = cpu;
            cpu = action(oldCpu);
            for (var i = oldCpu.Cycle; i < cpu.Cycle; i++)
            {
                crt.Append(Math.Abs(cursor - oldCpu.X) <= 1 ? 'O' : ' ');
                cursor++;
                if (cursor == 40)
                {
                    cursor = 0;
                    crt.Append(Environment.NewLine);
                }
            } 
        }
        Console.WriteLine(crt);
        return Console.ReadLine() ?? "fail";
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        if (line == "noop")
        {
            _program.Add(Noop);
        }
        else if (line.StartsWith("addx"))
        {
            _program.Add(c => AddX(c, int.Parse(line[5..])));
        }
    }
}
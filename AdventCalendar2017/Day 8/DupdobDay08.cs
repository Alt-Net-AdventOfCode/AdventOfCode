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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2017;

public partial class DupdobDay08: SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 8;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                b inc 5 if a > 1
                                                a inc 1 if b < 5
                                                c dec -10 if a >= 1
                                                c inc -20 if c == 10
                                                """, 1, 1);
    }

    public override object GetAnswer1()
    {
        var registers = new Dictionary<string, int>();
        _totalMax = int.MinValue;
        foreach (var instruction in _program)
        {
            // check condition
            var condition = instruction.Condition;
            switch (condition.Comparison)
            {
                case ">":
                    if (registers.GetValueOrDefault(condition.Register) <= condition.Value)
                        continue;
                    break;
                case ">=":
                    if (registers.GetValueOrDefault(condition.Register) < condition.Value)
                        continue;
                    break;
                case "<":
                    if (registers.GetValueOrDefault(condition.Register) >= condition.Value)
                        continue;
                    break;

                case "<=":
                    if (registers.GetValueOrDefault(condition.Register) > condition.Value)
                        continue;
                    break;

                case "==":
                    if (registers.GetValueOrDefault(condition.Register) != condition.Value)
                        continue;
                    break;
                case "!=":
                    if (registers.GetValueOrDefault(condition.Register) == condition.Value)
                        continue;
                    break;
            }

            registers[instruction.Register] = instruction.Operation switch
            {
                "inc" => registers.GetValueOrDefault(instruction.Register) + instruction.Immediate,
                "dec" => registers.GetValueOrDefault(instruction.Register) - instruction.Immediate,
                _ => registers.GetValueOrDefault(instruction.Register)
            };
            _totalMax = _totalMax = Math.Max(_totalMax, registers.Values.Max());
        }

        return registers.Values.Max();
    }

    public override object GetAnswer2()
    {
        return _totalMax;
    }

    private readonly Regex _wrapper = MyRegex();
    private List<Instruction> _program = [];
    private int _totalMax;

    private record Condition(string Register, string Comparison, int Value);

    private record Instruction(string Register, string Operation, int Immediate, Condition Condition);
    
    protected override void Parse(string data) => ParseLines(data.SplitLines());

    private void ParseLines(string[] lines)
    {
        foreach (var line in lines)
        {
            var match = _wrapper.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse= {0}", line);
                continue;
            }

            var condition = new Condition(match.Groups[4].Value, match.Groups[5].Value,
                int.Parse(match.Groups[6].Value));
            var instruction = new Instruction(match.Groups[1].Value, match.Groups[2].Value,
                int.Parse(match.Groups[3].Value), condition);
            _program.Add(instruction);
        }
    }

    [GeneratedRegex(@"(\w+)\s+(\w+)\s+(-?\d+)\s+if\s+(\w+)\s+(\S*)\s+(\S*)")]
    private static partial Regex MyRegex();
}
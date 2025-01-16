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

using System.Text.RegularExpressions;
using AoC;
using AoCAlgorithms;

namespace AdventCalendar2022;

public partial class DupdobDay21 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 21;
        // ReSharper disable StringLiteralTypo
        automaton.AddExample(@"root: pppw + sjmn
dbpl: 5
cczh: sllz + lgvd
zczc: 2
ptdq: humn - dvpt
dvpt: 3
lfqf: 4
humn: 5
ljgn: 2
sjmn: drzm * dbpl
sllz: 4
pppw: cczh / lfqf
lgvd: ljgn * ptdq
drzm: hmdt - zczc
hmdt: 32");
        automaton.RegisterTestResult(152L);
        automaton.RegisterTestResult(301L,2);
    }

    public override object GetAnswer1()
    {
        var root = _monkeys["root"];
        return root.Solve(_monkeys);
    }

    public override object GetAnswer2()
    {
        var root = (OperationMonkey) _monkeys["root"];
        var left = (OperationMonkey) _monkeys[root.LeftMonkey];
        var right =(OperationMonkey) _monkeys[root.RightMonkey];
        // ReSharper disable once StringLiteralTypo
        if (left.DependsOn("humn", _monkeys))
        {
            (left, right) = (right, left);
        }

        var leftValue = left.Solve(_monkeys);

        return right.InverseTo("humn", leftValue, _monkeys);
    }

    private readonly Regex _numberParser = MyRegex1();
    private readonly Regex _operationParser = MyRegex();

    private abstract class Monkey
    {
        public abstract long Solve(Dictionary<string, Monkey> monkeys);
        public abstract bool DependsOn(string name, Dictionary<string, Monkey> monkeys);
        public abstract long InverseTo(string monkeyToSolve, long expected, Dictionary<string, Monkey> monkeys);
    }

    private class NumberMonkey : Monkey
    {
        private int Number { get; }
        private string Name { get; }

        public NumberMonkey(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public override long Solve(Dictionary<string, Monkey> monkeys) => Number;

        public override bool DependsOn(string name, Dictionary<string, Monkey> _) => name == Name;
        
        public override long InverseTo(string monkeyToSolve, long expected, Dictionary<string, Monkey> _)
        {
            if (Name != monkeyToSolve)
            {
                throw new InvalidOperationException();
            }

            return expected;
        }
    }

    private class OperationMonkey : Monkey
    {
        public string LeftMonkey { get; }
        private string Operator { get; }
        public string RightMonkey { get; }

        public OperationMonkey(string leftMonkey, string @operator, string rightMonkey)
        {
            LeftMonkey = leftMonkey;
            Operator = @operator;
            RightMonkey = rightMonkey;
        }

        public override long Solve(Dictionary<string, Monkey> monkeys)
        {
            var left = monkeys[LeftMonkey].Solve(monkeys);
            var right = monkeys[RightMonkey].Solve(monkeys);
            return Operator switch
            {
                "+" => left + right,
                "-" => left - right,
                "*" => left * right,
                "/" => left / right,
                _ => throw new InvalidOperationException()
            };
        }

        public override bool DependsOn(string name, Dictionary<string, Monkey> monkeys) => monkeys[LeftMonkey].DependsOn(name, monkeys) || monkeys[RightMonkey].DependsOn(name, monkeys);

        public override long InverseTo(string monkeyToSolve, long expected, Dictionary<string, Monkey> monkeys)
        {
            var left = monkeys[LeftMonkey];
            var right = monkeys[RightMonkey];
            var inverted = false;
            if (left.DependsOn(monkeyToSolve, monkeys))
            {
                (left, right) = (right, left);
                inverted = true;
            }
            else if (!right.DependsOn(monkeyToSolve, monkeys))
            {
                return expected;
            }
            var leftValue = left.Solve(monkeys);
            return Operator switch
            {
                "+" => right.InverseTo(monkeyToSolve, expected - leftValue, monkeys),
                "-" => right.InverseTo(monkeyToSolve, leftValue + (inverted ? expected : -expected), monkeys),
                "*" => right.InverseTo(monkeyToSolve, expected / leftValue, monkeys),
                "/" when inverted => right.InverseTo(monkeyToSolve, leftValue * expected, monkeys),
                "/" => right.InverseTo(monkeyToSolve, leftValue / expected, monkeys),
                _ => throw new InvalidOperationException()
            };
        }
    }

    private readonly Dictionary<string, Monkey> _monkeys = new ();
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        var match = _numberParser.Match(line);
        if (match.Success)
        {
            _monkeys[match.GetString(1)] = new NumberMonkey(match.GetString(1), match.GetInt(2));
            return;
        }

        match = _operationParser.Match(line);
        if (match.Success)
        {
            _monkeys[match.GetString(1)] = new OperationMonkey(match.GetString(2), match.GetString(3), match.GetString(4));
        }
    }

    [GeneratedRegex("(\\w+): (\\w+) ([-+\\*\\/]) (\\w+)")]
    private static partial Regex MyRegex();
    [GeneratedRegex("(\\w+): (-?\\d+)")]
    private static partial Regex MyRegex1();
}
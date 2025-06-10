using System;
using System.Collections.Generic;
using AoC;

namespace AdventCalendar2016
{
    public class DupdobDay12: SolverWithLineParser
    {
        private readonly int[] _registers = new int[4];
        private int _pc;

        private readonly List<Action> _program = [];

        public override void SetupRun(DayAutomaton dayAutomaton)
        {
            dayAutomaton.Day = 12;
            dayAutomaton.AddExample(@"cpy 41 a
inc a
inc a
dec a
jnz a 2
dec a");
            dayAutomaton.RegisterTestResult(42);
        }

        private int NameToIndex(string register)
        {
            return register[0] - 'a';
        }

        private Func<int> TokenToValue(string token)
        {
            if (int.TryParse(token, out var value))
            {
                return () =>  value;
            }
            else
            {
                var index = NameToIndex(token);
                return () =>  _registers[index];
            }
        }
        
        protected override void ParseLine(string line, int index, int lineCount)
        {
            var tokens = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            switch (tokens[0])
            {
                case "cpy":
                    var operand = TokenToValue(tokens[1]); 
                    _program.Add(() => _registers[NameToIndex(tokens[2])] = operand());
                    break;
                case "inc":
                    _program.Add(() => _registers[NameToIndex(tokens[1])]++);
                    break;
                case "dec":
                    _program.Add(() => _registers[NameToIndex(tokens[1])]--);
                    break;
                case "jnz":
                    operand = TokenToValue(tokens[1]); 
                    _program.Add(() =>
                    {
                        if (operand() != 0)
                        {
                            _pc += int.Parse(tokens[2]) - 1;
                        }
                    });
                    break;
            }
        }

        public override object GetAnswer1()
        {
            for (_pc = 0; _pc < _program.Count; _pc++)
                _program[_pc]();
            return _registers[NameToIndex("a")];
        }

        public override object GetAnswer2()
        {
            for (var i = 0; i < _registers.Length; i++)
            {
                _registers[i] = 0;
            }

            _registers[NameToIndex("c")] = 1;
            for (_pc = 0; _pc < _program.Count; _pc++)
                _program[_pc]();
            return _registers[NameToIndex("a")];
        }

    }
}
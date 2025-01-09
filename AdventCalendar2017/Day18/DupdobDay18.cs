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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay18: SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 18;
        automatonBase.RegisterTestDataAndResult("""
                                                set a 1
                                                add a 2
                                                mul a a
                                                mod a 5
                                                snd a
                                                set a 0
                                                rcv a
                                                jgz a -1
                                                set a 1
                                                jgz a -2
                                                """, 4, 1);

        automatonBase.RegisterTestDataAndResult("""
                                                snd 1
                                                snd 2
                                                snd p
                                                rcv a
                                                rcv b
                                                rcv c
                                                rcv d
                                                """, 3, 2);
    }

    private class Processor
    {
        private readonly Dictionary<char, long> _registers = [];
        private int _pc;
        private readonly List<Instruction> _program1;
        private readonly bool _nzReceive;

        public Processor(List<Instruction> program, int id)
        {
            _program1 = program;
            _registers['p'] = id;
        }

        public Processor(List<Instruction> program)
        {
            _program1 = program;
            _nzReceive = true;

        }

        public Queue<long> OutFrequencies { get; } = [];

        public Queue<long> InFrequencies { get; set; } = [];
        
        public int Sent { get; private set; }

        public bool Run()
        {
            for(; _pc<_program1.Count; _pc++)
            {
                var current = _program1[_pc];
                switch (current.Opcode)
                {
                    case "snd":
                        // play snd
                        OutFrequencies.Enqueue(_registers.GetValueOrDefault(current.Register));
                        Sent++;
                        break;
                    case "set":
                        _registers[current.Register] = RegisterOrImmediate(current.Operand);
                        break;
                    case "add":
                        _registers[current.Register] = _registers.GetValueOrDefault(current.Register) + RegisterOrImmediate(current.Operand);
                        break;
                    case "mul":
                        _registers[current.Register] = _registers.GetValueOrDefault(current.Register) * RegisterOrImmediate(current.Operand);
                        break;                
                    case "mod":
                        _registers[current.Register] = _registers.GetValueOrDefault(current.Register) % RegisterOrImmediate(current.Operand);
                        break;
                    case "rcv":
                        if (!_nzReceive || _registers.GetValueOrDefault(current.Register) != 0)
                        {
                            if (!InFrequencies.TryDequeue(out var frequency))
                            {
                                // we are waiting
                                return true;
                            }
                            _registers[current.Register] = frequency;
                        }
                        break;
                    case "jgz":
                        if (RegisterOrImmediate(current.Register.ToString()) > 0)
                        {
                            _pc += (int)RegisterOrImmediate(current.Operand);
                            _pc--;
                        }
                        break;
                }
            }

            return false;
        
            long RegisterOrImmediate(string operand) => operand is [>= 'a' and <= 'z'] ? _registers.GetValueOrDefault(operand[0]) : long.Parse(operand);
        }
    }
    
    public override object GetAnswer1()
    {
        var processor = new Processor(_program)
        {
            InFrequencies = []
        };
        if (!processor.Run())
        {
            return null;
        }

        return processor.OutFrequencies.Last();
    }

    public override object GetAnswer2()
    {
        var processors = new Processor[2];
        processors[0] = new Processor(_program, 0);
        processors[1] = new Processor(_program, 1);
        processors[0].InFrequencies = processors[1].OutFrequencies;
        processors[1].InFrequencies = processors[0].OutFrequencies;

        do
        {
            processors[1].Run();
            processors[0].Run();
        } while (processors[0].OutFrequencies.Count > 0 || processors[1].OutFrequencies.Count>0);
        
        return processors[1].Sent;
    }

    private record Instruction(string Opcode, char Register, string Operand);

    private readonly List<Instruction> _program = [];
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var blocks = line.Split(' ', StringSplitOptions.TrimEntries);
        var register = blocks[1][0];
        var operand = blocks.Length > 2 ? blocks[2] : string.Empty;
        _program.Add(new Instruction(blocks[0], register, operand));
    }
}
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

public class DupdobDay23 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 23;
        automatonBase.AddExample("""
                                 set a 1
                                 mul a a
                                 set a 0
                                 jnz a -1
                                 set a 1
                                 """).Answer1(1);
    }

    private class Processor(List<Instruction> program)
    {
    private Dictionary<char, long> Registers { get; } = [];
        
        private int _pc;

        public int MulInvocations { get; private set; }

        public void Run()
        {
            for(; _pc<program.Count; _pc++)
            {
                var current = program[_pc];
                switch (current.Opcode)
                {
                    case "set":
                        Registers[current.Register] = RegisterOrImmediate(current.Operand);
                        break;
                    case "sub":
                        Registers[current.Register] = Registers.GetValueOrDefault(current.Register)- RegisterOrImmediate(current.Operand);
                        break;
                    case "mul":
                        Registers[current.Register] = Registers.GetValueOrDefault(current.Register) * RegisterOrImmediate(current.Operand);
                        MulInvocations++;
                        break;                
                    case "jnz":
                        if (RegisterOrImmediate(current.Register.ToString()) != 0)
                        {
                            _pc += (int)RegisterOrImmediate(current.Operand);
                            _pc--;
                        }
                        break;
                }
            }

            return;

            long RegisterOrImmediate(string operand) => operand is [>= 'a' and <= 'z'] ? Registers.GetValueOrDefault(operand[0]) : long.Parse(operand);
        }
    }
    
    public override object GetAnswer1()
    {
        var processor = new Processor(_program);
        processor.Run();
        return processor.MulInvocations;
    }

    public override object GetAnswer2()
    {
        var b = 57 * 100;
        b += 100000;
        var c = b + 17000;
        var count = 0;
        // code reverse engineered shows this code count non prime number for a range
        for (; b <= c; b += 17)
        {
            if (!IsPrime(b))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsPrime(int i)
    {
        if (i % 2 == 0)
        {
            return false;
        }

        var limit = (int)Math.Sqrt(i);
        for (var div = 3; div <= limit; div += 2)
        {
            if (i % div == 0)
            {
                return false;
            }
        }

        return true;
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
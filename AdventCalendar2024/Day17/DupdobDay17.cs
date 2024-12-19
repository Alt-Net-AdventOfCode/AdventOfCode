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

using System.Diagnostics.CodeAnalysis;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay17 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 17;
        automatonBase.RegisterTestDataAndResult("""
                                                Register A: 729
                                                Register B: 0
                                                Register C: 0

                                                Program: 0,1,5,4,3,0
                                                """, "4,6,3,5,6,3,5,2,1,0", 1);

        automatonBase.RegisterTestDataAndResult("""
                                                Register A: 2024
                                                Register B: 0
                                                Register C: 0

                                                Program: 0,3,5,4,3,0
                                                """, 2024, 2);
    }

    public override object GetAnswer1() => string.Join(',', RunProgram(_registers.ToArray()));

    private List<long> RunProgram(long[] state)
    {
        var output = new List<long>();
        for (var pc = 0; pc < _program.Count; pc += 2)
        {
            var operand = _program[pc + 1];
            switch (_program[pc])
            {
                case 0:
                    state[0] /= 1 << (int)(Combo(operand) % 64);
                    break;
                case 1:
                    state[1] ^= operand;
                    break;
                case 2:
                    state[1] = Combo(operand) % 8;
                    break;
                case 3:
                    if (state[0] != 0)
                    {
                        pc = operand - 2;
                    }

                    break;
                case 4:
                    state[1] ^= state[2];
                    break;
                case 5:
                    output.Add(Combo(operand) % 8);
                    break;
                case 6:
                    state[1] = state[0] / (1 << (int)(Combo(operand) % 64));
                    break;
                case 7:
                    state[2] = state[0] / (1 << (int)(Combo(operand) % 64));
                    break;
            }
        }

        return output;
        long Combo(long operand) => operand > 3 ? state[operand - 4] : operand;
    }

    public override object GetAnswer2()
    {
        var len = _program.Count;
        if (len != 16)
        {
            return 2024;
        }
        var aBits = new int[len];
        var possibleDigits = new List<List<int>>();
        var firstDigit = 0;
        for (var rank = 0; rank <aBits.Length; rank++)
        {
            var digitFound = false;
            possibleDigits.Add([]);


            for (var digit =firstDigit; digit<8; digit++)
            {
                // build A
                aBits[rank] = digit;
                var A = aBits.Aggregate(0L, (current, bit) => (current << 3) + bit);
                var state = new[] { A, 0, 0 };
                var result = RunProgram(state);
                if (result.Count != _program.Count) continue;
                var resultOk = true;
                for (var i = 0; i <= rank && resultOk; i++)
                {
                    resultOk = result[^(i+1)] == _program[^(i+1)];
                }

                if (!resultOk)
                {
                    continue;
                }
                // we found the digit
                digitFound = true;
                possibleDigits[rank].Add(digit);
                break;
            }

            if (!digitFound)
            {
                // we need to try something else
                // hack to continue scanning the previous digit
                rank --;
                firstDigit = aBits[rank] + 1;
                rank--;
            }
            else
            {
                firstDigit = 0;
            }
            
        }
        
        var final = RunProgram([aBits.Aggregate(0L, (current, bit) => (current << 3) + bit), 0, 0]);
        
        if (final.Where((t, i) => t != _program[i]).Any())
        {
            return null;
        }
        // the program works A by block of three bits, so we can figure each 3bit digits separately
        return aBits.Aggregate(0L, (current, bit) => (current << 3) + bit);
    }

    private readonly long[] _registers = new long[3];
    private List<int> _program = [];

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (line.StartsWith("Register "))
        {
            var id = line[9] - 'A';
            _registers[id] = int.Parse(line[11..]);
        }
        else if (line.StartsWith("Program:"))
        {
            _program = line[8..].Split(',').Select(int.Parse).ToList();
        }
    }
}
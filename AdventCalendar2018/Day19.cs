using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    public static class Day19
    {

        private delegate Registers Operation(Registers reg, int a, int b, int c);
        
        private static void MainDay19()
        {
            var mapping = new Dictionary<string, Operation>
            {
                ["addr"] = addr, ["addi"] = addi,
                ["mulr"] = mulr, ["muli"] = muli,
                ["banr"] = banr, ["bani"] = bani,
                ["borr"] = borr, ["bori"] = bori,
                ["setr"] = setr, ["seti"] = seti,
                ["gtir"] = gtir, ["gtri"] = gtri, ["gtrr"] = gtrr,
                ["eqir"] = eqir, ["eqri"] = eqri, ["eqrr"] = eqrr
            };
            var ip = 2;
            var lines = Input.Split(Environment.NewLine);
            var parser = new Regex(LineExpr);
            var state = new Registers(1, 0, 0, 0, 0, 0);
            var compiled = new (string instr, int A, int B, int C)[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                compiled[i] = ExtractOp(parser.Match(lines[i]));
            }
            Console.WriteLine($"Translated = {TranslatedAlgo(10551374)}");
            while (true)
            {
                var instructionPointer = state.Get(ip);
                
                if (instructionPointer < 0 || instructionPointer >= lines.Length)
                {
                    break;
                }
                var (instr, A, B, C) = compiled[instructionPointer];
                state = mapping[instr].Invoke(state, A, B, C);
                state.Incr(ip);
//                Console.WriteLine($" => {state}");
            }
            Console.WriteLine($"Fin: {state}");
        }

        private static int TranslatedAlgo(int seed)
        {
            var result = 0;
            for (int i= 1; i <= seed; i++)
            {
                if (seed % i == 0)
                {
                    result += i;
                }
            }
            return result;
        }
        private static (string, int, int, int) ExtractOp(Match result)
        {
            var code = result.Groups[1].Value;
            var A = int.Parse(result.Groups[2].Value);
            var B = int.Parse(result.Groups[3].Value);
            var C = int.Parse(result.Groups[4].Value);
            return (code, A, B, C);
        }

        private static Registers addr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, B, C, (a, b) => a + b);
        }
        
        private static Registers addi(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(A, B, C, (a, b) => a + b);
        }

        private static Registers mulr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, B, C, (a, b) => a * b);
        }
        
        private static Registers muli(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(A, B, C, (a, b) => a * b);
        }
        
        private static Registers banr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, B, C, (a, b) => a & b);
        }
        
        private static Registers bani(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(A, B, C, (a, b) => a & b);
        }
        
        private static Registers borr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, B, C, (a, b) => a | b);
        }
        
        private static Registers bori(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(A, B, C, (a, b) => a | b);
        }
        
        private static Registers setr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, 0, C, (a, _) => a);
        }
        
        private static Registers seti(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(0, A, C, (_, b) => b);
        }

        private static Registers gtir(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(B, A, C, (b, a) => a > b ? 1 : 0);
        }
        
        private static Registers gtri(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(A, B, C, (a, b) => a > b ? 1 : 0);
        }
        
        private static Registers gtrr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, B, C, (a, b) => a > b ? 1 : 0);
        }
        
        private static Registers eqir(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(B, A, C, (a, b) => a == b ? 1 : 0);
        }
        
        private static Registers eqri(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(A, B, C, (a, b) => a == b ? 1 : 0);
        }
        
        private static Registers eqrr(Registers reg, int A, int B, int C)
        {
            return reg.RegisterOp(A, B, C, (a, b) => a == b ? 1 : 0);
        }
        
        private sealed class Registers
        {
            private int[] Reg = new int[6];

            private bool Equals(Registers other)
            {
                return Reg[0] == other.Reg[0] &&
                       Reg[1] == other.Reg[1] &&
                       Reg[2] == other.Reg[2] &&
                       Reg[3] == other.Reg[3] &&
                    Reg[4] == other.Reg[4] &&
                    Reg[5] == other.Reg[5];
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Registers) obj);
            }

            public override int GetHashCode()
            {
                return Reg.Aggregate((current, next) => current*13+next);
            }

            public int Get(int id)
            {
                return Reg[id];
            }

            public void Incr(int id)
            {
                Reg[id]++;
            }

            public Registers(int r0, int r1, int r2, int r3, int r4, int r5)
            {
                Reg[0] = r0;    
                Reg[1] = r1;
                Reg[2] = r2;
                Reg[3] = r3;
                Reg[4] = r4;
                Reg[5] = r5;
            }

            private Registers CloneAndChange(int id, int value)
            {
                var result = new Registers(Reg[0], Reg[1], Reg[2], Reg[3], Reg[4], Reg[5]) {Reg = {[id] = value}};
                return result;
            }

            public Registers RegisterOp(int p1, int p2, int p3, Func<int, int, int> operation)
            {
                return CloneAndChange(p3, operation(Reg[p1], Reg[p2]));
            }
            
            public Registers ImmediateOp(int p1, int p2, int p3, Func<int, int, int> operation)
            {
                return CloneAndChange(p3, operation(Reg[p1], p2));
            }

            public override string ToString()
            {
                return $"{nameof(Reg)}: {Reg[0]},{Reg[1]},{Reg[2]},{Reg[3]},{Reg[4]},{Reg[5]}";
            }
        }

        private const string LineExpr = "(\\w+) (\\d+) (\\d+) (\\d+)";

        private const string Demo =
            @"seti 5 0 1
seti 6 0 2
addi 0 1 0
addr 1 2 3
setr 1 0 0
seti 8 0 4
seti 9 0 5";
        private const string Input =
            @"addi 2 16 2
seti 1 0 4
seti 1 5 5
mulr 4 5 1
eqrr 1 3 1
addr 1 2 2
addi 2 1 2
addr 4 0 0
addi 5 1 5
gtrr 5 3 1
addr 2 1 2
seti 2 6 2
addi 4 1 4
gtrr 4 3 1
addr 1 2 2
seti 1 7 2
mulr 2 2 2
addi 3 2 3
mulr 3 3 3
mulr 2 3 3
muli 3 11 3
addi 1 6 1
mulr 1 2 1
addi 1 6 1
addr 3 1 3
addr 2 0 2
seti 0 3 2
setr 2 3 1
mulr 1 2 1
addr 2 1 1
mulr 2 1 1
muli 1 14 1
mulr 1 2 1
addr 3 1 3
seti 0 9 0
seti 0 5 2";
    }
}
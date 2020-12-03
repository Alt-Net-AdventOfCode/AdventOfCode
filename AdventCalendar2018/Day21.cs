using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    public static class Day21
    {

        private delegate Registers Operation(Registers reg, int a, int b, int c);
        
        private static void MainDay21()
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
            var ip = 1;
            var lines = Input.Split("\r\n");

            var parser = new Regex(LineExpr);
            var state = new Registers(1, 0, 0, 0, 0, 0);
            var compiled = new (string instr, int A, int B, int C)[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                compiled[i] = ExtractOp(parser.Match(lines[i]));
            }
  
            for(var i = 0; i<100; i++)
            {
                var instructionPointer = state.Get(ip);
                
                if (instructionPointer < 0 || instructionPointer >= lines.Length)
                {
                    break;
                }
                var (instr, A, B, C) = compiled[instructionPointer];
                state = mapping[instr].Invoke(state, A, B, C);
                state.Incr(ip);
                Console.WriteLine($"{lines[instructionPointer]} => {state}");
//                Console.ReadKey();
            }
            Algo(0);
            Console.WriteLine($"Fin: {state}");
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

        private static int Algo(int A)
        {
            var R4 = 0; // Reg 4
            var R5 = 0; // Reg 5
            var R2 = 0; // 2
            var R3 = 0; // 3
            var list = new HashSet<int>();
            var previous = 0;
            R4 = 0;
labelB:
            R5 = R4 | 0x10000;
            R4 = 10704114;
loop2:
            R2 = R5 & 0xFF;

            R4 += R2;
            R4 &= 0xFFFFFF;
            R4 *= 65899;
            R4 &= 0xFFFFFF;

            if (R5 < 256)
            {
                // goto 28
                goto label;
            }
/*
            R2 = 0;
loop1: 
            R3 = R2 + 1;
            R3 *= 256;
            if (R3 > R5)
            {
                // goto 25
            }

            R2++;
            goto loop1;
            
            R2 = 0;
            for(;;)
            {
                R3 = (R2 + 1) * 256;
                if (R3 > R5)
                {
                    break;
                }
            }
            */
            R5 = R5/256;
            goto loop2;
label:
            if (R4 != A)
            {
                if (!list.Contains(R4))
                {
                    list.Add(R4);
                    previous = R4;
                }
                else
                {
                    Console.WriteLine($"R4 = {previous}");
                    return 0;
                }
                goto labelB;
            }
            return 0;
        }

        private const string LineExpr = "(\\w+) (\\d+) (\\d+) (\\d+)";

        private const string Input =
            @"seti 123 0 4
bani 4 456 4
eqri 4 72 4
addr 4 1 1
seti 0 0 1
seti 0 0 4
bori 4 65536 5
seti 10704114 0 4
bani 5 255 2
addr 4 2 4
bani 4 16777215 4
muli 4 65899 4
bani 4 16777215 4
gtir 256 5 2
addr 2 1 1
addi 1 1 1
seti 27 2 1
seti 0 4 2
addi 2 1 3
muli 3 256 3
gtrr 3 5 3
addr 3 1 1
addi 1 1 1
seti 25 5 1
addi 2 1 2
seti 17 5 1
setr 2 6 5
seti 7 8 1
eqrr 4 0 2
addr 2 1 1
seti 5 3 1";
    }
}
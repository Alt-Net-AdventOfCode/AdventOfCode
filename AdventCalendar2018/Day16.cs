using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    public static class Day16
    {

        private delegate Registers Operation(Registers reg, int a, int b, int c);
        
        private static void MainDay16()
        {
            var ops = new Operation[]{addr, addi, mulr, muli, banr, bani, bori, borr, setr, seti, gtir, gtri, gtrr, eqri, eqir, eqrr};
            var lines = Input.Split(Environment.NewLine);
            var before = new Regex(BeforeExpr);
            var instructions = new Regex(LineExpr);
            var after = new Regex(AfterExpr);
            var matchOps = new HashSet<int>[16];
            for (int i = 0; i < matchOps.Length; i++)
            {
                matchOps[i] = new HashSet<int>();
            }

            var moreThan3 = 0;
            for (int i = 0; i < lines.Length; i+=4)
            {
                var stateBefore = ExtractState(before.Match(lines[i]));
                var stateAfter = ExtractState(after.Match(lines[i + 2]));
                var (opcode, A, B, C) = ExtractOp(instructions.Match(lines[i+1]));

                var match = 0;
                for (var j = 0; j < ops.Length; j++)
                {
                    var check = ops[j].Invoke(stateBefore, A, B, C);
                    if (check.Equals(stateAfter))
                    {
                        match++;
                        if (match == 3)
                        {
                            moreThan3++;
                        }

                        matchOps[opcode].Add(j);
                    }
                }

            }
            Console.WriteLine($"Result 1: {moreThan3}");
            
            // match opCode to ops
            var translator = new Dictionary<int, int>();
            while (translator.Count<16)
            {
                for (var i = 0; i < matchOps.Length; i++)
                {
                    if (matchOps[i].Count == 1)
                    {
                        translator[i] = matchOps[i].First();
                        // removed use code
                        for (var j = 0; j < matchOps.Length; j++)
                        {
                            matchOps[j].Remove(translator[i]);
                        }
                    }
                }
            }

            foreach (var entry in translator)
            {
                Console.WriteLine($"{entry.Key} = {entry.Value}");
            }
            
            // execute program
            var state = new Registers(0, 0, 0, 0);
            var programLines = Input2.Split(Environment.NewLine);
            for (int i = 0; i < programLines.Length; i++)
            {
                var (code, A, B, C) = ExtractOp(instructions.Match(programLines[i]));
                state = ops[translator[code]].Invoke(state, A, B, C);
            }
            
            Console.WriteLine($"{state}");
        }

        private static (int, int, int, int) ExtractOp(Match result)
        {
            var code = int.Parse(result.Groups[1].Value);
            var A = int.Parse(result.Groups[2].Value);
            var B = int.Parse(result.Groups[3].Value);
            var C = int.Parse(result.Groups[4].Value);
            return (code, A, B, C);
        }

        private static Registers ExtractState(Match result)
        {
            var A = int.Parse(result.Groups[1].Value);
            var B = int.Parse(result.Groups[2].Value);
            var C = int.Parse(result.Groups[3].Value);
            var D = int.Parse(result.Groups[4].Value);
            return new Registers(A, B, C, D);
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
            return reg.RegisterOp(A, B, C, (a, _) => a);
        }
        
        private static Registers seti(Registers reg, int A, int B, int C)
        {
            return reg.ImmediateOp(B, A, C, (_, b) => b);
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
            private int[] Reg = new int[4];

            private bool Equals(Registers other)
            {
                return Reg[0] == other.Reg[0] &&
                       Reg[1] == other.Reg[1] &&
                       Reg[2] == other.Reg[2] &&
                       Reg[3] == other.Reg[3];
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
                return (Reg != null ? Reg.GetHashCode() : 0);
            }

            public Registers(int r0, int r1, int r2, int r3)
            {
                Reg[0] = r0;
                Reg[1] = r1;
                Reg[2] = r2;
                Reg[3] = r3;
            }

            private Registers CloneAndChange(int id, int value)
            {
                var result = new Registers(Reg[0], Reg[1], Reg[2], Reg[3]) {Reg = {[id] = value}};
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
                return $"{nameof(Reg)}: {Reg[0]},{Reg[1]},{Reg[2]},{Reg[3]}";
            }
        }

        private const string BeforeExpr = "Before: \\[(\\d+), (\\d+), (\\d+), (\\d+)\\]";
        private const string AfterExpr = "After:  \\[(\\d+), (\\d+), (\\d+), (\\d+)\\]";
        private const string LineExpr = "(\\d+) (\\d+) (\\d+) (\\d+)";
        private const string Input =
            @"Before: [1, 1, 0, 1]
0 1 0 1
After:  [1, 1, 0, 1]

Before: [2, 2, 2, 1]
2 1 2 2
After:  [2, 2, 1, 1]

Before: [1, 3, 2, 2]
1 2 2 0
After:  [4, 3, 2, 2]

Before: [2, 2, 1, 1]
8 0 2 1
After:  [2, 3, 1, 1]

Before: [0, 1, 2, 2]
7 3 1 1
After:  [0, 3, 2, 2]

Before: [1, 2, 2, 0]
8 2 0 1
After:  [1, 3, 2, 0]

Before: [2, 2, 2, 0]
2 1 2 0
After:  [1, 2, 2, 0]

Before: [0, 1, 1, 1]
3 1 0 1
After:  [0, 1, 1, 1]

Before: [2, 3, 2, 2]
15 1 2 2
After:  [2, 3, 6, 2]

Before: [0, 3, 1, 1]
9 0 0 3
After:  [0, 3, 1, 0]

Before: [2, 0, 1, 1]
4 2 3 2
After:  [2, 0, 0, 1]

Before: [1, 1, 3, 3]
6 0 3 3
After:  [1, 1, 3, 0]

Before: [3, 2, 3, 2]
11 1 3 3
After:  [3, 2, 3, 1]

Before: [3, 2, 1, 2]
14 3 3 1
After:  [3, 6, 1, 2]

Before: [1, 0, 1, 2]
10 1 0 1
After:  [1, 1, 1, 2]

Before: [1, 2, 0, 2]
11 1 3 2
After:  [1, 2, 1, 2]

Before: [3, 3, 0, 0]
14 0 3 2
After:  [3, 3, 9, 0]

Before: [2, 0, 2, 3]
6 0 3 1
After:  [2, 0, 2, 3]

Before: [3, 2, 2, 2]
11 1 3 1
After:  [3, 1, 2, 2]

Before: [2, 2, 0, 2]
13 1 2 1
After:  [2, 4, 0, 2]

Before: [2, 2, 2, 0]
8 3 1 3
After:  [2, 2, 2, 2]

Before: [2, 3, 1, 3]
6 0 3 2
After:  [2, 3, 0, 3]

Before: [3, 3, 1, 1]
14 1 3 1
After:  [3, 9, 1, 1]

Before: [2, 2, 1, 3]
6 0 3 2
After:  [2, 2, 0, 3]

Before: [2, 3, 2, 3]
15 3 3 0
After:  [9, 3, 2, 3]

Before: [1, 0, 0, 3]
10 1 0 1
After:  [1, 1, 0, 3]

Before: [1, 3, 1, 3]
15 1 3 3
After:  [1, 3, 1, 9]

Before: [1, 1, 2, 3]
15 1 2 3
After:  [1, 1, 2, 2]

Before: [1, 1, 3, 0]
0 1 0 0
After:  [1, 1, 3, 0]

Before: [1, 2, 3, 2]
13 3 2 2
After:  [1, 2, 4, 2]

Before: [0, 1, 1, 2]
9 0 0 0
After:  [0, 1, 1, 2]

Before: [2, 2, 0, 0]
12 1 1 3
After:  [2, 2, 0, 1]

Before: [1, 0, 2, 3]
6 0 3 2
After:  [1, 0, 0, 3]

Before: [1, 1, 0, 3]
6 0 3 3
After:  [1, 1, 0, 0]

Before: [0, 2, 0, 0]
9 0 0 2
After:  [0, 2, 0, 0]

Before: [0, 2, 2, 3]
5 0 3 0
After:  [3, 2, 2, 3]

Before: [1, 0, 2, 1]
5 1 2 3
After:  [1, 0, 2, 2]

Before: [1, 1, 3, 3]
0 1 0 2
After:  [1, 1, 1, 3]

Before: [2, 0, 3, 1]
12 2 3 0
After:  [0, 0, 3, 1]

Before: [0, 3, 2, 0]
5 0 2 0
After:  [2, 3, 2, 0]

Before: [2, 1, 2, 2]
1 2 2 0
After:  [4, 1, 2, 2]

Before: [1, 2, 1, 3]
6 0 3 2
After:  [1, 2, 0, 3]

Before: [0, 2, 3, 1]
14 1 3 2
After:  [0, 2, 6, 1]

Before: [1, 3, 3, 3]
5 0 3 2
After:  [1, 3, 3, 3]

Before: [3, 1, 0, 3]
14 3 2 3
After:  [3, 1, 0, 6]

Before: [2, 0, 3, 3]
12 3 2 0
After:  [1, 0, 3, 3]

Before: [1, 2, 1, 2]
11 1 3 3
After:  [1, 2, 1, 1]

Before: [2, 1, 2, 1]
1 0 2 1
After:  [2, 4, 2, 1]

Before: [0, 1, 1, 0]
3 1 0 1
After:  [0, 1, 1, 0]

Before: [0, 2, 3, 0]
9 0 0 2
After:  [0, 2, 0, 0]

Before: [3, 1, 2, 3]
15 2 3 3
After:  [3, 1, 2, 6]

Before: [0, 1, 0, 0]
3 1 0 0
After:  [1, 1, 0, 0]

Before: [0, 0, 1, 2]
5 2 3 2
After:  [0, 0, 3, 2]

Before: [0, 2, 2, 2]
8 0 3 1
After:  [0, 2, 2, 2]

Before: [1, 2, 0, 3]
15 0 1 0
After:  [2, 2, 0, 3]

Before: [2, 1, 0, 3]
6 0 3 2
After:  [2, 1, 0, 3]

Before: [0, 2, 3, 2]
11 1 3 1
After:  [0, 1, 3, 2]

Before: [1, 1, 2, 1]
0 1 0 1
After:  [1, 1, 2, 1]

Before: [2, 2, 2, 0]
12 1 1 1
After:  [2, 1, 2, 0]

Before: [3, 1, 2, 2]
1 2 2 2
After:  [3, 1, 4, 2]

Before: [0, 1, 3, 0]
3 1 0 3
After:  [0, 1, 3, 1]

Before: [3, 0, 2, 0]
1 2 2 2
After:  [3, 0, 4, 0]

Before: [2, 3, 3, 2]
15 1 3 2
After:  [2, 3, 6, 2]

Before: [3, 0, 2, 0]
1 2 2 0
After:  [4, 0, 2, 0]

Before: [3, 2, 3, 2]
12 1 1 0
After:  [1, 2, 3, 2]

Before: [2, 3, 3, 2]
13 0 2 0
After:  [4, 3, 3, 2]

Before: [1, 0, 2, 2]
10 1 0 0
After:  [1, 0, 2, 2]

Before: [2, 0, 0, 0]
8 1 0 1
After:  [2, 2, 0, 0]

Before: [0, 2, 2, 1]
12 2 1 0
After:  [1, 2, 2, 1]

Before: [0, 3, 2, 1]
9 0 0 3
After:  [0, 3, 2, 0]

Before: [2, 2, 2, 1]
1 0 2 1
After:  [2, 4, 2, 1]

Before: [0, 1, 2, 3]
3 1 0 2
After:  [0, 1, 1, 3]

Before: [1, 1, 0, 2]
0 1 0 0
After:  [1, 1, 0, 2]

Before: [3, 2, 1, 2]
11 1 3 0
After:  [1, 2, 1, 2]

Before: [3, 1, 3, 3]
8 1 0 0
After:  [3, 1, 3, 3]

Before: [3, 1, 2, 3]
15 0 2 2
After:  [3, 1, 6, 3]

Before: [0, 2, 2, 1]
2 1 2 3
After:  [0, 2, 2, 1]

Before: [1, 1, 0, 2]
0 1 0 3
After:  [1, 1, 0, 1]

Before: [2, 2, 3, 2]
11 1 3 0
After:  [1, 2, 3, 2]

Before: [1, 1, 3, 1]
0 1 0 2
After:  [1, 1, 1, 1]

Before: [1, 2, 3, 0]
8 3 1 0
After:  [2, 2, 3, 0]

Before: [1, 0, 2, 3]
10 1 0 0
After:  [1, 0, 2, 3]

Before: [1, 3, 3, 1]
12 1 2 1
After:  [1, 1, 3, 1]

Before: [0, 1, 2, 2]
3 1 0 0
After:  [1, 1, 2, 2]

Before: [2, 2, 2, 3]
6 0 3 2
After:  [2, 2, 0, 3]

Before: [3, 2, 3, 0]
13 1 2 0
After:  [4, 2, 3, 0]

Before: [0, 1, 0, 1]
9 0 0 2
After:  [0, 1, 0, 1]

Before: [2, 3, 3, 2]
13 3 2 1
After:  [2, 4, 3, 2]

Before: [2, 2, 1, 3]
4 1 2 2
After:  [2, 2, 1, 3]

Before: [1, 3, 0, 1]
8 0 1 2
After:  [1, 3, 3, 1]

Before: [0, 1, 2, 0]
3 1 0 1
After:  [0, 1, 2, 0]

Before: [0, 2, 0, 2]
11 1 3 1
After:  [0, 1, 0, 2]

Before: [3, 2, 3, 1]
14 2 3 3
After:  [3, 2, 3, 9]

Before: [0, 1, 1, 3]
3 1 0 0
After:  [1, 1, 1, 3]

Before: [2, 2, 2, 1]
2 1 2 0
After:  [1, 2, 2, 1]

Before: [0, 2, 1, 2]
4 3 3 3
After:  [0, 2, 1, 0]

Before: [0, 1, 1, 2]
7 3 1 3
After:  [0, 1, 1, 3]

Before: [1, 0, 1, 0]
10 1 0 0
After:  [1, 0, 1, 0]

Before: [3, 2, 2, 1]
12 1 1 3
After:  [3, 2, 2, 1]

Before: [0, 1, 0, 0]
9 0 0 2
After:  [0, 1, 0, 0]

Before: [2, 2, 2, 1]
2 1 2 1
After:  [2, 1, 2, 1]

Before: [1, 3, 0, 3]
2 1 3 0
After:  [1, 3, 0, 3]

Before: [1, 1, 2, 2]
0 1 0 2
After:  [1, 1, 1, 2]

Before: [0, 2, 0, 2]
11 1 3 3
After:  [0, 2, 0, 1]

Before: [0, 2, 3, 2]
11 1 3 0
After:  [1, 2, 3, 2]

Before: [2, 3, 2, 1]
7 3 2 0
After:  [3, 3, 2, 1]

Before: [2, 1, 1, 3]
14 3 2 0
After:  [6, 1, 1, 3]

Before: [1, 1, 3, 2]
0 1 0 0
After:  [1, 1, 3, 2]

Before: [2, 2, 2, 3]
1 0 2 2
After:  [2, 2, 4, 3]

Before: [1, 1, 0, 3]
0 1 0 3
After:  [1, 1, 0, 1]

Before: [1, 0, 3, 0]
10 1 0 2
After:  [1, 0, 1, 0]

Before: [0, 2, 0, 2]
11 1 3 2
After:  [0, 2, 1, 2]

Before: [1, 3, 2, 1]
7 3 2 0
After:  [3, 3, 2, 1]

Before: [2, 2, 1, 0]
14 0 3 2
After:  [2, 2, 6, 0]

Before: [0, 1, 0, 0]
5 0 1 3
After:  [0, 1, 0, 1]

Before: [0, 2, 2, 3]
15 2 3 0
After:  [6, 2, 2, 3]

Before: [3, 3, 0, 3]
15 0 3 0
After:  [9, 3, 0, 3]

Before: [0, 3, 3, 0]
14 2 3 0
After:  [9, 3, 3, 0]

Before: [0, 1, 1, 2]
7 3 1 2
After:  [0, 1, 3, 2]

Before: [1, 3, 1, 3]
2 1 3 0
After:  [1, 3, 1, 3]

Before: [1, 0, 3, 3]
10 1 0 2
After:  [1, 0, 1, 3]

Before: [0, 1, 3, 2]
3 1 0 3
After:  [0, 1, 3, 1]

Before: [0, 1, 3, 1]
3 1 0 2
After:  [0, 1, 1, 1]

Before: [1, 1, 2, 0]
0 1 0 1
After:  [1, 1, 2, 0]

Before: [0, 3, 0, 2]
15 1 3 3
After:  [0, 3, 0, 6]

Before: [0, 2, 1, 1]
4 1 2 2
After:  [0, 2, 1, 1]

Before: [0, 2, 2, 1]
4 3 3 0
After:  [0, 2, 2, 1]

Before: [1, 2, 1, 2]
8 1 0 1
After:  [1, 3, 1, 2]

Before: [1, 2, 2, 0]
15 0 2 1
After:  [1, 2, 2, 0]

Before: [2, 2, 3, 1]
4 3 3 1
After:  [2, 0, 3, 1]

Before: [1, 2, 1, 0]
8 3 1 3
After:  [1, 2, 1, 2]

Before: [3, 0, 2, 2]
8 1 0 2
After:  [3, 0, 3, 2]

Before: [0, 0, 2, 0]
9 0 0 2
After:  [0, 0, 0, 0]

Before: [1, 2, 2, 3]
4 1 0 2
After:  [1, 2, 1, 3]

Before: [2, 1, 2, 2]
1 3 2 1
After:  [2, 4, 2, 2]

Before: [2, 3, 0, 3]
2 1 3 1
After:  [2, 1, 0, 3]

Before: [1, 2, 0, 2]
11 1 3 1
After:  [1, 1, 0, 2]

Before: [3, 1, 0, 2]
7 3 1 1
After:  [3, 3, 0, 2]

Before: [0, 2, 2, 3]
1 1 2 3
After:  [0, 2, 2, 4]

Before: [1, 2, 3, 3]
6 0 3 0
After:  [0, 2, 3, 3]

Before: [2, 0, 2, 1]
7 3 2 0
After:  [3, 0, 2, 1]

Before: [0, 2, 2, 2]
8 0 3 2
After:  [0, 2, 2, 2]

Before: [1, 0, 3, 1]
10 1 0 2
After:  [1, 0, 1, 1]

Before: [0, 1, 2, 1]
4 3 3 2
After:  [0, 1, 0, 1]

Before: [0, 2, 2, 0]
2 1 2 3
After:  [0, 2, 2, 1]

Before: [0, 0, 2, 2]
5 1 2 0
After:  [2, 0, 2, 2]

Before: [1, 0, 0, 3]
10 1 0 3
After:  [1, 0, 0, 1]

Before: [1, 0, 3, 1]
10 1 0 3
After:  [1, 0, 3, 1]

Before: [0, 0, 3, 2]
9 0 0 3
After:  [0, 0, 3, 0]

Before: [1, 0, 1, 3]
10 1 0 2
After:  [1, 0, 1, 3]

Before: [3, 2, 3, 2]
4 3 3 2
After:  [3, 2, 0, 2]

Before: [0, 1, 1, 3]
3 1 0 2
After:  [0, 1, 1, 3]

Before: [1, 2, 3, 3]
4 1 0 1
After:  [1, 1, 3, 3]

Before: [0, 0, 0, 2]
9 0 0 1
After:  [0, 0, 0, 2]

Before: [1, 0, 2, 1]
10 1 0 1
After:  [1, 1, 2, 1]

Before: [1, 2, 3, 2]
5 0 3 2
After:  [1, 2, 3, 2]

Before: [2, 2, 3, 2]
12 1 1 0
After:  [1, 2, 3, 2]

Before: [2, 2, 2, 2]
12 2 1 2
After:  [2, 2, 1, 2]

Before: [2, 2, 2, 3]
1 2 2 1
After:  [2, 4, 2, 3]

Before: [3, 1, 3, 2]
7 3 1 2
After:  [3, 1, 3, 2]

Before: [2, 2, 0, 0]
13 0 2 1
After:  [2, 4, 0, 0]

Before: [0, 2, 1, 3]
15 1 3 1
After:  [0, 6, 1, 3]

Before: [0, 1, 1, 2]
3 1 0 0
After:  [1, 1, 1, 2]

Before: [2, 1, 0, 0]
8 3 0 1
After:  [2, 2, 0, 0]

Before: [0, 1, 3, 3]
15 2 3 0
After:  [9, 1, 3, 3]

Before: [0, 1, 1, 2]
3 1 0 1
After:  [0, 1, 1, 2]

Before: [0, 1, 1, 1]
8 0 2 0
After:  [1, 1, 1, 1]

Before: [1, 2, 2, 3]
12 1 1 3
After:  [1, 2, 2, 1]

Before: [3, 3, 1, 2]
12 1 0 1
After:  [3, 1, 1, 2]

Before: [3, 0, 0, 3]
14 3 2 2
After:  [3, 0, 6, 3]

Before: [3, 3, 3, 1]
12 2 3 3
After:  [3, 3, 3, 0]

Before: [2, 3, 2, 0]
14 2 3 1
After:  [2, 6, 2, 0]

Before: [2, 3, 0, 2]
15 1 3 2
After:  [2, 3, 6, 2]

Before: [3, 2, 3, 2]
11 1 3 1
After:  [3, 1, 3, 2]

Before: [1, 2, 2, 1]
15 3 2 1
After:  [1, 2, 2, 1]

Before: [1, 2, 2, 1]
5 0 2 1
After:  [1, 3, 2, 1]

Before: [1, 2, 3, 3]
6 0 3 2
After:  [1, 2, 0, 3]

Before: [1, 2, 2, 0]
5 0 2 2
After:  [1, 2, 3, 0]

Before: [3, 0, 0, 2]
4 3 3 1
After:  [3, 0, 0, 2]

Before: [1, 2, 1, 3]
8 1 0 2
After:  [1, 2, 3, 3]

Before: [2, 0, 2, 1]
1 2 2 1
After:  [2, 4, 2, 1]

Before: [2, 1, 2, 3]
1 2 2 3
After:  [2, 1, 2, 4]

Before: [1, 1, 0, 1]
14 3 2 1
After:  [1, 2, 0, 1]

Before: [1, 0, 2, 1]
1 2 2 1
After:  [1, 4, 2, 1]

Before: [2, 2, 0, 3]
15 0 3 3
After:  [2, 2, 0, 6]

Before: [3, 0, 2, 2]
1 3 2 0
After:  [4, 0, 2, 2]

Before: [2, 2, 3, 2]
13 3 2 1
After:  [2, 4, 3, 2]

Before: [1, 1, 2, 1]
0 1 0 2
After:  [1, 1, 1, 1]

Before: [0, 2, 3, 2]
11 1 3 2
After:  [0, 2, 1, 2]

Before: [2, 1, 1, 2]
7 3 1 0
After:  [3, 1, 1, 2]

Before: [3, 0, 2, 0]
14 2 3 3
After:  [3, 0, 2, 6]

Before: [2, 2, 0, 2]
14 0 3 3
After:  [2, 2, 0, 6]

Before: [0, 0, 3, 2]
9 0 0 0
After:  [0, 0, 3, 2]

Before: [1, 1, 3, 1]
0 1 0 1
After:  [1, 1, 3, 1]

Before: [1, 1, 1, 1]
0 1 0 1
After:  [1, 1, 1, 1]

Before: [1, 0, 2, 0]
10 1 0 1
After:  [1, 1, 2, 0]

Before: [1, 1, 0, 3]
0 1 0 0
After:  [1, 1, 0, 3]

Before: [0, 1, 0, 2]
13 3 2 1
After:  [0, 4, 0, 2]

Before: [2, 1, 0, 0]
14 1 2 3
After:  [2, 1, 0, 2]

Before: [3, 3, 2, 2]
4 3 3 1
After:  [3, 0, 2, 2]

Before: [0, 2, 0, 0]
12 1 1 3
After:  [0, 2, 0, 1]

Before: [1, 0, 2, 1]
1 2 2 2
After:  [1, 0, 4, 1]

Before: [0, 1, 2, 1]
9 0 0 2
After:  [0, 1, 0, 1]

Before: [1, 1, 3, 2]
0 1 0 2
After:  [1, 1, 1, 2]

Before: [3, 0, 3, 2]
15 0 3 0
After:  [6, 0, 3, 2]

Before: [2, 1, 2, 0]
5 3 2 0
After:  [2, 1, 2, 0]

Before: [3, 2, 2, 2]
1 2 2 2
After:  [3, 2, 4, 2]

Before: [1, 1, 0, 0]
0 1 0 3
After:  [1, 1, 0, 1]

Before: [1, 2, 0, 3]
12 1 1 2
After:  [1, 2, 1, 3]

Before: [2, 2, 2, 0]
1 0 2 1
After:  [2, 4, 2, 0]

Before: [0, 2, 1, 3]
8 0 2 2
After:  [0, 2, 1, 3]

Before: [3, 2, 0, 0]
14 1 3 1
After:  [3, 6, 0, 0]

Before: [0, 2, 2, 3]
15 3 3 3
After:  [0, 2, 2, 9]

Before: [1, 0, 3, 2]
10 1 0 0
After:  [1, 0, 3, 2]

Before: [1, 2, 2, 0]
2 1 2 2
After:  [1, 2, 1, 0]

Before: [0, 0, 3, 3]
9 0 0 1
After:  [0, 0, 3, 3]

Before: [0, 1, 2, 3]
3 1 0 1
After:  [0, 1, 2, 3]

Before: [1, 0, 1, 1]
10 1 0 3
After:  [1, 0, 1, 1]

Before: [2, 0, 2, 3]
6 0 3 0
After:  [0, 0, 2, 3]

Before: [0, 2, 3, 0]
13 1 2 1
After:  [0, 4, 3, 0]

Before: [0, 1, 3, 2]
3 1 0 1
After:  [0, 1, 3, 2]

Before: [1, 1, 0, 3]
6 0 3 0
After:  [0, 1, 0, 3]

Before: [3, 2, 0, 1]
13 1 2 0
After:  [4, 2, 0, 1]

Before: [2, 1, 3, 1]
5 0 1 2
After:  [2, 1, 3, 1]

Before: [1, 1, 3, 2]
0 1 0 1
After:  [1, 1, 3, 2]

Before: [2, 0, 0, 3]
6 0 3 0
After:  [0, 0, 0, 3]

Before: [1, 2, 1, 0]
13 1 2 1
After:  [1, 4, 1, 0]

Before: [2, 2, 1, 2]
11 1 3 3
After:  [2, 2, 1, 1]

Before: [0, 1, 2, 3]
5 1 3 3
After:  [0, 1, 2, 3]

Before: [2, 0, 3, 3]
6 0 3 2
After:  [2, 0, 0, 3]

Before: [2, 2, 1, 0]
12 1 0 0
After:  [1, 2, 1, 0]

Before: [2, 2, 2, 2]
11 1 3 0
After:  [1, 2, 2, 2]

Before: [1, 1, 0, 1]
0 1 0 0
After:  [1, 1, 0, 1]

Before: [0, 2, 2, 2]
11 1 3 0
After:  [1, 2, 2, 2]

Before: [2, 2, 0, 2]
11 1 3 3
After:  [2, 2, 0, 1]

Before: [2, 2, 2, 0]
2 1 2 2
After:  [2, 2, 1, 0]

Before: [3, 1, 1, 2]
7 3 1 3
After:  [3, 1, 1, 3]

Before: [0, 2, 1, 3]
9 0 0 0
After:  [0, 2, 1, 3]

Before: [0, 2, 2, 2]
11 1 3 3
After:  [0, 2, 2, 1]

Before: [3, 3, 3, 1]
7 3 2 2
After:  [3, 3, 3, 1]

Before: [2, 0, 3, 3]
6 0 3 1
After:  [2, 0, 3, 3]

Before: [0, 0, 2, 3]
5 1 3 1
After:  [0, 3, 2, 3]

Before: [3, 3, 2, 0]
5 3 2 2
After:  [3, 3, 2, 0]

Before: [3, 1, 2, 0]
14 2 3 3
After:  [3, 1, 2, 6]

Before: [1, 1, 2, 0]
1 2 2 2
After:  [1, 1, 4, 0]

Before: [1, 3, 1, 1]
4 2 3 1
After:  [1, 0, 1, 1]

Before: [1, 1, 2, 3]
0 1 0 1
After:  [1, 1, 2, 3]

Before: [3, 3, 3, 2]
15 2 3 0
After:  [6, 3, 3, 2]

Before: [1, 1, 0, 1]
0 1 0 3
After:  [1, 1, 0, 1]

Before: [2, 1, 1, 1]
13 0 2 1
After:  [2, 4, 1, 1]

Before: [3, 3, 2, 1]
1 2 2 1
After:  [3, 4, 2, 1]

Before: [0, 2, 2, 3]
2 1 2 3
After:  [0, 2, 2, 1]

Before: [2, 0, 1, 3]
6 0 3 2
After:  [2, 0, 0, 3]

Before: [1, 3, 3, 3]
6 0 3 2
After:  [1, 3, 0, 3]

Before: [0, 1, 0, 2]
7 3 1 3
After:  [0, 1, 0, 3]

Before: [0, 2, 2, 1]
12 2 1 2
After:  [0, 2, 1, 1]

Before: [1, 3, 2, 3]
6 0 3 1
After:  [1, 0, 2, 3]

Before: [1, 3, 1, 3]
15 1 3 2
After:  [1, 3, 9, 3]

Before: [0, 3, 2, 2]
1 2 2 1
After:  [0, 4, 2, 2]

Before: [3, 3, 2, 3]
2 1 3 2
After:  [3, 3, 1, 3]

Before: [0, 0, 2, 3]
5 0 2 0
After:  [2, 0, 2, 3]

Before: [3, 0, 2, 3]
15 2 3 0
After:  [6, 0, 2, 3]

Before: [2, 3, 0, 3]
14 3 2 1
After:  [2, 6, 0, 3]

Before: [2, 0, 2, 1]
1 0 2 2
After:  [2, 0, 4, 1]

Before: [0, 1, 3, 1]
9 0 0 2
After:  [0, 1, 0, 1]

Before: [1, 2, 2, 2]
2 1 2 3
After:  [1, 2, 2, 1]

Before: [0, 0, 3, 1]
9 0 0 0
After:  [0, 0, 3, 1]

Before: [0, 1, 3, 3]
3 1 0 2
After:  [0, 1, 1, 3]

Before: [2, 1, 2, 3]
12 2 0 0
After:  [1, 1, 2, 3]

Before: [1, 2, 3, 2]
11 1 3 2
After:  [1, 2, 1, 2]

Before: [3, 2, 0, 2]
11 1 3 3
After:  [3, 2, 0, 1]

Before: [2, 1, 2, 2]
12 2 0 1
After:  [2, 1, 2, 2]

Before: [0, 1, 0, 2]
13 3 2 2
After:  [0, 1, 4, 2]

Before: [0, 1, 1, 0]
9 0 0 3
After:  [0, 1, 1, 0]

Before: [2, 1, 1, 3]
5 1 3 1
After:  [2, 3, 1, 3]

Before: [0, 2, 2, 2]
1 2 2 1
After:  [0, 4, 2, 2]

Before: [0, 1, 1, 2]
5 0 1 2
After:  [0, 1, 1, 2]

Before: [1, 1, 1, 2]
0 1 0 1
After:  [1, 1, 1, 2]

Before: [3, 3, 1, 2]
12 1 0 0
After:  [1, 3, 1, 2]

Before: [3, 2, 1, 0]
14 0 3 0
After:  [9, 2, 1, 0]

Before: [2, 2, 1, 1]
15 3 1 0
After:  [2, 2, 1, 1]

Before: [1, 3, 2, 1]
7 3 2 2
After:  [1, 3, 3, 1]

Before: [0, 2, 0, 0]
9 0 0 3
After:  [0, 2, 0, 0]

Before: [1, 2, 1, 3]
4 1 0 1
After:  [1, 1, 1, 3]

Before: [1, 1, 3, 2]
15 2 3 1
After:  [1, 6, 3, 2]

Before: [2, 3, 3, 1]
7 3 2 0
After:  [3, 3, 3, 1]

Before: [0, 1, 2, 2]
5 0 1 0
After:  [1, 1, 2, 2]

Before: [3, 1, 0, 2]
7 3 1 0
After:  [3, 1, 0, 2]

Before: [1, 3, 3, 3]
15 2 3 3
After:  [1, 3, 3, 9]

Before: [3, 2, 2, 3]
15 3 3 1
After:  [3, 9, 2, 3]

Before: [0, 0, 0, 2]
9 0 0 2
After:  [0, 0, 0, 2]

Before: [2, 1, 3, 0]
5 1 2 3
After:  [2, 1, 3, 3]

Before: [3, 0, 2, 1]
1 2 2 0
After:  [4, 0, 2, 1]

Before: [1, 0, 2, 3]
10 1 0 3
After:  [1, 0, 2, 1]

Before: [0, 3, 1, 2]
14 3 3 2
After:  [0, 3, 6, 2]

Before: [0, 3, 3, 3]
2 1 3 2
After:  [0, 3, 1, 3]

Before: [0, 2, 2, 0]
5 3 2 3
After:  [0, 2, 2, 2]

Before: [1, 1, 2, 1]
5 2 1 0
After:  [3, 1, 2, 1]

Before: [1, 2, 0, 2]
11 1 3 3
After:  [1, 2, 0, 1]

Before: [3, 0, 2, 1]
7 3 2 3
After:  [3, 0, 2, 3]

Before: [0, 2, 1, 3]
9 0 0 2
After:  [0, 2, 0, 3]

Before: [2, 2, 2, 2]
11 1 3 3
After:  [2, 2, 2, 1]

Before: [1, 3, 2, 3]
1 2 2 0
After:  [4, 3, 2, 3]

Before: [1, 3, 1, 3]
2 1 3 2
After:  [1, 3, 1, 3]

Before: [0, 1, 2, 3]
5 0 3 1
After:  [0, 3, 2, 3]

Before: [2, 2, 1, 2]
11 1 3 1
After:  [2, 1, 1, 2]

Before: [1, 0, 3, 1]
7 3 2 0
After:  [3, 0, 3, 1]

Before: [1, 2, 1, 3]
6 0 3 0
After:  [0, 2, 1, 3]

Before: [3, 2, 2, 2]
2 1 2 0
After:  [1, 2, 2, 2]

Before: [3, 0, 0, 3]
5 1 3 0
After:  [3, 0, 0, 3]

Before: [2, 3, 3, 3]
6 0 3 2
After:  [2, 3, 0, 3]

Before: [1, 2, 3, 2]
11 1 3 0
After:  [1, 2, 3, 2]

Before: [2, 3, 3, 3]
6 0 3 1
After:  [2, 0, 3, 3]

Before: [2, 1, 0, 3]
6 0 3 0
After:  [0, 1, 0, 3]

Before: [3, 2, 3, 1]
4 3 3 1
After:  [3, 0, 3, 1]

Before: [1, 2, 2, 3]
1 2 2 1
After:  [1, 4, 2, 3]

Before: [0, 2, 0, 2]
12 1 1 2
After:  [0, 2, 1, 2]

Before: [3, 3, 1, 3]
2 1 3 3
After:  [3, 3, 1, 1]

Before: [1, 1, 2, 2]
5 1 2 2
After:  [1, 1, 3, 2]

Before: [0, 0, 2, 2]
5 0 2 3
After:  [0, 0, 2, 2]

Before: [0, 1, 3, 1]
9 0 0 0
After:  [0, 1, 3, 1]

Before: [0, 1, 0, 0]
3 1 0 1
After:  [0, 1, 0, 0]

Before: [1, 0, 0, 1]
10 1 0 1
After:  [1, 1, 0, 1]

Before: [1, 1, 1, 3]
0 1 0 0
After:  [1, 1, 1, 3]

Before: [0, 2, 2, 2]
4 3 3 1
After:  [0, 0, 2, 2]

Before: [1, 2, 1, 0]
8 2 1 0
After:  [3, 2, 1, 0]

Before: [0, 1, 2, 1]
3 1 0 3
After:  [0, 1, 2, 1]

Before: [1, 0, 2, 1]
10 1 0 3
After:  [1, 0, 2, 1]

Before: [1, 1, 3, 1]
5 0 2 1
After:  [1, 3, 3, 1]

Before: [2, 3, 2, 1]
4 3 3 2
After:  [2, 3, 0, 1]

Before: [3, 3, 0, 3]
12 1 0 1
After:  [3, 1, 0, 3]

Before: [3, 3, 1, 3]
8 2 0 3
After:  [3, 3, 1, 3]

Before: [1, 3, 3, 1]
7 3 2 2
After:  [1, 3, 3, 1]

Before: [3, 0, 3, 1]
4 3 3 0
After:  [0, 0, 3, 1]

Before: [1, 3, 2, 3]
15 3 2 3
After:  [1, 3, 2, 6]

Before: [3, 2, 2, 0]
2 1 2 2
After:  [3, 2, 1, 0]

Before: [3, 1, 2, 1]
4 3 3 1
After:  [3, 0, 2, 1]

Before: [2, 3, 1, 3]
2 1 3 3
After:  [2, 3, 1, 1]

Before: [2, 2, 0, 3]
6 0 3 1
After:  [2, 0, 0, 3]

Before: [2, 3, 2, 2]
1 0 2 0
After:  [4, 3, 2, 2]

Before: [0, 1, 3, 3]
5 0 3 3
After:  [0, 1, 3, 3]

Before: [2, 0, 3, 3]
12 3 2 2
After:  [2, 0, 1, 3]

Before: [1, 1, 3, 2]
13 3 2 2
After:  [1, 1, 4, 2]

Before: [1, 3, 3, 3]
2 1 3 2
After:  [1, 3, 1, 3]

Before: [3, 2, 2, 2]
14 0 3 1
After:  [3, 9, 2, 2]

Before: [1, 0, 3, 2]
8 1 3 2
After:  [1, 0, 2, 2]

Before: [0, 3, 3, 3]
9 0 0 2
After:  [0, 3, 0, 3]

Before: [3, 2, 0, 1]
4 3 3 0
After:  [0, 2, 0, 1]

Before: [3, 0, 3, 2]
4 3 3 0
After:  [0, 0, 3, 2]

Before: [3, 0, 3, 2]
8 1 2 1
After:  [3, 3, 3, 2]

Before: [0, 2, 1, 1]
8 0 3 0
After:  [1, 2, 1, 1]

Before: [2, 2, 2, 0]
1 1 2 1
After:  [2, 4, 2, 0]

Before: [2, 2, 1, 3]
13 0 2 1
After:  [2, 4, 1, 3]

Before: [0, 1, 3, 1]
3 1 0 0
After:  [1, 1, 3, 1]

Before: [0, 1, 2, 2]
3 1 0 3
After:  [0, 1, 2, 1]

Before: [1, 2, 2, 3]
6 0 3 1
After:  [1, 0, 2, 3]

Before: [3, 2, 0, 3]
12 1 1 2
After:  [3, 2, 1, 3]

Before: [2, 3, 1, 1]
13 0 2 0
After:  [4, 3, 1, 1]

Before: [2, 1, 1, 0]
5 0 1 1
After:  [2, 3, 1, 0]

Before: [3, 3, 2, 3]
15 3 2 3
After:  [3, 3, 2, 6]

Before: [0, 1, 0, 3]
3 1 0 3
After:  [0, 1, 0, 1]

Before: [1, 0, 3, 2]
14 2 3 1
After:  [1, 9, 3, 2]

Before: [0, 2, 3, 1]
7 3 2 0
After:  [3, 2, 3, 1]

Before: [2, 3, 0, 2]
14 1 2 2
After:  [2, 3, 6, 2]

Before: [2, 2, 3, 1]
7 3 2 0
After:  [3, 2, 3, 1]

Before: [0, 3, 3, 3]
12 3 2 2
After:  [0, 3, 1, 3]

Before: [0, 2, 2, 2]
9 0 0 2
After:  [0, 2, 0, 2]

Before: [0, 3, 1, 0]
8 0 1 2
After:  [0, 3, 3, 0]

Before: [0, 3, 3, 0]
9 0 0 0
After:  [0, 3, 3, 0]

Before: [0, 3, 0, 3]
15 3 3 1
After:  [0, 9, 0, 3]

Before: [2, 1, 1, 3]
6 0 3 2
After:  [2, 1, 0, 3]

Before: [2, 0, 0, 3]
15 3 3 1
After:  [2, 9, 0, 3]

Before: [0, 1, 0, 3]
5 0 3 2
After:  [0, 1, 3, 3]

Before: [1, 2, 2, 1]
4 1 0 0
After:  [1, 2, 2, 1]

Before: [0, 2, 3, 2]
15 2 3 0
After:  [6, 2, 3, 2]

Before: [0, 2, 2, 3]
9 0 0 1
After:  [0, 0, 2, 3]

Before: [3, 3, 3, 3]
2 1 3 1
After:  [3, 1, 3, 3]

Before: [1, 0, 2, 3]
10 1 0 1
After:  [1, 1, 2, 3]

Before: [2, 1, 1, 3]
6 0 3 1
After:  [2, 0, 1, 3]

Before: [0, 2, 2, 3]
1 2 2 1
After:  [0, 4, 2, 3]

Before: [1, 1, 1, 1]
0 1 0 2
After:  [1, 1, 1, 1]

Before: [0, 0, 2, 0]
1 2 2 1
After:  [0, 4, 2, 0]

Before: [1, 3, 0, 2]
13 3 2 3
After:  [1, 3, 0, 4]

Before: [1, 1, 0, 0]
0 1 0 2
After:  [1, 1, 1, 0]

Before: [1, 1, 1, 3]
0 1 0 2
After:  [1, 1, 1, 3]

Before: [1, 0, 3, 1]
10 1 0 0
After:  [1, 0, 3, 1]

Before: [2, 2, 1, 2]
11 1 3 0
After:  [1, 2, 1, 2]

Before: [0, 1, 0, 1]
9 0 0 3
After:  [0, 1, 0, 0]

Before: [0, 2, 1, 3]
5 2 3 2
After:  [0, 2, 3, 3]

Before: [1, 2, 2, 0]
1 1 2 3
After:  [1, 2, 2, 4]

Before: [1, 1, 0, 1]
0 1 0 2
After:  [1, 1, 1, 1]

Before: [0, 1, 2, 3]
3 1 0 0
After:  [1, 1, 2, 3]

Before: [0, 0, 2, 1]
14 2 3 1
After:  [0, 6, 2, 1]

Before: [2, 1, 2, 2]
7 3 1 3
After:  [2, 1, 2, 3]

Before: [1, 3, 3, 2]
12 1 2 0
After:  [1, 3, 3, 2]

Before: [3, 2, 2, 3]
1 2 2 2
After:  [3, 2, 4, 3]

Before: [0, 3, 2, 1]
8 0 3 2
After:  [0, 3, 1, 1]

Before: [1, 1, 0, 3]
0 1 0 2
After:  [1, 1, 1, 3]

Before: [3, 2, 1, 2]
11 1 3 2
After:  [3, 2, 1, 2]

Before: [0, 2, 1, 2]
11 1 3 1
After:  [0, 1, 1, 2]

Before: [1, 2, 1, 3]
6 0 3 1
After:  [1, 0, 1, 3]

Before: [2, 2, 2, 3]
2 1 2 2
After:  [2, 2, 1, 3]

Before: [3, 2, 2, 1]
1 1 2 1
After:  [3, 4, 2, 1]

Before: [1, 0, 0, 1]
10 1 0 3
After:  [1, 0, 0, 1]

Before: [0, 1, 0, 1]
3 1 0 3
After:  [0, 1, 0, 1]

Before: [1, 1, 1, 0]
0 1 0 3
After:  [1, 1, 1, 1]

Before: [1, 1, 1, 2]
7 3 1 3
After:  [1, 1, 1, 3]

Before: [3, 3, 1, 3]
2 1 3 0
After:  [1, 3, 1, 3]

Before: [1, 0, 0, 2]
10 1 0 0
After:  [1, 0, 0, 2]

Before: [0, 2, 1, 3]
13 1 2 3
After:  [0, 2, 1, 4]

Before: [1, 0, 2, 0]
10 1 0 3
After:  [1, 0, 2, 1]

Before: [2, 3, 3, 0]
13 0 2 3
After:  [2, 3, 3, 4]

Before: [3, 1, 2, 3]
5 1 3 3
After:  [3, 1, 2, 3]

Before: [1, 1, 1, 3]
0 1 0 1
After:  [1, 1, 1, 3]

Before: [2, 1, 2, 1]
7 3 2 3
After:  [2, 1, 2, 3]

Before: [1, 0, 0, 0]
10 1 0 0
After:  [1, 0, 0, 0]

Before: [1, 1, 0, 2]
0 1 0 1
After:  [1, 1, 0, 2]

Before: [3, 1, 2, 2]
1 2 2 0
After:  [4, 1, 2, 2]

Before: [1, 1, 1, 2]
0 1 0 3
After:  [1, 1, 1, 1]

Before: [1, 2, 3, 2]
11 1 3 1
After:  [1, 1, 3, 2]

Before: [3, 0, 2, 1]
1 2 2 3
After:  [3, 0, 2, 4]

Before: [0, 1, 0, 2]
3 1 0 1
After:  [0, 1, 0, 2]

Before: [1, 0, 3, 3]
10 1 0 3
After:  [1, 0, 3, 1]

Before: [1, 3, 1, 2]
5 0 3 2
After:  [1, 3, 3, 2]

Before: [1, 0, 0, 2]
10 1 0 2
After:  [1, 0, 1, 2]

Before: [1, 1, 1, 3]
6 0 3 3
After:  [1, 1, 1, 0]

Before: [1, 2, 0, 2]
11 1 3 0
After:  [1, 2, 0, 2]

Before: [1, 2, 1, 2]
11 1 3 1
After:  [1, 1, 1, 2]

Before: [0, 0, 2, 0]
9 0 0 3
After:  [0, 0, 2, 0]

Before: [3, 2, 2, 2]
1 3 2 2
After:  [3, 2, 4, 2]

Before: [0, 1, 3, 2]
3 1 0 2
After:  [0, 1, 1, 2]

Before: [2, 2, 1, 2]
11 1 3 2
After:  [2, 2, 1, 2]

Before: [1, 3, 0, 2]
13 3 2 0
After:  [4, 3, 0, 2]

Before: [2, 1, 2, 3]
6 0 3 2
After:  [2, 1, 0, 3]

Before: [0, 2, 1, 2]
11 1 3 2
After:  [0, 2, 1, 2]

Before: [2, 2, 2, 2]
2 1 2 1
After:  [2, 1, 2, 2]

Before: [3, 0, 2, 2]
1 2 2 3
After:  [3, 0, 2, 4]

Before: [0, 0, 3, 2]
13 3 2 0
After:  [4, 0, 3, 2]

Before: [1, 2, 1, 1]
4 1 0 1
After:  [1, 1, 1, 1]

Before: [1, 0, 2, 1]
10 1 0 0
After:  [1, 0, 2, 1]

Before: [2, 0, 2, 3]
6 0 3 3
After:  [2, 0, 2, 0]

Before: [1, 2, 3, 1]
4 3 3 3
After:  [1, 2, 3, 0]

Before: [1, 1, 1, 2]
5 2 3 2
After:  [1, 1, 3, 2]

Before: [1, 2, 3, 0]
14 1 3 1
After:  [1, 6, 3, 0]

Before: [0, 0, 1, 3]
5 2 3 0
After:  [3, 0, 1, 3]

Before: [0, 0, 0, 1]
9 0 0 0
After:  [0, 0, 0, 1]

Before: [2, 1, 3, 1]
5 0 1 3
After:  [2, 1, 3, 3]

Before: [2, 1, 2, 3]
6 0 3 3
After:  [2, 1, 2, 0]

Before: [2, 3, 1, 3]
2 1 3 1
After:  [2, 1, 1, 3]

Before: [1, 2, 1, 2]
4 3 3 3
After:  [1, 2, 1, 0]

Before: [2, 2, 3, 2]
11 1 3 1
After:  [2, 1, 3, 2]

Before: [2, 3, 3, 3]
13 0 2 1
After:  [2, 4, 3, 3]

Before: [1, 3, 2, 3]
6 0 3 2
After:  [1, 3, 0, 3]

Before: [2, 2, 1, 1]
4 1 2 0
After:  [1, 2, 1, 1]

Before: [1, 0, 3, 3]
5 1 3 2
After:  [1, 0, 3, 3]

Before: [3, 3, 2, 1]
7 3 2 1
After:  [3, 3, 2, 1]

Before: [1, 0, 2, 2]
10 1 0 1
After:  [1, 1, 2, 2]

Before: [2, 1, 1, 3]
13 0 2 0
After:  [4, 1, 1, 3]

Before: [1, 1, 0, 0]
0 1 0 0
After:  [1, 1, 0, 0]

Before: [2, 2, 2, 0]
2 1 2 1
After:  [2, 1, 2, 0]

Before: [0, 1, 1, 0]
3 1 0 2
After:  [0, 1, 1, 0]

Before: [1, 1, 2, 2]
1 3 2 3
After:  [1, 1, 2, 4]

Before: [2, 2, 1, 2]
13 3 2 0
After:  [4, 2, 1, 2]

Before: [0, 1, 0, 0]
9 0 0 1
After:  [0, 0, 0, 0]

Before: [1, 3, 1, 1]
14 1 2 2
After:  [1, 3, 6, 1]

Before: [1, 3, 0, 3]
6 0 3 1
After:  [1, 0, 0, 3]

Before: [1, 2, 2, 2]
11 1 3 1
After:  [1, 1, 2, 2]

Before: [1, 2, 0, 1]
12 1 1 2
After:  [1, 2, 1, 1]

Before: [3, 3, 2, 2]
4 3 3 0
After:  [0, 3, 2, 2]

Before: [0, 0, 0, 2]
4 3 3 3
After:  [0, 0, 0, 0]

Before: [1, 1, 2, 0]
0 1 0 2
After:  [1, 1, 1, 0]

Before: [3, 0, 2, 1]
7 3 2 2
After:  [3, 0, 3, 1]

Before: [3, 1, 2, 1]
7 3 2 1
After:  [3, 3, 2, 1]

Before: [0, 0, 2, 2]
1 3 2 2
After:  [0, 0, 4, 2]

Before: [0, 1, 3, 1]
3 1 0 3
After:  [0, 1, 3, 1]

Before: [1, 3, 2, 2]
14 3 3 3
After:  [1, 3, 2, 6]

Before: [1, 3, 3, 0]
8 0 1 0
After:  [3, 3, 3, 0]

Before: [1, 3, 2, 3]
1 2 2 1
After:  [1, 4, 2, 3]

Before: [0, 1, 3, 2]
7 3 1 3
After:  [0, 1, 3, 3]

Before: [3, 3, 2, 2]
15 1 2 2
After:  [3, 3, 6, 2]

Before: [0, 3, 3, 1]
7 3 2 3
After:  [0, 3, 3, 3]

Before: [2, 3, 3, 3]
2 1 3 1
After:  [2, 1, 3, 3]

Before: [1, 1, 3, 1]
14 0 2 2
After:  [1, 1, 2, 1]

Before: [2, 1, 0, 3]
8 1 0 2
After:  [2, 1, 3, 3]

Before: [1, 2, 2, 0]
2 1 2 0
After:  [1, 2, 2, 0]

Before: [1, 0, 0, 0]
10 1 0 2
After:  [1, 0, 1, 0]

Before: [1, 3, 3, 3]
2 1 3 3
After:  [1, 3, 3, 1]

Before: [0, 2, 2, 1]
1 1 2 3
After:  [0, 2, 2, 4]

Before: [1, 2, 2, 3]
6 0 3 3
After:  [1, 2, 2, 0]

Before: [3, 2, 0, 2]
11 1 3 2
After:  [3, 2, 1, 2]

Before: [2, 2, 2, 2]
1 2 2 0
After:  [4, 2, 2, 2]

Before: [1, 0, 0, 1]
4 3 3 1
After:  [1, 0, 0, 1]

Before: [3, 1, 1, 2]
4 3 3 1
After:  [3, 0, 1, 2]

Before: [1, 1, 2, 3]
6 0 3 3
After:  [1, 1, 2, 0]

Before: [0, 3, 1, 3]
9 0 0 2
After:  [0, 3, 0, 3]

Before: [1, 3, 0, 0]
8 2 1 0
After:  [3, 3, 0, 0]

Before: [3, 2, 0, 2]
13 1 2 3
After:  [3, 2, 0, 4]

Before: [0, 0, 1, 2]
13 3 2 2
After:  [0, 0, 4, 2]

Before: [0, 2, 0, 2]
11 1 3 0
After:  [1, 2, 0, 2]

Before: [0, 3, 1, 3]
9 0 0 3
After:  [0, 3, 1, 0]

Before: [2, 2, 0, 2]
11 1 3 0
After:  [1, 2, 0, 2]

Before: [0, 3, 1, 2]
8 2 1 0
After:  [3, 3, 1, 2]

Before: [0, 3, 2, 2]
9 0 0 2
After:  [0, 3, 0, 2]

Before: [3, 2, 0, 2]
13 1 2 2
After:  [3, 2, 4, 2]

Before: [0, 3, 1, 1]
14 1 3 2
After:  [0, 3, 9, 1]

Before: [2, 0, 2, 0]
1 0 2 1
After:  [2, 4, 2, 0]

Before: [3, 2, 3, 2]
11 1 3 0
After:  [1, 2, 3, 2]

Before: [0, 0, 3, 1]
7 3 2 0
After:  [3, 0, 3, 1]

Before: [2, 2, 0, 2]
12 1 1 2
After:  [2, 2, 1, 2]

Before: [0, 3, 2, 1]
8 0 1 0
After:  [3, 3, 2, 1]

Before: [2, 0, 3, 1]
7 3 2 0
After:  [3, 0, 3, 1]

Before: [0, 0, 1, 2]
8 0 3 2
After:  [0, 0, 2, 2]

Before: [1, 1, 3, 0]
0 1 0 3
After:  [1, 1, 3, 1]

Before: [1, 2, 0, 3]
4 1 0 1
After:  [1, 1, 0, 3]

Before: [1, 1, 2, 1]
0 1 0 0
After:  [1, 1, 2, 1]

Before: [2, 3, 2, 3]
6 0 3 3
After:  [2, 3, 2, 0]

Before: [2, 1, 3, 2]
7 3 1 3
After:  [2, 1, 3, 3]

Before: [1, 0, 0, 2]
4 3 3 2
After:  [1, 0, 0, 2]

Before: [0, 0, 1, 1]
4 2 3 2
After:  [0, 0, 0, 1]

Before: [1, 1, 0, 2]
0 1 0 2
After:  [1, 1, 1, 2]

Before: [0, 1, 0, 2]
3 1 0 2
After:  [0, 1, 1, 2]

Before: [2, 2, 0, 1]
13 0 2 0
After:  [4, 2, 0, 1]

Before: [2, 2, 0, 3]
6 0 3 3
After:  [2, 2, 0, 0]

Before: [0, 2, 0, 2]
13 3 2 0
After:  [4, 2, 0, 2]

Before: [2, 2, 2, 2]
1 0 2 3
After:  [2, 2, 2, 4]

Before: [1, 2, 2, 3]
2 1 2 3
After:  [1, 2, 2, 1]

Before: [1, 2, 0, 0]
15 0 1 3
After:  [1, 2, 0, 2]

Before: [2, 2, 0, 2]
11 1 3 1
After:  [2, 1, 0, 2]

Before: [1, 0, 2, 3]
5 0 2 0
After:  [3, 0, 2, 3]

Before: [1, 0, 0, 1]
10 1 0 2
After:  [1, 0, 1, 1]

Before: [3, 1, 1, 1]
14 0 3 2
After:  [3, 1, 9, 1]

Before: [0, 1, 3, 3]
3 1 0 3
After:  [0, 1, 3, 1]

Before: [2, 1, 2, 2]
1 0 2 2
After:  [2, 1, 4, 2]

Before: [0, 2, 2, 1]
12 1 1 0
After:  [1, 2, 2, 1]

Before: [1, 2, 1, 2]
11 1 3 2
After:  [1, 2, 1, 2]

Before: [0, 1, 2, 2]
3 1 0 1
After:  [0, 1, 2, 2]

Before: [2, 2, 3, 2]
11 1 3 3
After:  [2, 2, 3, 1]

Before: [0, 2, 1, 2]
12 1 1 1
After:  [0, 1, 1, 2]

Before: [2, 1, 1, 3]
6 0 3 0
After:  [0, 1, 1, 3]

Before: [1, 2, 1, 3]
8 0 1 3
After:  [1, 2, 1, 3]

Before: [0, 3, 0, 2]
9 0 0 3
After:  [0, 3, 0, 0]

Before: [0, 2, 2, 2]
11 1 3 1
After:  [0, 1, 2, 2]

Before: [0, 1, 0, 2]
3 1 0 0
After:  [1, 1, 0, 2]

Before: [2, 0, 1, 2]
13 0 2 2
After:  [2, 0, 4, 2]

Before: [1, 0, 0, 2]
10 1 0 3
After:  [1, 0, 0, 1]

Before: [3, 1, 3, 2]
14 0 3 3
After:  [3, 1, 3, 9]

Before: [2, 2, 2, 3]
15 2 3 0
After:  [6, 2, 2, 3]

Before: [1, 0, 1, 0]
10 1 0 2
After:  [1, 0, 1, 0]

Before: [0, 2, 1, 0]
9 0 0 3
After:  [0, 2, 1, 0]

Before: [1, 2, 2, 2]
11 1 3 0
After:  [1, 2, 2, 2]

Before: [1, 2, 3, 1]
15 3 1 0
After:  [2, 2, 3, 1]

Before: [0, 0, 3, 1]
8 0 2 2
After:  [0, 0, 3, 1]

Before: [2, 0, 2, 1]
14 0 3 1
After:  [2, 6, 2, 1]

Before: [0, 0, 2, 3]
15 2 3 1
After:  [0, 6, 2, 3]

Before: [1, 2, 2, 0]
12 1 1 2
After:  [1, 2, 1, 0]

Before: [2, 2, 3, 0]
12 1 0 2
After:  [2, 2, 1, 0]

Before: [3, 2, 0, 2]
11 1 3 1
After:  [3, 1, 0, 2]

Before: [2, 2, 2, 3]
6 0 3 3
After:  [2, 2, 2, 0]

Before: [0, 1, 0, 0]
3 1 0 2
After:  [0, 1, 1, 0]

Before: [0, 1, 1, 3]
3 1 0 3
After:  [0, 1, 1, 1]

Before: [2, 0, 0, 3]
6 0 3 1
After:  [2, 0, 0, 3]

Before: [3, 1, 0, 0]
8 1 0 0
After:  [3, 1, 0, 0]

Before: [0, 1, 2, 1]
3 1 0 1
After:  [0, 1, 2, 1]

Before: [1, 0, 1, 0]
10 1 0 3
After:  [1, 0, 1, 1]

Before: [0, 0, 3, 1]
12 2 3 3
After:  [0, 0, 3, 0]

Before: [0, 1, 0, 1]
4 3 3 2
After:  [0, 1, 0, 1]

Before: [1, 0, 2, 1]
7 3 2 2
After:  [1, 0, 3, 1]

Before: [1, 1, 2, 2]
15 0 2 0
After:  [2, 1, 2, 2]

Before: [3, 3, 0, 3]
2 1 3 2
After:  [3, 3, 1, 3]

Before: [1, 0, 3, 3]
10 1 0 0
After:  [1, 0, 3, 3]

Before: [1, 3, 2, 1]
14 2 3 2
After:  [1, 3, 6, 1]

Before: [0, 1, 1, 1]
3 1 0 0
After:  [1, 1, 1, 1]

Before: [3, 3, 0, 1]
4 3 3 3
After:  [3, 3, 0, 0]

Before: [3, 3, 2, 1]
7 3 2 2
After:  [3, 3, 3, 1]

Before: [3, 2, 2, 1]
2 1 2 3
After:  [3, 2, 2, 1]

Before: [1, 1, 1, 3]
6 0 3 0
After:  [0, 1, 1, 3]

Before: [3, 0, 2, 3]
5 1 3 3
After:  [3, 0, 2, 3]

Before: [2, 2, 3, 3]
15 1 3 2
After:  [2, 2, 6, 3]

Before: [1, 2, 1, 2]
8 2 1 3
After:  [1, 2, 1, 3]

Before: [0, 2, 3, 1]
15 3 1 0
After:  [2, 2, 3, 1]

Before: [3, 3, 0, 3]
12 3 0 2
After:  [3, 3, 1, 3]

Before: [2, 2, 2, 2]
11 1 3 2
After:  [2, 2, 1, 2]

Before: [3, 0, 3, 0]
8 1 2 1
After:  [3, 3, 3, 0]

Before: [1, 1, 1, 2]
5 0 3 2
After:  [1, 1, 3, 2]

Before: [0, 3, 3, 2]
9 0 0 2
After:  [0, 3, 0, 2]

Before: [0, 3, 2, 1]
9 0 0 0
After:  [0, 3, 2, 1]

Before: [1, 3, 1, 3]
5 2 3 3
After:  [1, 3, 1, 3]

Before: [2, 1, 3, 3]
6 0 3 3
After:  [2, 1, 3, 0]

Before: [0, 1, 1, 0]
9 0 0 0
After:  [0, 1, 1, 0]

Before: [1, 0, 2, 3]
6 0 3 0
After:  [0, 0, 2, 3]

Before: [3, 3, 3, 1]
7 3 2 0
After:  [3, 3, 3, 1]

Before: [0, 1, 3, 0]
3 1 0 2
After:  [0, 1, 1, 0]

Before: [1, 2, 2, 2]
4 3 3 1
After:  [1, 0, 2, 2]

Before: [3, 2, 2, 2]
11 1 3 3
After:  [3, 2, 2, 1]

Before: [0, 1, 0, 2]
9 0 0 0
After:  [0, 1, 0, 2]

Before: [1, 0, 1, 3]
10 1 0 0
After:  [1, 0, 1, 3]

Before: [3, 3, 2, 3]
2 1 3 1
After:  [3, 1, 2, 3]

Before: [2, 3, 1, 0]
8 3 0 1
After:  [2, 2, 1, 0]

Before: [1, 1, 2, 1]
0 1 0 3
After:  [1, 1, 2, 1]

Before: [3, 3, 2, 3]
15 3 2 1
After:  [3, 6, 2, 3]

Before: [0, 2, 1, 2]
4 1 2 3
After:  [0, 2, 1, 1]

Before: [2, 2, 1, 0]
13 1 2 1
After:  [2, 4, 1, 0]

Before: [1, 0, 3, 3]
10 1 0 1
After:  [1, 1, 3, 3]

Before: [3, 3, 2, 0]
1 2 2 2
After:  [3, 3, 4, 0]

Before: [1, 2, 1, 2]
11 1 3 0
After:  [1, 2, 1, 2]

Before: [1, 3, 1, 3]
6 0 3 3
After:  [1, 3, 1, 0]

Before: [3, 3, 1, 3]
8 2 1 1
After:  [3, 3, 1, 3]

Before: [1, 0, 0, 1]
4 3 3 0
After:  [0, 0, 0, 1]

Before: [1, 2, 2, 2]
11 1 3 3
After:  [1, 2, 2, 1]

Before: [3, 1, 2, 1]
8 1 0 0
After:  [3, 1, 2, 1]

Before: [3, 0, 1, 2]
8 1 3 3
After:  [3, 0, 1, 2]

Before: [1, 1, 3, 3]
0 1 0 3
After:  [1, 1, 3, 1]

Before: [0, 0, 1, 2]
13 3 2 1
After:  [0, 4, 1, 2]

Before: [2, 3, 0, 1]
13 0 2 0
After:  [4, 3, 0, 1]

Before: [1, 2, 2, 0]
2 1 2 1
After:  [1, 1, 2, 0]

Before: [2, 3, 1, 0]
13 0 2 2
After:  [2, 3, 4, 0]

Before: [0, 1, 2, 3]
3 1 0 3
After:  [0, 1, 2, 1]

Before: [2, 3, 2, 2]
15 1 3 1
After:  [2, 6, 2, 2]

Before: [0, 1, 2, 0]
3 1 0 3
After:  [0, 1, 2, 1]

Before: [1, 2, 2, 2]
15 0 2 2
After:  [1, 2, 2, 2]

Before: [1, 2, 2, 1]
12 1 1 2
After:  [1, 2, 1, 1]

Before: [0, 1, 1, 0]
3 1 0 0
After:  [1, 1, 1, 0]

Before: [3, 3, 0, 1]
14 0 3 1
After:  [3, 9, 0, 1]

Before: [1, 0, 2, 1]
10 1 0 2
After:  [1, 0, 1, 1]

Before: [0, 3, 2, 3]
9 0 0 2
After:  [0, 3, 0, 3]

Before: [0, 2, 1, 2]
11 1 3 3
After:  [0, 2, 1, 1]

Before: [2, 0, 1, 3]
6 0 3 1
After:  [2, 0, 1, 3]

Before: [1, 1, 2, 3]
0 1 0 3
After:  [1, 1, 2, 1]

Before: [3, 0, 2, 2]
1 2 2 1
After:  [3, 4, 2, 2]

Before: [1, 3, 2, 0]
5 3 2 3
After:  [1, 3, 2, 2]

Before: [3, 3, 0, 3]
8 2 0 1
After:  [3, 3, 0, 3]

Before: [0, 0, 3, 3]
9 0 0 3
After:  [0, 0, 3, 0]

Before: [0, 3, 1, 3]
8 2 1 3
After:  [0, 3, 1, 3]

Before: [1, 1, 3, 2]
0 1 0 3
After:  [1, 1, 3, 1]

Before: [1, 0, 0, 3]
10 1 0 2
After:  [1, 0, 1, 3]

Before: [2, 0, 1, 2]
13 3 2 3
After:  [2, 0, 1, 4]

Before: [3, 2, 3, 2]
13 1 2 0
After:  [4, 2, 3, 2]

Before: [0, 1, 0, 2]
3 1 0 3
After:  [0, 1, 0, 1]

Before: [1, 1, 3, 1]
0 1 0 0
After:  [1, 1, 3, 1]

Before: [0, 1, 3, 0]
3 1 0 0
After:  [1, 1, 3, 0]

Before: [0, 1, 0, 1]
3 1 0 1
After:  [0, 1, 0, 1]

Before: [3, 2, 1, 2]
4 1 2 2
After:  [3, 2, 1, 2]

Before: [1, 1, 2, 3]
0 1 0 0
After:  [1, 1, 2, 3]

Before: [3, 2, 3, 2]
11 1 3 2
After:  [3, 2, 1, 2]

Before: [0, 1, 3, 1]
3 1 0 1
After:  [0, 1, 3, 1]

Before: [1, 1, 2, 0]
0 1 0 3
After:  [1, 1, 2, 1]

Before: [0, 1, 3, 0]
9 0 0 3
After:  [0, 1, 3, 0]

Before: [2, 2, 3, 1]
14 2 3 2
After:  [2, 2, 9, 1]

Before: [0, 1, 0, 1]
3 1 0 2
After:  [0, 1, 1, 1]

Before: [3, 3, 3, 3]
15 0 3 3
After:  [3, 3, 3, 9]

Before: [2, 1, 2, 3]
6 0 3 0
After:  [0, 1, 2, 3]

Before: [0, 2, 1, 2]
8 0 3 1
After:  [0, 2, 1, 2]

Before: [0, 2, 2, 1]
2 1 2 1
After:  [0, 1, 2, 1]

Before: [1, 0, 1, 2]
10 1 0 0
After:  [1, 0, 1, 2]

Before: [1, 2, 3, 3]
15 1 3 3
After:  [1, 2, 3, 6]

Before: [2, 1, 2, 3]
15 1 2 3
After:  [2, 1, 2, 2]

Before: [1, 2, 2, 1]
14 1 3 3
After:  [1, 2, 2, 6]

Before: [1, 1, 1, 0]
0 1 0 1
After:  [1, 1, 1, 0]

Before: [3, 2, 3, 1]
4 3 3 0
After:  [0, 2, 3, 1]

Before: [0, 2, 2, 2]
11 1 3 2
After:  [0, 2, 1, 2]

Before: [2, 1, 3, 1]
14 3 2 3
After:  [2, 1, 3, 2]

Before: [0, 2, 2, 3]
2 1 2 0
After:  [1, 2, 2, 3]

Before: [0, 1, 3, 3]
5 0 3 2
After:  [0, 1, 3, 3]

Before: [0, 3, 2, 2]
15 1 2 0
After:  [6, 3, 2, 2]

Before: [2, 2, 1, 2]
14 3 3 1
After:  [2, 6, 1, 2]

Before: [2, 2, 2, 2]
11 1 3 1
After:  [2, 1, 2, 2]

Before: [3, 2, 2, 2]
11 1 3 0
After:  [1, 2, 2, 2]

Before: [1, 3, 2, 1]
15 0 2 0
After:  [2, 3, 2, 1]

Before: [1, 2, 0, 0]
15 0 1 2
After:  [1, 2, 2, 0]

Before: [1, 2, 2, 3]
1 1 2 1
After:  [1, 4, 2, 3]

Before: [3, 2, 2, 1]
7 3 2 0
After:  [3, 2, 2, 1]

Before: [3, 2, 2, 0]
12 1 1 2
After:  [3, 2, 1, 0]

Before: [1, 0, 3, 3]
6 0 3 1
After:  [1, 0, 3, 3]

Before: [2, 0, 2, 1]
1 0 2 3
After:  [2, 0, 2, 4]

Before: [2, 0, 1, 2]
13 0 2 0
After:  [4, 0, 1, 2]

Before: [2, 2, 2, 2]
12 2 1 0
After:  [1, 2, 2, 2]

Before: [2, 2, 0, 2]
13 3 2 1
After:  [2, 4, 0, 2]

Before: [3, 3, 3, 0]
14 2 3 1
After:  [3, 9, 3, 0]

Before: [3, 1, 0, 2]
4 3 3 3
After:  [3, 1, 0, 0]

Before: [0, 2, 2, 3]
1 1 2 0
After:  [4, 2, 2, 3]

Before: [0, 1, 1, 2]
9 0 0 1
After:  [0, 0, 1, 2]

Before: [3, 3, 1, 0]
14 1 3 2
After:  [3, 3, 9, 0]

Before: [2, 0, 2, 3]
5 1 2 2
After:  [2, 0, 2, 3]

Before: [1, 1, 1, 2]
0 1 0 0
After:  [1, 1, 1, 2]

Before: [3, 2, 3, 3]
15 1 3 0
After:  [6, 2, 3, 3]

Before: [1, 3, 2, 1]
14 1 3 0
After:  [9, 3, 2, 1]

Before: [2, 0, 2, 2]
1 2 2 0
After:  [4, 0, 2, 2]

Before: [0, 2, 1, 2]
11 1 3 0
After:  [1, 2, 1, 2]

Before: [0, 1, 1, 1]
3 1 0 3
After:  [0, 1, 1, 1]

Before: [1, 0, 1, 0]
10 1 0 1
After:  [1, 1, 1, 0]

Before: [1, 1, 1, 1]
4 3 3 3
After:  [1, 1, 1, 0]

Before: [3, 2, 2, 2]
14 0 3 3
After:  [3, 2, 2, 9]

Before: [0, 1, 2, 1]
9 0 0 0
After:  [0, 1, 2, 1]

Before: [2, 0, 2, 3]
1 2 2 0
After:  [4, 0, 2, 3]

Before: [1, 1, 1, 3]
0 1 0 3
After:  [1, 1, 1, 1]

Before: [0, 1, 0, 3]
3 1 0 1
After:  [0, 1, 0, 3]

Before: [2, 0, 2, 1]
5 1 2 0
After:  [2, 0, 2, 1]

Before: [1, 3, 3, 1]
14 0 2 0
After:  [2, 3, 3, 1]

Before: [1, 0, 0, 3]
6 0 3 1
After:  [1, 0, 0, 3]

Before: [2, 3, 0, 3]
6 0 3 3
After:  [2, 3, 0, 0]

Before: [0, 1, 2, 2]
1 3 2 3
After:  [0, 1, 2, 4]

Before: [3, 1, 2, 3]
1 2 2 2
After:  [3, 1, 4, 3]

Before: [3, 1, 0, 3]
8 2 0 3
After:  [3, 1, 0, 3]

Before: [3, 3, 1, 2]
14 0 3 3
After:  [3, 3, 1, 9]

Before: [3, 0, 3, 2]
4 3 3 3
After:  [3, 0, 3, 0]

Before: [2, 3, 1, 0]
8 2 1 0
After:  [3, 3, 1, 0]

Before: [0, 1, 2, 1]
3 1 0 2
After:  [0, 1, 1, 1]

Before: [0, 1, 3, 2]
3 1 0 0
After:  [1, 1, 3, 2]

Before: [1, 1, 2, 0]
15 0 2 1
After:  [1, 2, 2, 0]

Before: [0, 3, 2, 2]
1 3 2 0
After:  [4, 3, 2, 2]

Before: [1, 0, 1, 2]
10 1 0 3
After:  [1, 0, 1, 1]

Before: [0, 2, 2, 0]
12 1 1 0
After:  [1, 2, 2, 0]

Before: [1, 1, 3, 3]
0 1 0 1
After:  [1, 1, 3, 3]

Before: [1, 0, 2, 0]
10 1 0 2
After:  [1, 0, 1, 0]

Before: [2, 3, 2, 1]
15 1 2 1
After:  [2, 6, 2, 1]

Before: [1, 2, 0, 0]
8 1 0 1
After:  [1, 3, 0, 0]

Before: [3, 2, 1, 2]
11 1 3 3
After:  [3, 2, 1, 1]

Before: [1, 1, 1, 1]
0 1 0 3
After:  [1, 1, 1, 1]

Before: [2, 3, 1, 3]
6 0 3 3
After:  [2, 3, 1, 0]

Before: [0, 2, 1, 3]
13 1 2 2
After:  [0, 2, 4, 3]

Before: [1, 0, 3, 1]
7 3 2 3
After:  [1, 0, 3, 3]

Before: [2, 1, 0, 0]
8 3 0 3
After:  [2, 1, 0, 2]

Before: [1, 1, 1, 2]
0 1 0 2
After:  [1, 1, 1, 2]

Before: [3, 2, 2, 2]
1 3 2 3
After:  [3, 2, 2, 4]

Before: [3, 1, 1, 1]
14 0 3 3
After:  [3, 1, 1, 9]

Before: [2, 2, 3, 3]
6 0 3 0
After:  [0, 2, 3, 3]

Before: [0, 1, 1, 2]
3 1 0 2
After:  [0, 1, 1, 2]

Before: [3, 2, 1, 2]
14 0 3 0
After:  [9, 2, 1, 2]

Before: [2, 0, 3, 1]
7 3 2 3
After:  [2, 0, 3, 3]

Before: [1, 2, 2, 1]
2 1 2 0
After:  [1, 2, 2, 1]

Before: [0, 0, 2, 1]
4 3 3 3
After:  [0, 0, 2, 0]

Before: [3, 2, 3, 3]
12 3 0 3
After:  [3, 2, 3, 1]

Before: [1, 1, 2, 2]
0 1 0 3
After:  [1, 1, 2, 1]

Before: [2, 3, 1, 1]
14 1 3 2
After:  [2, 3, 9, 1]

Before: [1, 0, 2, 1]
15 0 2 1
After:  [1, 2, 2, 1]

Before: [0, 1, 2, 3]
9 0 0 3
After:  [0, 1, 2, 0]

Before: [3, 0, 0, 2]
4 3 3 0
After:  [0, 0, 0, 2]

Before: [3, 0, 3, 1]
7 3 2 3
After:  [3, 0, 3, 3]

Before: [3, 2, 2, 3]
12 2 1 1
After:  [3, 1, 2, 3]

Before: [0, 3, 3, 3]
2 1 3 1
After:  [0, 1, 3, 3]

Before: [1, 0, 0, 0]
10 1 0 1
After:  [1, 1, 0, 0]

Before: [1, 3, 0, 3]
2 1 3 3
After:  [1, 3, 0, 1]

Before: [3, 2, 2, 3]
12 3 0 2
After:  [3, 2, 1, 3]

Before: [1, 1, 2, 0]
0 1 0 0
After:  [1, 1, 2, 0]

Before: [0, 2, 2, 1]
2 1 2 2
After:  [0, 2, 1, 1]

Before: [1, 0, 1, 2]
10 1 0 2
After:  [1, 0, 1, 2]

Before: [1, 0, 3, 0]
14 0 2 2
After:  [1, 0, 2, 0]

Before: [0, 2, 3, 2]
11 1 3 3
After:  [0, 2, 3, 1]

Before: [0, 1, 0, 3]
3 1 0 2
After:  [0, 1, 1, 3]

Before: [3, 3, 2, 1]
12 1 0 0
After:  [1, 3, 2, 1]

Before: [0, 2, 3, 3]
13 1 2 0
After:  [4, 2, 3, 3]

Before: [1, 1, 3, 0]
0 1 0 1
After:  [1, 1, 3, 0]

Before: [1, 2, 2, 2]
2 1 2 2
After:  [1, 2, 1, 2]

Before: [2, 1, 3, 3]
13 0 2 2
After:  [2, 1, 4, 3]

Before: [1, 0, 1, 3]
10 1 0 3
After:  [1, 0, 1, 1]

Before: [2, 1, 3, 2]
7 3 1 1
After:  [2, 3, 3, 2]

Before: [1, 1, 1, 0]
0 1 0 0
After:  [1, 1, 1, 0]

Before: [0, 2, 3, 2]
13 3 2 3
After:  [0, 2, 3, 4]

Before: [3, 2, 2, 3]
12 3 0 3
After:  [3, 2, 2, 1]

Before: [2, 3, 2, 3]
2 1 3 1
After:  [2, 1, 2, 3]

Before: [2, 3, 0, 3]
6 0 3 1
After:  [2, 0, 0, 3]

Before: [2, 2, 2, 3]
1 2 2 0
After:  [4, 2, 2, 3]

Before: [3, 2, 3, 2]
4 3 3 1
After:  [3, 0, 3, 2]

Before: [0, 2, 2, 0]
12 1 1 1
After:  [0, 1, 2, 0]

Before: [2, 2, 3, 2]
11 1 3 2
After:  [2, 2, 1, 2]

Before: [3, 0, 2, 3]
1 2 2 2
After:  [3, 0, 4, 3]

Before: [0, 3, 2, 0]
9 0 0 0
After:  [0, 3, 2, 0]

Before: [0, 2, 3, 1]
9 0 0 2
After:  [0, 2, 0, 1]

Before: [0, 1, 1, 1]
3 1 0 2
After:  [0, 1, 1, 1]

Before: [2, 2, 2, 3]
2 1 2 0
After:  [1, 2, 2, 3]

Before: [0, 1, 2, 3]
15 3 2 0
After:  [6, 1, 2, 3]

Before: [1, 1, 0, 2]
7 3 1 3
After:  [1, 1, 0, 3]

Before: [1, 1, 1, 1]
0 1 0 0
After:  [1, 1, 1, 1]

Before: [1, 2, 3, 3]
15 0 1 3
After:  [1, 2, 3, 2]

Before: [0, 3, 2, 3]
2 1 3 1
After:  [0, 1, 2, 3]

Before: [0, 3, 0, 0]
8 2 1 3
After:  [0, 3, 0, 3]

Before: [2, 0, 2, 3]
1 0 2 0
After:  [4, 0, 2, 3]

Before: [0, 3, 0, 3]
5 0 3 3
After:  [0, 3, 0, 3]

Before: [0, 1, 2, 0]
3 1 0 0
After:  [1, 1, 2, 0]
";

 private const string Input2=@"7 3 2 0
        7 2 1 1
        7 1 0 3
        8 1 0 1
        14 1 2 1
        1 2 1 2
        7 3 3 1
        7 2 0 0
        14 3 0 3
        13 3 2 3
        0 1 0 1
        14 1 1 1
        1 1 2 2
        7 2 2 1
        7 3 1 0
        7 1 0 3
        1 3 3 3
        14 3 3 3
        1 2 3 2
        3 2 1 0
        14 3 0 3
        13 3 0 3
        7 2 3 2
        14 2 0 1
        13 1 0 1
        6 3 2 2
        14 2 2 2
        1 0 2 0
        7 3 1 1
        7 2 2 3
        14 3 0 2
        13 2 0 2
        7 2 1 3
        14 3 3 3
        1 0 3 0
        7 0 0 3
        14 0 0 2
        13 2 3 2
        14 0 0 1
        13 1 1 1
        10 3 2 3
        14 3 2 3
        1 0 3 0
        3 0 1 1
        7 0 1 0
        7 1 1 2
        7 3 1 3
        9 3 2 0
        14 0 1 0
        1 1 0 1
        3 1 0 3
        14 0 0 2
        13 2 2 2
        7 3 2 1
        14 2 0 0
        13 0 1 0
        3 0 2 0
        14 0 2 0
        1 0 3 3
        3 3 2 1
        7 2 1 0
        14 2 0 2
        13 2 3 2
        14 0 0 3
        13 3 2 3
        8 0 2 2
        14 2 2 2
        1 1 2 1
        14 0 0 2
        13 2 0 2
        14 1 0 3
        13 3 1 3
        15 3 0 2
        14 2 1 2
        1 2 1 1
        3 1 0 0
        14 2 0 1
        13 1 3 1
        7 2 1 3
        7 2 0 2
        5 2 3 3
        14 3 2 3
        14 3 3 3
        1 0 3 0
        3 0 0 3
        7 1 1 2
        7 2 1 1
        7 3 0 0
        8 1 0 1
        14 1 2 1
        14 1 3 1
        1 3 1 3
        3 3 0 1
        7 0 0 3
        14 1 0 0
        13 0 0 0
        7 2 2 2
        7 3 2 0
        14 0 2 0
        1 1 0 1
        7 1 1 2
        14 2 0 3
        13 3 2 3
        7 3 3 0
        9 0 2 2
        14 2 1 2
        1 2 1 1
        3 1 2 2
        14 1 0 1
        13 1 0 1
        7 1 2 0
        7 3 1 3
        13 0 1 1
        14 1 1 1
        1 2 1 2
        3 2 0 1
        7 2 0 3
        7 2 3 2
        7 2 3 0
        5 0 3 2
        14 2 2 2
        1 1 2 1
        3 1 1 0
        14 0 0 1
        13 1 2 1
        14 0 0 2
        13 2 3 2
        14 2 0 3
        13 3 3 3
        8 1 2 1
        14 1 1 1
        1 1 0 0
        3 0 0 2
        7 3 3 0
        7 2 3 3
        7 0 0 1
        0 0 3 3
        14 3 2 3
        1 3 2 2
        3 2 0 3
        14 3 0 2
        13 2 2 2
        7 1 1 1
        12 2 0 0
        14 0 2 0
        1 0 3 3
        3 3 3 1
        7 0 2 3
        7 3 0 2
        7 2 3 0
        8 0 2 3
        14 3 3 3
        1 3 1 1
        7 3 3 3
        2 0 2 2
        14 2 2 2
        14 2 1 2
        1 2 1 1
        3 1 3 3
        7 1 0 0
        7 3 2 1
        7 2 3 2
        3 0 2 1
        14 1 1 1
        14 1 3 1
        1 1 3 3
        3 3 1 0
        14 1 0 1
        13 1 0 1
        7 3 2 2
        7 0 1 3
        7 2 1 1
        14 1 1 1
        1 0 1 0
        3 0 0 1
        7 2 0 0
        7 2 2 2
        7 2 2 3
        11 0 3 2
        14 2 3 2
        1 1 2 1
        14 1 0 2
        13 2 3 2
        7 0 3 3
        2 0 2 0
        14 0 2 0
        14 0 3 0
        1 1 0 1
        3 1 2 0
        7 2 3 2
        7 3 3 1
        7 3 2 3
        12 2 1 1
        14 1 3 1
        1 1 0 0
        7 0 0 1
        7 0 2 2
        7 2 0 3
        10 2 3 2
        14 2 3 2
        1 2 0 0
        3 0 3 1
        14 3 0 0
        13 0 0 0
        7 2 2 2
        5 2 3 3
        14 3 1 3
        1 3 1 1
        3 1 2 2
        7 1 0 3
        14 0 0 1
        13 1 2 1
        7 2 1 0
        4 0 3 1
        14 1 2 1
        1 2 1 2
        3 2 3 0
        7 3 1 3
        7 3 3 1
        7 1 0 2
        9 3 2 1
        14 1 3 1
        14 1 1 1
        1 0 1 0
        3 0 0 2
        7 0 3 3
        14 2 0 1
        13 1 3 1
        7 2 2 0
        12 0 1 0
        14 0 1 0
        1 0 2 2
        14 0 0 0
        13 0 2 0
        5 0 3 0
        14 0 2 0
        1 2 0 2
        3 2 2 0
        7 2 1 3
        7 2 1 2
        12 2 1 1
        14 1 2 1
        1 0 1 0
        3 0 2 2
        7 0 3 1
        7 2 3 0
        11 0 3 0
        14 0 3 0
        14 0 2 0
        1 0 2 2
        3 2 3 1
        7 1 0 0
        7 3 2 2
        7 3 3 3
        14 0 2 2
        14 2 2 2
        1 2 1 1
        3 1 3 2
        7 1 0 3
        7 3 1 1
        7 2 0 0
        15 3 0 3
        14 3 3 3
        1 3 2 2
        3 2 3 1
        7 1 0 3
        7 1 0 0
        14 1 0 2
        13 2 2 2
        1 0 3 3
        14 3 2 3
        14 3 2 3
        1 3 1 1
        3 1 2 3
        14 0 0 2
        13 2 3 2
        7 0 3 1
        13 0 1 1
        14 1 2 1
        1 1 3 3
        3 3 2 2
        14 3 0 3
        13 3 1 3
        7 2 0 1
        7 3 3 0
        1 3 3 1
        14 1 3 1
        1 1 2 2
        3 2 1 1
        7 2 2 0
        7 0 3 3
        7 2 1 2
        6 3 2 3
        14 3 1 3
        1 3 1 1
        3 1 1 3
        7 1 1 0
        7 1 0 1
        7 3 3 2
        14 1 2 2
        14 2 3 2
        14 2 1 2
        1 2 3 3
        7 3 3 1
        7 2 2 2
        3 0 2 2
        14 2 3 2
        1 2 3 3
        3 3 2 1
        7 0 3 3
        7 2 2 2
        7 3 2 0
        12 2 0 3
        14 3 3 3
        1 1 3 1
        7 0 3 3
        7 3 0 2
        14 1 0 0
        13 0 2 0
        2 0 2 2
        14 2 2 2
        1 1 2 1
        7 3 1 3
        7 3 0 2
        0 3 0 3
        14 3 1 3
        1 3 1 1
        14 2 0 3
        13 3 1 3
        4 0 3 3
        14 3 2 3
        14 3 1 3
        1 1 3 1
        3 1 1 0
        14 1 0 2
        13 2 2 2
        7 1 2 1
        7 0 2 3
        6 3 2 3
        14 3 1 3
        1 3 0 0
        3 0 2 3
        14 3 0 1
        13 1 0 1
        7 1 3 0
        3 0 2 0
        14 0 2 0
        14 0 2 0
        1 0 3 3
        14 0 0 0
        13 0 1 0
        7 0 0 2
        14 0 2 2
        14 2 1 2
        1 3 2 3
        7 2 2 2
        3 0 2 2
        14 2 3 2
        1 3 2 3
        3 3 2 2
        7 2 1 0
        7 2 0 3
        11 0 3 3
        14 3 2 3
        1 3 2 2
        3 2 0 0
        7 2 3 1
        7 0 2 3
        7 2 1 2
        6 3 2 3
        14 3 3 3
        14 3 1 3
        1 0 3 0
        7 0 3 3
        6 3 2 2
        14 2 1 2
        1 0 2 0
        14 0 0 3
        13 3 1 3
        7 1 0 1
        7 0 2 2
        14 1 2 2
        14 2 2 2
        14 2 2 2
        1 0 2 0
        3 0 1 3
        7 2 1 0
        7 3 3 2
        14 1 2 0
        14 0 2 0
        1 3 0 3
        3 3 0 0
        14 0 0 3
        13 3 2 3
        7 0 2 2
        10 2 3 2
        14 2 1 2
        1 2 0 0
        3 0 1 1
        14 0 0 2
        13 2 3 2
        14 1 0 3
        13 3 0 3
        7 3 2 0
        7 2 0 0
        14 0 1 0
        1 0 1 1
        7 0 1 2
        7 0 0 0
        7 2 1 3
        10 2 3 3
        14 3 2 3
        1 1 3 1
        7 2 0 3
        10 2 3 0
        14 0 1 0
        1 1 0 1
        7 2 3 2
        7 0 2 3
        7 3 1 0
        8 2 0 0
        14 0 3 0
        1 1 0 1
        7 3 0 3
        14 0 0 2
        13 2 0 2
        7 1 3 0
        9 3 2 3
        14 3 1 3
        1 1 3 1
        7 0 3 3
        7 3 2 2
        7 0 1 0
        10 3 2 3
        14 3 2 3
        1 1 3 1
        3 1 3 3
        7 3 3 0
        7 1 3 1
        7 0 2 2
        2 2 0 2
        14 2 1 2
        1 3 2 3
        3 3 1 1
        7 2 3 2
        7 2 0 0
        7 0 3 3
        6 3 2 2
        14 2 2 2
        1 2 1 1
        3 1 3 2
        7 1 2 1
        7 2 3 3
        15 1 0 3
        14 3 2 3
        14 3 3 3
        1 2 3 2
        7 1 0 3
        7 2 3 1
        4 0 3 0
        14 0 2 0
        1 0 2 2
        3 2 2 0
        14 2 0 2
        13 2 1 2
        7 0 3 3
        5 1 3 3
        14 3 1 3
        1 0 3 0
        3 0 2 1
        7 2 1 0
        7 1 3 3
        15 3 0 3
        14 3 3 3
        14 3 1 3
        1 1 3 1
        3 1 3 3
        7 1 2 1
        7 0 2 2
        7 1 1 0
        14 0 2 1
        14 1 2 1
        1 1 3 3
        3 3 1 2
        7 3 0 1
        7 2 0 0
        14 2 0 3
        13 3 1 3
        13 3 1 3
        14 3 2 3
        14 3 3 3
        1 3 2 2
        3 2 2 0
        7 0 0 2
        7 3 0 3
        9 3 2 1
        14 1 2 1
        14 1 3 1
        1 0 1 0
        7 0 1 3
        7 3 3 1
        7 2 0 2
        12 2 1 3
        14 3 1 3
        14 3 1 3
        1 3 0 0
        7 1 2 3
        14 0 0 2
        13 2 3 2
        1 3 3 3
        14 3 1 3
        1 3 0 0
        3 0 0 2
        7 2 2 3
        7 2 1 0
        11 0 3 0
        14 0 3 0
        1 0 2 2
        3 2 3 3
        7 3 1 2
        7 1 3 0
        13 0 1 2
        14 2 2 2
        1 3 2 3
        7 1 0 1
        7 2 1 0
        7 1 3 2
        15 1 0 1
        14 1 1 1
        14 1 1 1
        1 1 3 3
        3 3 3 0
        7 3 1 1
        7 3 1 3
        14 2 0 2
        13 2 0 2
        9 3 2 3
        14 3 2 3
        14 3 3 3
        1 3 0 0
        3 0 1 1
        7 2 1 0
        7 1 2 2
        7 2 1 3
        11 0 3 2
        14 2 2 2
        14 2 1 2
        1 1 2 1
        7 2 0 2
        7 0 2 0
        7 1 0 3
        1 3 3 3
        14 3 1 3
        1 1 3 1
        3 1 0 0
        14 2 0 1
        13 1 3 1
        7 0 2 2
        7 1 3 3
        1 3 3 2
        14 2 1 2
        1 0 2 0
        3 0 3 2
        7 0 3 0
        7 0 3 3
        7 2 2 1
        7 3 0 3
        14 3 1 3
        14 3 3 3
        1 2 3 2
        3 2 1 1
        7 2 2 2
        7 2 0 3
        7 3 0 0
        12 2 0 0
        14 0 3 0
        14 0 1 0
        1 0 1 1
        7 1 3 2
        7 2 0 0
        7 1 3 3
        4 0 3 3
        14 3 1 3
        1 3 1 1
        14 2 0 3
        13 3 0 3
        7 3 1 0
        14 0 0 2
        13 2 2 2
        6 3 2 3
        14 3 2 3
        1 3 1 1
        3 1 2 2
        7 0 0 1
        14 3 0 3
        13 3 1 3
        7 2 2 0
        15 3 0 0
        14 0 3 0
        1 2 0 2
        3 2 2 1
        7 0 0 2
        14 3 0 0
        13 0 3 0
        9 0 2 0
        14 0 2 0
        1 0 1 1
        7 2 2 0
        7 0 3 3
        7 3 1 2
        8 0 2 2
        14 2 1 2
        14 2 3 2
        1 2 1 1
        3 1 1 0
        14 1 0 1
        13 1 3 1
        7 2 0 2
        6 3 2 2
        14 2 3 2
        1 0 2 0
        3 0 1 1
        7 1 1 3
        7 2 0 0
        7 3 1 2
        7 2 3 0
        14 0 2 0
        1 0 1 1
        7 0 0 3
        14 0 0 0
        13 0 1 0
        10 3 2 2
        14 2 1 2
        1 2 1 1
        3 1 0 0
        14 3 0 2
        13 2 0 2
        7 0 2 1
        7 2 2 3
        10 2 3 1
        14 1 1 1
        1 0 1 0
        3 0 1 1
        14 2 0 0
        13 0 2 0
        7 3 1 2
        7 1 2 3
        8 0 2 0
        14 0 3 0
        1 1 0 1
        3 1 2 2
        7 2 3 1
        7 2 1 3
        7 2 3 0
        11 0 3 1
        14 1 1 1
        1 2 1 2
        3 2 0 1
        14 2 0 2
        13 2 2 2
        14 3 0 0
        13 0 3 0
        7 0 0 3
        6 3 2 2
        14 2 1 2
        1 1 2 1
        7 0 2 0
        7 2 1 2
        7 3 0 0
        14 0 3 0
        1 0 1 1
        3 1 0 3
        7 3 1 0
        7 3 3 2
        14 0 0 1
        13 1 2 1
        8 1 0 0
        14 0 3 0
        1 3 0 3
        3 3 2 1
        7 0 2 0
        7 0 2 3
        7 2 0 2
        6 3 2 2
        14 2 2 2
        14 2 3 2
        1 2 1 1
        3 1 1 0
        7 3 3 1
        7 1 3 3
        7 0 1 2
        9 1 2 1
        14 1 2 1
        1 0 1 0
        3 0 0 3
        7 2 0 1
        7 3 2 0
        9 0 2 2
        14 2 3 2
        1 2 3 3
        14 1 0 2
        13 2 2 2
        7 1 2 1
        12 2 0 1
        14 1 3 1
        14 1 1 1
        1 1 3 3
        3 3 1 2
        7 2 0 0
        7 1 2 3
        7 1 3 1
        4 0 3 1
        14 1 2 1
        14 1 3 1
        1 1 2 2
        7 3 3 1
        7 2 2 3
        12 0 1 0
        14 0 3 0
        1 0 2 2
        3 2 2 1
        7 3 1 2
        7 3 2 0
        0 0 3 2
        14 2 3 2
        1 1 2 1
        3 1 0 0
        7 2 1 2
        7 1 2 1
        15 1 3 2
        14 2 1 2
        14 2 2 2
        1 2 0 0
        3 0 2 1
        7 1 0 3
        7 1 2 2
        7 1 1 0
        1 3 3 2
        14 2 3 2
        14 2 2 2
        1 2 1 1
        3 1 0 2
        7 0 1 0
        7 2 3 1
        1 3 3 0
        14 0 1 0
        1 0 2 2
        3 2 0 0
        7 1 1 1
        7 2 0 3
        7 3 3 2
        15 1 3 3
        14 3 3 3
        1 0 3 0
        7 2 0 2
        7 0 1 3
        7 0 3 1
        6 3 2 1
        14 1 2 1
        14 1 1 1
        1 0 1 0
        3 0 2 3
        7 3 1 2
        7 3 3 0
        7 3 2 1
        7 2 0 2
        14 2 1 2
        1 2 3 3
        7 3 2 2
        7 1 2 1
        9 0 2 0
        14 0 2 0
        14 0 3 0
        1 3 0 3
        3 3 0 1
        7 1 3 3
        14 1 0 0
        13 0 3 0
        7 2 3 2
        12 2 0 3
        14 3 3 3
        1 1 3 1
        3 1 2 0
        7 3 1 2
        7 3 0 1
        7 1 1 3
        14 3 2 3
        14 3 2 3
        14 3 1 3
        1 0 3 0
        3 0 3 1
        7 1 3 3
        7 0 2 2
        7 2 3 0
        4 0 3 2
        14 2 2 2
        1 2 1 1
        7 1 2 0
        7 2 3 3
        7 0 0 2
        10 2 3 3
        14 3 1 3
        1 3 1 1
        7 3 1 3
        7 3 2 0
        2 2 0 2
        14 2 1 2
        14 2 1 2
        1 2 1 1
        14 2 0 0
        13 0 1 0
        7 3 0 2
        7 0 2 3
        7 2 0 0
        14 0 2 0
        1 0 1 1
        3 1 0 3
        14 1 0 0
        13 0 3 0
        7 2 0 2
        7 2 2 1
        8 2 0 1
        14 1 3 1
        14 1 3 1
        1 1 3 3
        3 3 2 2
        7 2 3 0
        7 2 1 3
        7 3 3 1
        0 1 0 3
        14 3 2 3
        14 3 1 3
        1 3 2 2
        3 2 3 1
        7 0 0 2
        7 3 0 0
        7 2 2 3
        9 0 2 0
        14 0 1 0
        1 0 1 1
        7 2 3 2
        7 0 1 3
        7 0 2 0
        5 2 3 2
        14 2 2 2
        1 1 2 1
        3 1 1 2
        7 2 0 1
        5 1 3 1
        14 1 1 1
        1 2 1 2
        3 2 3 1
        7 3 0 3
        7 0 0 2
        14 2 0 0
        13 0 1 0
        14 0 2 0
        14 0 3 0
        1 1 0 1
        3 1 1 3
        7 2 2 1
        7 3 2 0
        8 1 0 1
        14 1 3 1
        1 1 3 3
        3 3 2 1
        14 2 0 3
        13 3 0 3
        14 1 0 0
        13 0 0 0
        14 0 0 2
        13 2 3 2
        10 3 2 2
        14 2 1 2
        14 2 2 2
        1 1 2 1
        3 1 3 2
        7 2 3 0
        14 2 0 3
        13 3 1 3
        7 0 1 1
        1 3 3 1
        14 1 3 1
        1 1 2 2
        3 2 2 1
        14 0 0 2
        13 2 2 2
        7 1 1 0
        7 0 0 3
        3 0 2 0
        14 0 3 0
        1 0 1 1
        3 1 1 2
        7 2 2 3
        7 2 0 0
        7 3 0 1
        5 0 3 0
        14 0 3 0
        1 2 0 2
        7 2 3 0
        14 1 0 3
        13 3 0 3
        14 1 0 1
        13 1 1 1
        15 1 0 3
        14 3 1 3
        1 3 2 2
        3 2 2 1
        7 2 1 2
        14 0 0 3
        13 3 1 3
        7 0 1 0
        7 3 0 0
        14 0 2 0
        1 0 1 1
        3 1 1 2
        7 2 1 3
        7 3 0 1
        7 2 3 0
        11 0 3 3
        14 3 3 3
        1 2 3 2
        3 2 1 1
        7 2 2 3
        7 0 1 2
        14 1 0 0
        13 0 0 0
        10 2 3 2
        14 2 1 2
        14 2 3 2
        1 2 1 1
        7 3 3 0
        7 3 0 3
        7 3 2 2
        9 3 2 2
        14 2 1 2
        1 2 1 1
        3 1 2 2
        14 0 0 1
        13 1 0 1
        7 0 0 3
        7 1 1 0
        13 0 1 0
        14 0 1 0
        14 0 2 0
        1 2 0 2
        14 2 0 0
        13 0 2 0
        7 2 3 3
        11 0 3 1
        14 1 2 1
        1 1 2 2
        3 2 1 1
        7 3 3 3
        7 3 3 2
        2 0 2 3
        14 3 1 3
        14 3 3 3
        1 1 3 1
        3 1 0 3
        7 1 1 2
        7 3 0 1
        12 0 1 1
        14 1 2 1
        1 1 3 3
        3 3 3 1
        14 3 0 2
        13 2 0 2
        7 2 2 3
        11 0 3 3
        14 3 2 3
        1 1 3 1
        3 1 1 2
        7 3 3 1
        7 1 2 3
        4 0 3 3
        14 3 3 3
        14 3 2 3
        1 2 3 2
        3 2 0 0
        7 0 2 2
        14 2 0 3
        13 3 1 3
        1 3 3 1
        14 1 2 1
        14 1 2 1
        1 1 0 0
        3 0 2 2
        7 2 1 0
        7 1 0 1
        15 1 0 1
        14 1 2 1
        14 1 2 1
        1 2 1 2
        3 2 2 1
        7 3 3 3
        7 3 2 2
        8 0 2 0
        14 0 3 0
        1 0 1 1
        7 0 2 2
        7 2 1 3
        14 1 0 0
        13 0 3 0
        2 2 0 0
        14 0 3 0
        14 0 3 0
        1 0 1 1
        3 1 0 2
        7 2 2 0
        7 2 3 1
        11 0 3 0
        14 0 3 0
        14 0 1 0
        1 0 2 2
        3 2 2 3
        7 2 2 0
        7 1 3 2
        14 3 0 1
        13 1 3 1
        12 0 1 0
        14 0 2 0
        14 0 3 0
        1 3 0 3
        3 3 2 0
        7 2 2 2
        7 0 1 3
        6 3 2 3
        14 3 1 3
        1 3 0 0";
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;

namespace AdventCalendar2019.Day22
{
    public class DupdobDay22
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay22();;

            runner.ParseInput();
            
            Console.WriteLine($"Answer 1: {runner.Answer1()}");
            Console.WriteLine($"Answer 2: {runner.Answer2()}");
        }

        private int Answer1()
        {
            var size = 10007;
            Simplify(size);
            var pos = (((2019 * _multiplier + _offset) % size) + size) % size;
            return (int)pos;
        }

        private void Simplify(long size)
        {
            _offset = 0;
            _multiplier = 1;
            foreach (var op in _shuffle)
            {
                _multiplier = (_multiplier * op.mul) % size;
                _offset = (_offset * op.mul + op.offset) % size;
            }

            if (_offset < 0)
            {
                _offset += size;
            }
        }

        private static long Mod(long a, long b)
        {
            return (a >= 0) ? (a % b) : (b + a % b);
        }

        private static long mod_multiply(long a, long b, long n)
        {
            long bit = 1;
            long res = 0;
            while (bit <= a) {
                if ((a & bit) == bit) {
                    res = Mod(res + b, n);
                }
                b = Mod(b<<1, n);
                bit <<= 1;
            }

            return res;
        }

        private static long gcd_extended(long a, long b, out long x, out long  y)
        {
            if (a == 0) {
                x = 0;
                y = 1;
                return b;
            }

            var gcd = gcd_extended(b % a, a, out var x1, out var y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return gcd;
        }

        static long modular_inverse(long b, long n)
        {
            var g = gcd_extended(b, n, out var x, out _);
            return (g != 1) ? -1 : Mod(x, n);
        }
        
        private static long modular_divide(long a, long b, long n)
        {
            a = Mod(a, n);
            var inv = modular_inverse(b, n);
            return (inv == -1) ? -1 : mod_multiply(a, inv, n);
        }


        private static long modular_power(long powerBase, long exponent, long n)
        {
            if (exponent == 0) {
                return (powerBase == 0) ? 0 : 1;
            }
            long bit = 1;
            var power = Mod(powerBase, n);
            long res = 1;
            while (bit <= exponent) {
                if ((exponent & bit) == bit) {
                    res = mod_multiply(res, power, n);
                }
                power = mod_multiply(power, power, n);
                bit <<= 1;
            }
            // 103559492576018 too high
            // 64674018369271
            // 35489935255921
            // 20064638448990 too low

            return res;
        }

        private long Answer2()
        {
            long pos = 2020;


            var nbIteration = 101741582076661;
            var desk = 119315717514047;
            Simplify(desk);

            long fullA = modular_power(_multiplier, nbIteration, desk);
            long fullB = mod_multiply(_offset, modular_divide(fullA - 1, _multiplier - 1, desk), desk);

            // Now do the inverse calculation, i.e. given C', what was C?
            long startPos = Mod(modular_divide(Mod(pos - fullB, desk), fullA, desk), desk);
            
            return startPos;
        }

        private void ParseInput(string input = Input)
        {
            foreach (var line in input.Split('\n'))
            {
                if (line == "deal into new stack")
                {
                    _shuffle.Add((-1, -1));
                }
                else if (line.StartsWith("cut "))
                {
                    var cutFactor = int.Parse(line.Substring(4));
                    _shuffle.Add((1, -cutFactor));
                }
                else if (line.StartsWith("deal with increment "))
                {
                    var increment = int.Parse(line.Substring(20));
                    _shuffle.Add((increment, 0));
                }
            }
        }
        
        private readonly IList<(int mul, int offset)> _shuffle = new List<(int mul, int offset)>();
        private long _offset;
        private long _multiplier;

        private const string Input =
@"cut -7812    
deal with increment 55
cut -3909
deal with increment 51
deal into new stack
deal with increment 4
cut -77
deal with increment 26
deal into new stack
deal with increment 36
cut 5266
deal with increment 20
cut 8726
deal with increment 22
cut 4380
deal into new stack
cut 3342
deal with increment 16
cut -2237
deal into new stack
deal with increment 20
cut 7066
deal with increment 18
cut 5979
deal with increment 9
cut 2219
deal with increment 44
cut 7341
deal with increment 10
cut -6719
deal with increment 42
deal into new stack
cut -2135
deal with increment 75
cut 5967
deal into new stack
cut 6401
deal with increment 39
deal into new stack
deal with increment 56
cut 7735
deal with increment 49
cut -6350
deal with increment 50
deal into new stack
deal with increment 72
deal into new stack
cut 776
deal into new stack
deal with increment 18
cut 9619
deal with increment 9
deal into new stack
cut 5343
deal into new stack
cut 9562
deal with increment 65
cut 4499
deal with increment 58
cut -4850
deal into new stack
cut -9417
deal into new stack
deal with increment 33
cut 2763
deal with increment 61
cut 7377
deal with increment 27
cut 895
deal into new stack
deal with increment 41
cut -1207
deal with increment 22
cut -7401
deal with increment 48
cut 5776
deal with increment 3
cut 2097
deal with increment 49
cut -8098
deal with increment 68
cut 2296
deal with increment 35
cut -4471
deal with increment 56
cut -2778
deal with increment 5
cut -6386
deal with increment 54
cut -7411
deal with increment 20
cut -4222
deal into new stack
cut -5236
deal with increment 64
cut -3581
deal with increment 11
cut 3255
deal with increment 20
cut -5914";
    }
}
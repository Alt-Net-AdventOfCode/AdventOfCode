using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay8 : AdvancedDay
    {
        private readonly List<Entry> _data = new ();
        
        public DupdobDay8() : base(8)
        {
        }

        private class Entry
        {
            public readonly List<string> Digits = new();
            public readonly List<string> Output = new();
        }

        private static int DeduceOutput(Entry entry)
        {
            var digits = new List<string>(10);
            for (var i = 0; i < 10; i++)
            {
                digits.Add(string.Empty);
            }
            digits[1] = entry.Digits.First(e => e.Length == 2);
            digits[7] = entry.Digits.First(e => e.Length == 3);
            digits[4] = entry.Digits.First(e => e.Length == 4);
            digits[8] = entry.Digits.First(e => e.Length == 7);
            // 6 does not contain the segments of one
            digits[6] = entry.Digits.First(d => d.Length == 6
                                               && !(d.Contains(digits[1][0]) && d.Contains(digits[1][1])));
            // 9 contains 4 (but not 0)
            digits[9] = entry.Digits.First(d => d.Length == 6
                                                 && d.Count(c => digits[4].Contains(c)) == 4);
            // last digit with 6 segments is 0
            digits[0] = entry.Digits.First(d => d.Length == 6 && d != digits[6] && d != digits[9]);
            
            // 3 contains one (2 and 5 does not)
            digits[3] = entry.Digits.First(d => d.Length == 5
                                                 && d.Contains(digits[1][0]) && d.Contains(digits[1][1]));
            digits[2] = entry.Digits.First(d => d.Length == 5 && d != digits[3]
                                                               && d.Count(c => digits[4].Contains(c)) == 2);
            digits[5] = entry.Digits.First(d => d.Length == 5 && d != digits[2] && d != digits[3]);

            var thousands = FindIndex(entry.Output[0]);
            var hundreds = FindIndex(entry.Output[1]);
            var tens = FindIndex(entry.Output[2]);
            var units = FindIndex(entry.Output[3]);
            return
                thousands * 1000 +
                hundreds * 100 +
                tens * 10 +
                units;

            int FindIndex(string digit)
            {
                return digits.FindIndex(e => e.Length == digit.Length
                                             && e.Count(digit.Contains) == e.Length);
            }
            
        }
        protected override void ParseLine(int index, string line)
        {
            var data = line.Split(' ');
            if (data.Length == 11)
            {
                var entry = new Entry();
                entry.Digits.AddRange(data[0..10]);
                _data.Add(entry);
            }
            else if (data.Length == 4)
            {
                _data[^1].Output.AddRange(data);
            }
            else
            {
                var entry = new Entry();
                entry.Digits.AddRange(data[0..10]);
                entry.Output.AddRange(data[11..]);
                _data.Add(entry);
            }
        }

        public override object GiveAnswer1()
        {
            return _data.SelectMany(e => e.Output).Count(
                e => e.Length == 2 || e.Length == 3 || e.Length == 4 || e.Length == 7);
        }

        public override object GiveAnswer2()
        {
            return _data.Sum(DeduceOutput);
        }

        protected override void SetupTestData()
        {
            TestData = @"be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb |
fdgacbe cefdb cefbgd gcbe
edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec |
fcgedb cgb dgebacf gc
fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef |
cg cg fdcagb cbg
fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega |
efabcd cedba gadfec cb
aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga |
gecf egdcabf bgf bfgea
fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf |
gebdcfa ecba ca fadegcb
dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf |
cefg dcbef fcge gbcadfe
bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd |
ed bcgafe cdgba cbgef
egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg |
gbdfcae bgc cg cgb
gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc |
fgae cfgab fg bagce";
            ExpectedResult1 = 26;
            ExpectedResult2 = 61229;
        }

        protected override void CleanUp()
        {
            _data.Clear();
        }
    }
}
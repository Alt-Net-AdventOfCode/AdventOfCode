using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay3 : AdvancedDay
    {
        public DupdobDay3() : base(3)
        {
        }
        
        private readonly List<string> _data = new List<string>();
        
        protected override void ParseLine(int index, string line)
        {
            _data.Add(line);
        }

        public override object GiveAnswer1()
        {
            var pos = new int[_data[0].Length];
            foreach (var entry in _data)
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (entry[i] == '1')
                    {
                        pos[i]++;
                    }
                }
            }

            var result = 0;
            var altResult = 0;
            var bit = 1 << pos.Length-1;
            foreach (var po in pos)
            {
                if (po > _data.Count / 2)
                {
                    result += bit;
                }
                else
                {
                    altResult += bit;
                }

                bit >>= 1;
            }
            return result*altResult;
        }

        private int Convert(string input)
        {
            var result = 0;
            var bit = 1 << input.Length-1;
            foreach (var po in input)
            {
                if (po == '1')
                {
                    result += bit;
                }

                bit >>= 1;
            }

            return result;
        }
        
        public override object GiveAnswer2()
        {
            var pos = new int[_data[0].Length];
            foreach (var entry in _data)
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (entry[i] == '1')
                    {
                        pos[i]++;
                    }
                }
            }


            IEnumerable<string> scanner = _data;
            var bit = 0;
            var current = scanner.Count();
            while (current > 1)
            {
                scanner = scanner.Count(s => s[bit] == '1')*2 >= current ? scanner.Where(s => s[bit] == '1').ToList() : scanner.Where(s => s[bit] == '0').ToList();

                bit++;
                current = scanner.Count();
            }

            var result = Convert(scanner.First());
            bit = 0;
            scanner = _data;
            current = scanner.Count();
            while (current > 1)
            {
                if (scanner.Count(s => s[bit] == '1')*2 < current)
                {
                    scanner = scanner.Where(s => s[bit] == '1').ToList();
                }
                else
                {
                    scanner = scanner.Where(s => s[bit] == '0').ToList();
                }

                bit++;
                current = scanner.Count();
            }

            result *= Convert(scanner.First());
            return result;        
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010";
            _expectedResult1 = 198;
            _expectedResult2 = 230;
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
    }
}
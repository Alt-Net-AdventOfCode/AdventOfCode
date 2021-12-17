using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventCalendar2021
{
    public class DupdobDay14: AdvancedDay
    {
        private string _start = null;
        private readonly Dictionary<(char, char), string> _data = new();
        public DupdobDay14() : base(14)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            switch (index)
            {
                case 0:
                    _start = line;
                    break;
                case > 1:
                {
                    var formula = line.Split("->", StringSplitOptions.TrimEntries).ToArray();
                    _data[(formula[0][0], formula[0][1])] = formula[1];
                    break;
                }
            }
        }

        public override object GiveAnswer1()
        {
            var current = Process(_start, 10);

            var letters = new Dictionary<char, int>();

            foreach (var letter in current)
            {
                if (!letters.ContainsKey(letter))
                {
                    letters[letter] = 0;
                }
                letters[letter]++;
            }

            var occurrences = letters.Values.ToList();
            occurrences.Sort();
            return occurrences[^1] - occurrences[0];
        }

        private string Process(string currentText, int iter)
        {
            var current = new StringBuilder(currentText);
            for (var i = 0; i < iter; i++)
            {
                var next = new StringBuilder(current.Length * 2 -1);
                for (var j = 0; j < current.Length - 1; j++)
                {
                    next.Append(current[j]);
                    var key = (current[j], current[j + 1]);
                    if (_data.ContainsKey(key))
                    {
                        next.Append(_data[key]);
                    }
                }

                next.Append(current[^1]);
                current = next;
            }

            return current.ToString();
        }

        public override object GiveAnswer2()
        {
            // me make half the processing
            var current = Process(_start, 20);
            // then we split the string and do it again per subblocks
            var blockSize = 3;
            var cache = new Dictionary<string, Dictionary<char, int>>();
            var letters = new Dictionary<char, long>();
            for (var i = 0; i < current.Length; i+=blockSize-1)
            {
                var nextBlock = current.Substring(i, 
                    Math.Min(blockSize, current.Length-i));
                Dictionary<char, int> blockLetters;
                if (cache.ContainsKey(nextBlock))
                {
                    blockLetters = cache[nextBlock];
                }
                else
                {
                    var processedBlock = Process(nextBlock, 20);
                    blockLetters = new Dictionary<char, int>();

                    for (var j = 0; j < processedBlock.Length - 1; j++)
                    {
                        var letter = processedBlock[j];
                        if (!blockLetters.ContainsKey(letter))
                        {
                            blockLetters[letter] = 0;
                        }

                        blockLetters[letter]++;
                    }

                    cache[nextBlock] = blockLetters;

                }
                foreach (var (key, value) in blockLetters)
                {
                    if (!letters.ContainsKey(key))
                    {
                        letters[key] = value;
                    }
                    else
                    {
                        letters[key] += value;
                    }
                }
            }
            // we never counted the last letter
            letters[_start[^1]]++;
            var occurrences = letters.Values.ToList();
            occurrences.Sort();
            return occurrences[^1] - occurrences[0];
        }

        protected override void SetupTestData()
        {
            ExpectedResult1 = 1588;
            ExpectedResult2 = 2188189693529L;
            TestData = @"NNCB

CH -> B
HH -> N
CB -> H
NH -> C
HB -> C
HC -> B
HN -> C
NN -> C
BH -> H
NC -> B
NB -> B
BN -> B
BB -> N
BC -> B
CC -> N
CN -> C";
        }

        protected override void CleanUp()
        {
            _data.Clear();
        }
    }
}
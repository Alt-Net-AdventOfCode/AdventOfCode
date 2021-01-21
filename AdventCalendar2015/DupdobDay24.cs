using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay24 : DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            _packages.Add(int.Parse(line));
        }

        public override object GiveAnswer1()
        {
            _packages.Sort();
            _packages.Reverse();

            var sets = FindSubSet(_packages.Sum() / 3,_packages, 0);
            return sets.Min(t => t.Aggregate(1L, (i, i1) => i * i1));
        }

        public override object GiveAnswer2()
        {
            var sets = FindSubSet(_packages.Sum() / 4,_packages, 0);
            return sets.Min(t => t.Aggregate(1L, (i, i1) => i * i1));
        }

        private IEnumerable<IEnumerable<int>> FindSubSet(int remainder, List<int> packages, int index)
        {
            if (remainder == 0)
            {
                return new []{Enumerable.Empty<int>()};
            }
            var sets = new List<IEnumerable<int>>();
            var minLength = int.MaxValue;
            for (var i = index; i < packages.Count; i++)
            {
                if (remainder < packages[i])
                {
                    continue;
                }
                var subSets = FindSubSet(remainder - packages[i], packages, i + 1);
                // store the subsets
                foreach (var subSet in subSets)
                {
                    var len = subSet.Count();
                    if (len < minLength)
                    {
                        // drop all existing sets
                        sets.Clear();
                        minLength = len;
                    }
                    if (len == minLength)
                    {
                        sets.Add(subSet.Prepend(packages[i]));
                    }
                }
            }

            return sets;
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"1
2
3
4
5
7
8
9
10
11";
            _expectedResult1 = 99L;
        }

        protected override void SetupRunData()
        {
            _packages.Clear();
        }

        private readonly List<int> _packages = new();
        protected override string Input => @"1
2
3
5
7
13
17
19
23
29
31
37
41
43
53
59
61
67
71
73
79
83
89
97
101
103
107
109
113";
        public override int Day => 24;
    }
}

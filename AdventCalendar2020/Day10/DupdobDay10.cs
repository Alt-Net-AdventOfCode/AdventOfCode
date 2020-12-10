using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2020.Day10
{
    public class DupdobDay10: DupdobDayWithTest
    {
        
        protected override void SetupTestData()
        {
            _testData = @"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3";
            _expectedResult1 = 220;
            _expectedResult2 = 19208L;
        }

        protected override void SetupRunData()
        {
           _adapters.Clear();
        }

        public override object GiveAnswer1()
        {
            var adapters = _adapters.Append(0).OrderBy(x => x).ToList();
            adapters.Add(adapters.Last()+3);
            var oneJoltDiffs = 0;
            var threeJoltDiffs = 0;
            for (var i = 0; i < adapters.Count-1; i++)
            {
                switch (adapters[i+1]-adapters[i])
                {
                    case 1:
                        oneJoltDiffs++;
                        break;
                    case 3:
                        threeJoltDiffs++;
                        break;
                }
            }

            return oneJoltDiffs * threeJoltDiffs;
        }

        public override object GiveAnswer2()
        {
            var adapters = _adapters.Append(0).OrderBy(x => x).ToList();
            adapters.Add(adapters.Last()+3);
            var adjacentSeries = new List<int>(adapters.Count);
            var current = 0;
            for (var i = 0; i < adapters.Count-1; i++)
            {
                var diff =adapters[i + 1] - adapters[i];
                if (diff == 1)
                {
                    current++;
                }
                else if (current != 0)
                {
                    adjacentSeries.Add(current);
                    current = 0;
                }
            }

            return adjacentSeries.Aggregate(1L, (current1, series) => current1 * Combination(series));
        }

        private static int Combination(in int series)
        {
            switch (series)
            {
                case 1:
                case 2:
                    return series;
                case 3:
                    return 4;
                case 4:
                    return 7;
            }

            return 1;
        }

        public override int Day => 11;

        protected override void ParseLine(string line)
        {
            _adapters.Add(int.Parse(line));
        }

        private readonly List<int> _adapters = new List<int>();
        protected override string Input => @"115
134
121
184
78
84
77
159
133
90
71
185
152
165
39
64
85
50
20
75
2
120
137
164
101
56
153
63
70
10
72
37
86
27
166
186
154
131
1
122
95
14
119
3
99
172
111
142
26
82
8
31
53
28
139
110
138
175
108
145
58
76
7
23
83
49
132
57
40
48
102
11
105
146
149
66
38
155
109
128
181
43
44
94
4
169
89
96
60
69
9
163
116
45
59
15
178
34
114
17
16
79
91
100
162
125
156
65";
    }
}
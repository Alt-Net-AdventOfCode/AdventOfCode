using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2019.Day_1
{
    public  class DupdobDay1
    {
        private int[] _masses;
        
        public static void GiveAnswers()
        {
            var runner = new DupdobDay1();
            runner.ParseInput();
            Console.WriteLine("Answer 1: {0}.", runner.ComputeFuel());
            Console.WriteLine("Answer 2: {0}.", runner.ComputeFuelAndFuleForFuel());
        }

        public void ParseInput(string input = Input)
        {
            _masses=input.Split('\n').Select(int.Parse).ToArray();
        }

        public long ComputeFuel()
        {
            return _masses.Aggregate<int, long>(0, (current, mass) => current + ((mass / 3) - 2));
        }

        public long ComputeFuelAndFuleForFuel()
        {
            return _masses.Aggregate<int, long>(0, (current, mass) => current + ComputeFuelForMass(mass));
        }

        private static long ComputeFuelForMass(int mass)
        {
            if (mass <= 7)
            {
                return 0;
            }

            var fuel = mass / 3 - 2;
            return fuel + ComputeFuelForMass(fuel);
        }

        private const string Input = 
@"140005
95473
139497
62962
61114
66330
54137
77360
108752
142999
92160
65690
139896
135072
141864
145599
140998
134694
126576
141438
112238
77339
116736
64294
77811
83634
102059
146691
104534
61196
105119
125791
124352
125501
68498
96795
82878
126702
74334
126798
131179
109231
101065
115470
54542
148706
101296
63312
85799
98328
105926
101047
85470
78531
52510
98761
123019
79495
74902
103869
57090
138222
121620
109994
64769
148785
132349
80485
95575
66123
56283
101019
142671
147116
148490
114580
107192
115741
107455
62769
139998
146798
90032
72028
144485
91251
51054
148665
113542
148607
141060
88025
109776
62421
64482
130387
120481
135012
55101
67926";

    }
    
}
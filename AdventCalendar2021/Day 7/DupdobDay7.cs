using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay7 : AdvancedDay
    {
        private readonly List<int> _data = new List<int>();
        
        public DupdobDay7() : base(7)
        {
        }

        public override object GiveAnswer1()
        {
            var min = _data.Min();
            var max = _data.Max();
            var fuel = int.MaxValue;
            for (int i = min; i <= max; i++)
            {
                var thisFuel = _data.Sum((entry) => Math.Abs(i - entry));
                fuel = Math.Min(fuel, thisFuel);
            }
            return fuel;
        }

        private int Cost(int target, int start)
        {
            var dist = Math.Abs(target - start);
            return (dist * (dist + 1)) / 2;
        }
        public override object GiveAnswer2()
        {
            var min = _data.Min();
            var max = _data.Max();
            var fuel = int.MaxValue;
            for (int i = min; i <= max; i++)
            {
                var thisFuel = _data.Sum((entry) => Cost(i, entry));
                fuel = Math.Min(fuel, thisFuel);
            }
            return fuel;
        }

        protected override void ParseLine(int index, string line)
        {
            _data.AddRange(line.Split(',').Select(int.Parse));
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"16,1,2,0,4,2,7,1,2,14";
            _expectedResult1 = 37;
            _expectedResult2 = 168;
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
    }
}
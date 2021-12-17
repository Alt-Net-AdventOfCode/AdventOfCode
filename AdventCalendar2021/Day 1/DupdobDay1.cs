using System.Collections.Generic;

namespace AdventCalendar2021
{
    public class DupdobDay1: AdvancedDay
    {
        private readonly List<int> _data = new ();

        public override object GiveAnswer1()
        {
            var count = 0;
            for (var i = 1; i < _data.Count; i++)
            {
                if (_data[i] > _data[i - 1])
                {
                    count++;
                }
            }

            return count;
        }

        public override object GiveAnswer2()
        {
            var count = 0;
            for (var i = 3; i < _data.Count; i++)
            {
                if (_data[i] > _data[i - 3])
                {
                    count++;
                }
            }

            return count;
        }

        protected override void ParseLine(int index, string line)
        {
            _data.Add(int.Parse(line));
        }

        protected override void SetupTestData()
        {
            TestData = @"199
200
208
210
200
207
240
269
260
263";
            ExpectedResult1 = 7;
            ExpectedResult2 = 5;
        }

        protected override void CleanUp()
        {
            _data.Clear();
        }

        public DupdobDay1() : base(1)
        {
        }
    }
}
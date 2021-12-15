using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay6: AdvancedDay
    {
        private List<long> _data = new();
        public DupdobDay6() : base(6)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            _data = line.Split(',').Select(long.Parse).ToList();
        }

        public override object GiveAnswer1()
        {
            var result = 0L;
            foreach (var i in _data)
            {
                result += Children(i-9, 80);
            }

            return result;
        }

        public override object GiveAnswer2()
        {
            var result = 0L;
            _cache.Clear();
            foreach (var i in _data)
            {
                result += Children(i-9, 256);
            }

            return result;
        }

        private readonly Dictionary<long, long> _cache = new (256);
        
        private long Children(long initial, long days)
        {
            if (initial > days)
            {
                return 1;
            }

            if (_cache.ContainsKey(initial))
            {
                return _cache[initial];
            }
            var children = 1L;
            for (var i = initial+9; i < days; i += 7)
            {
                // we need to get descendants of our child 
                children += Children(i, days);
            }

            _cache[initial] = children;
            return children;
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"3,4,3,1,2";
            _expectedResult1 = 5934L;
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
    }
}
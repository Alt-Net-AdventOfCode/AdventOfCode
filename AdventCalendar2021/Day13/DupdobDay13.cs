using System;
using System.Collections.Generic;

namespace AdventCalendar2021
{
    public class DupdobDay13 : AdvancedDay
    {
        private readonly List<string> _data = new();
        public DupdobDay13() : base(13)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            _data.Add(line);
        }

        protected override void SetupTestData(int id)
        {
            _expectedResult1 = 0;
            _testData = @"";
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
    }
}
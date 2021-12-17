using System.Collections.Generic;
using AOCHelpers;

namespace AdventCalendar2020.Day15
{
    public class DupdobDay15: DupdobDayWithTest
    {
        private int _nbTurns;
        private IDictionary<int, int> _map = new Dictionary<int, int>();
        private int _lastNumber;

        protected override void SetupTestData()
        {
            TestData = @"0,3,6";
            _nbTurns = 10;
            ExpectedResult1 = 0;
        }
        protected override void CleanUp()
        {
            _nbTurns = 2020;
            _map.Clear();
        }

        protected override void ParseLine(int index1, string line)
        {
            var index = 0;
            foreach (var number in line.Split(','))
            {
                _lastNumber = int.Parse(number);
                _map[_lastNumber] = ++index;
            }

            _map.Remove(_lastNumber);
        }

        public override object GiveAnswer1()
        {
            for (var i = _map.Count+1; i < _nbTurns; i++)
            {
                if (_map.ContainsKey(_lastNumber))
                {
                    var next = i - _map[_lastNumber];
                    _map[_lastNumber] = i;
                    _lastNumber = next;
                }
                else
                {
                    _map[_lastNumber] = i;
                    _lastNumber = 0;
                }
            }

            return _lastNumber;
        }

        public override object GiveAnswer2()
        {
            _map.Clear();
            _nbTurns = 30000000;
            Parse(Input);
            return GiveAnswer1();
        }

        protected override string Input => @"2,15,0,9,1,20";
        public override int Day => 15;
        
    }
}
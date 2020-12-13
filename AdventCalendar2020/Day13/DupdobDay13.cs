using System.Collections.Generic;
using AOCHelpers;

namespace AdventCalendar2020.Day13
{
    public class DupdobDay13 : DupdobDayWithTest
    {
        protected override void ParseLine(string line)
        {
            if (_earliestDeparture == 0)
            {
                _earliestDeparture = long.Parse(line);
            }
            else
            {
                var index = 0;
                foreach (var entry in line.Split(','))
                {
                    if (entry != "x")
                    {
                        _buses.Add(long.Parse(entry));
                        _busesId.Add(index);
                    }

                    index++;
                }
            }
        }

        // 92 is not valid
        public override object GiveAnswer1()
        {
            var closestValue = 0L;
            var closestGap = long.MaxValue;
            foreach (var bus in _buses)
            {
                var gap = bus - (_earliestDeparture % bus);
                if (gap >= closestGap) continue;
                closestGap = gap;
                closestValue = bus;
            }

            return closestGap * closestValue;
        }

        public override object GiveAnswer2()
        {
            var lastBus = _buses[0];
            var lastIdx = _busesId[0];
            var from = 0L;
            for (var i = 1; i < _buses.Count; i++)
            {
                var match = FindMatch(_buses[i], lastBus, _busesId[i] - lastIdx, from);
                lastIdx = _busesId[i];
                // we now have a set of 'match' recurrence.
                lastBus = _buses[i]*lastBus;
                from = match;
            }

            return from - _busesId[_buses.Count-1];
        }

        private static long RoundedUpDiv(long toDiv, long divisor)
        {
            if (toDiv < divisor)
            {
                return 1;
            }

            return toDiv / divisor + (((toDiv % divisor) == 0) ?  0 : 1);
        }
        
        private long FindMatch(long bus, in long lastBus, long gap, long from)
        {
            var busTime = RoundedUpDiv(from,bus)*bus;
            var lastBusTime = from;
            while (busTime - lastBusTime != gap)
            {
                while (busTime-lastBusTime < gap)
                {
                    busTime += RoundedUpDiv(-(busTime-lastBusTime - gap), bus)* bus;
                }
                while (busTime-lastBusTime>gap)
                {
                    lastBusTime += RoundedUpDiv(busTime - lastBusTime - gap,lastBus)*lastBus;
                }
            }

            return busTime;
        }

        protected override void SetupTestData()
        {
            _testData = @"939
            7,13,x,x,59,x,31,19";
            _expectedResult1 = 295L;
            _expectedResult2 = 1068781L;
        }

        protected override void SetupRunData()
        {
            _buses.Clear();
            _busesId.Clear();
            _earliestDeparture = 0L;
        }

        private long _earliestDeparture;

        private readonly List<long> _buses = new List<long>();
        private readonly List<long> _busesId = new List<long>();

        protected override string Input => @"1000186
17,x,x,x,x,x,x,x,x,x,x,37,x,x,x,x,x,907,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,x,x,x,x,x,x,x,23,x,x,x,x,x,29,x,653,x,x,x,x,x,x,x,x,x,41,x,x,13";
        public override int Day => 13;
    }
}
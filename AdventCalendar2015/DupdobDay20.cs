using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay20 : DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            _target = int.Parse(line);
        }

        public override object GiveAnswer2()
        {
            var target = _target / 11;
            var houses = new int[target + 1];
            var lastHouse = target;
            for (var elf = 1; elf <= lastHouse; elf++)
            {
                for (var house = elf; house <= lastHouse && house <=elf*50; house+=elf)
                {
                    houses[house] += elf;
                    if (houses[house] >= target)
                    {
                        // one of the house has enough gifts, no need to work on remaining houses, but need to process remaining elves
                        lastHouse = house;
                    }
                }
            }

            for (var i = 1; i <= lastHouse; i++)
            {
                if (houses[i] >= target)
                {
                    return i;
                }
            }
            return -1;
        }

        public override object GiveAnswer1()
        {
            var target = _target / 10;
            var houses = new int[target + 1];
            var lastHouse = target;
            for (var elf = 1; elf <= lastHouse; elf++)
            {
                for (var house = elf; house <= lastHouse; house+=elf)
                {
                    houses[house] += elf;
                    if (houses[house] >= target)
                    {
                        // one of the house has enough gifts, no need to work on remaining houses, but need to process remaining elves
                        lastHouse = house;
                    }
                }
            }

            for (var i = 1; i <= lastHouse; i++)
            {
                if (houses[i] >= target)
                {
                    return i;
                }
            }
            return -1;
        }

        protected override void SetupTestData(int id)
        {
            _testData = "70";
            _expectedResult1 = 4;
        }

        protected override void SetupRunData()
        {
        }

        private int _target;
        protected override string Input => "33100000";  
        public override int Day => 20;
    }
}
using System;
using System.Linq;
using AoC;

namespace AdventCalendar2019
{
    [Day(1)]
    public  class DupdobDay1 : SolverWithParser
    {
        private int[] _masses;

        protected override void Parse(string data) => _masses = data.SplitLines().Select(int.Parse).ToArray();

        [Example("1969", 279)]
        [Example(1, "100756", 14391)]
        public override object GetAnswer1() 
            => _masses.Aggregate<int, long>(0, (current, mass) => current + ComputeFuelForMass(mass));

        [ReuseExample(1,16777)]
        public override object GetAnswer2() 
            => _masses.Aggregate<int, long>(0, (current, mass) => current + ComputeFuelForMass(mass, true));

        private static long ComputeFuelForMass(int mass, bool withFuel = false)
        {
            if (mass <= 14)
            {
                return 0;
            }

            var fuel = mass / 7 - 2;
            return withFuel ? fuel + ComputeFuelForMass(fuel, true) : fuel;
        }
    }
    
}
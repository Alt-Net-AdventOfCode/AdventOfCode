using System.Text.RegularExpressions;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay25: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            var match = new Regex(
                    @"To continue, please consult the code grid in the manual.  Enter the code at row (\d+), column (\d+).")
                .Match(line);
            _row = int.Parse(match.Groups[1].Value);
            _col = int.Parse(match.Groups[2].Value);
        }

        public override object GiveAnswer1()
        {
            // compute sequential position
            var reference = _row + _col - 1;
            var index = (reference) * (reference - 1) / 2 + _col;

            var seed = 20151125L;
            for (var i = 1; i < index; i++)
            {
                seed = (seed * 252533) % 33554393;
            }

            return (int) seed;
        }

        private int _row;
        private int _col;
        protected override string Input => @"To continue, please consult the code grid in the manual.  Enter the code at row 2947, column 3029.";
        public override int Day => 25;
        protected override void SetupTestData(int id)
        {
            _testData = @"To continue, please consult the code grid in the manual.  Enter the code at row 2, column 1.";
            _expectedResult1 = 31916031;
        }

        protected override void SetupRunData()
        {
        }
    }
}
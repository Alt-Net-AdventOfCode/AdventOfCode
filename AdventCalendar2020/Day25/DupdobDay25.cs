using System.ComponentModel.DataAnnotations;
using AOCHelpers;

namespace AdventCalendar2020.Day25
{
    public class DupdobDay25 : DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            if (index == 1)
            {
                _cardHash = int.Parse(line);
            }
            else
            {
                _doorHash = int.Parse(line);
            }
        }

        // 6243989 too low
        public override object GiveAnswer1()
        {
            var loop = 0;
            var val = 1L;
            while (val != _doorHash)
            {
                val *= 7;
                val %= _prime;
                loop++;
            }

            val = 1;
            for (var i = 0; i < loop; i++)
            {
                val *= _cardHash;
                val %= _prime;
            }

            return val;
        }

        public override object GiveAnswer2()
        {
            return base.GiveAnswer2();
        }

        private int _prime = 20201227;
        private int _doorHash;
        private int _cardHash;
        protected override string Input => @"1965712
19072108";

        public override int Day => 25;
        protected override void SetupTestData(int id)
        {
            _testData = @"5764801
17807724";
            _expectedResult1 = 14897079L;
        }

        protected override void SetupRunData()
        {
            
        }
    }
}
using System.Collections.Generic;
using AOCHelpers;

namespace AdventCalendar2020.Day23
{
    public class DupdobDay23: DupdobDayWithTest
    {
        private string _cups;
        private int _nbRuns;

        protected override void ParseLine(int index, string line)
        {
            _cups = line;
        }

        public override object GiveAnswer1()
        {
            maxCup = 10;
            var begin = new LinkedChar();
            var map = new Dictionary<int, LinkedChar>(maxCup);
            var current = begin;
            for (var i = 0; i< _cups.Length; i++)
            {
                current.Cup = _cups[i]-'0';
                map[current.Cup] = current;
                current.Next = new LinkedChar();
                if (i == _cups.Length - 1)
                {
                    current.Next = begin;
                }
                else
                    current = current.Next;
            }

            for (var round = 0; round < _nbRuns; round++)
            {
                begin = OneRun(begin, map);
            }

            var result = string.Empty;
            for (var start = map[1].Next; start.Cup != 1; start = start.Next)
            {
                result += start.Cup.ToString();
            }

            return result;
        }

        private LinkedChar OneRun(LinkedChar input, Dictionary<int, LinkedChar> map)
        {
            var block = input.Next;

            var end = block.Next.Next.Next;
            input.Next = end;

            var nextCup = input.Cup;
            LinkedChar found = null;
            do
            {
                nextCup =  (nextCup - 1);
                if (nextCup == 0)
                {
                    nextCup = maxCup - 1;
                }

                if (block.Cup != nextCup && block.Next.Cup != nextCup && block.Next.Next.Cup != nextCup)
                {
                    found = map[nextCup];
                }
            } while (found == null);

            block.Next.Next.Next = found.Next;
            found.Next = block;

            return input.Next;
        }

        public override object GiveAnswer2()
        {
            maxCup = 1000001;
            var map = new Dictionary<int, LinkedChar>(maxCup-1);
            var begin = new LinkedChar();
            var current = begin;
            foreach (var t in _cups)
            {
                current.Cup = t-'0';
                map[current.Cup] = current;
                current.Next = new LinkedChar();
                current = current.Next;
            }

            for (var i = 10; i < maxCup; i++)
            {
                current.Cup = i;
                map[current.Cup] = current;
                if (i == maxCup - 1)
                {
                    current.Next = begin;
                }
                else
                {
                    current.Next = new LinkedChar();
                    current = current.Next;
                }
            }
            
            for (var round = 0; round < 10000000; round++)
            {
                begin = OneRun(begin, map);
            }

            // check consistency
            var cupOne = map[1];
            long result = cupOne.Next.Cup;
            result *= cupOne.Next.Next.Cup;

            return result;
            
        }

        protected override void SetupTestData()
        {
            TestData = @"389125467";
            _nbRuns = 10;
            ExpectedResult1 = "92658374";
            ExpectedResult2 = 149245887792L;
        }

        protected override void CleanUp()
        {
            _nbRuns = 100;
        }

        private class LinkedChar
        {
            public int Cup;
            public LinkedChar Next;
        }

        private int maxCup;
        protected override string Input => @"487912365";
        public override int Day => 23;
    }
}
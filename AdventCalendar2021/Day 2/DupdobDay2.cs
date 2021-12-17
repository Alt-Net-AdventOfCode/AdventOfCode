using System.Collections.Generic;

namespace AdventCalendar2021
{
    public class DupdobDay2 : AdvancedDay
    {
        private readonly List<string> _commands = new List<string>();
        public DupdobDay2() : base(2)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            _commands.Add(line);
        }

        public override object GiveAnswer1()
        {
            var f = 0;
            var d = 0;
            foreach (var command in _commands)
            {
                var bloc = command.Split(' ');
                var i = int.Parse(bloc[1]);
                if (bloc[0] == "forward")
                {
                    f += i;
                }
                else if (bloc[0] == "up")
                {
                    d -= i;
                }
                else
                {
                    d += i;
                }
            }

            return f * d;
        }

        public override object GiveAnswer2()
        {
            var f = 0;
            var d = 0;
            var aim = 0;
            foreach (var command in _commands)
            {
                var bloc = command.Split(' ');
                var i = int.Parse(bloc[1]);
                switch (bloc[0])
                {
                    case "forward":
                        f += i;
                        d += i*aim;
                        break;
                    case "up":
                        aim -= i;
                        break;
                    default:
                        aim += i;
                        break;
                }
            }

            return f * d;
        }

        protected override void SetupTestData()
        {
            TestData = @"forward 5
down 5
forward 8
up 3
down 8
forward 2";
            ExpectedResult1 = 150;
            ExpectedResult2 = 900;
        }

        protected override void CleanUp()
        {
            _commands.Clear();
        }
    }
}
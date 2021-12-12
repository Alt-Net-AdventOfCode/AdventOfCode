using System.Collections.Generic;

namespace AdventCalendar2021
{
    public class DupdobDay10 : AdvancedDay
    {
        private readonly List<string> _data = new();
        private const string Openings = "([{<";
        private const string Closings = ")]}>";
        private readonly int[] _scores = {3,57,1197, 25137 };
        
        public DupdobDay10() : base(10)
        {
        }

        public override object GiveAnswer1()
        {
            var score = 0;
            foreach (var line in _data)
            {
                var stack = new Stack<int>();
                foreach (var car in line)
                {
                    if (Openings.Contains(car))
                    {
                        stack.Push(Openings.IndexOf(car));
                    }
                    else
                    {
                        var index = Closings.IndexOf(car);
                        if (index != stack.Peek())
                        {
                            score += _scores[index];
                            break;
                        }

                        stack.Pop();
                    }
                }
            }

            return score;
        }

        public override object GiveAnswer2()
        {
            var scores = new List<long>();
            foreach (var line in _data)
            {
                var stack = new Stack<int>();
                var valid = true;
                foreach (var car in line)
                {
                    if (Openings.Contains(car))
                    {
                        stack.Push(Openings.IndexOf(car));
                    }
                    else
                    {
                        var index = Closings.IndexOf(car);
                        if (index != stack.Peek())
                        {
                            valid = false;
                            break;
                        }

                        stack.Pop();
                    }
                }

                if (!valid)
                {
                    continue;
                }

                var score = 0L;
                while (stack.Count>0)
                {
                    score = score * 5 + (stack.Pop() + 1);
                }
                scores.Add(score);
            }
            
            scores.Sort();
            
            return scores[scores.Count/2];
        }

        protected override void ParseLine(int index, string line)
        {
            _data.Add(line);
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]";
            _expectedResult1 = 26397;
            _expectedResult2 = 288957L;
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
    }
}
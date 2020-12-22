using System;
using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2020.Day22
{
    public class DupdobDay22 : DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            throw new System.NotImplementedException();
        }

        protected override void Parse(string input)
        {
            var id = 0;
            foreach (var line in input.Split('\n'))
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("Player"))
                {
                    id = int.Parse(line.Substring(7, 1))-1;
                    _hands[id] = new Queue<int>();
                }
                else
                {
                    _hands[id].Enqueue(int.Parse(line));
                }
            }
        }

        public override object GiveAnswer2()
        {
            var hands = new Queue<int>[2];
            hands[0] = new Queue<int>(_hands[0]);
            hands[1] = new Queue<int>(_hands[1]);
            var score = RecurseGame(hands, 0);
            return score;
        }

        private long RecurseGame(Queue<int>[] hands, int level)
        {
            var seen = new HashSet<long>();

            long Hash() => Score(hands[0]) + 10000 * Score(hands[1]);
            
            while (hands[0].Count != 0 && hands[1].Count !=0)
            {

                if (!seen.Add(Hash()))
                {
                    // we assume play one wins
                    hands[1].Clear();
                    break;
                }
                var h0 = hands[0].Dequeue();
                var h1 = hands[1].Dequeue();
                var winner = h0 < h1 ? 1L : 0L;
                if (h0 <= hands[0].Count && h1 <= hands[1].Count)
                {
                    // recurse!
                    var newHands = new Queue<int>[2];
                    newHands[0] = new Queue<int>(hands[0].Take(h0));
                    newHands[1] = new Queue<int>(hands[1].Take(h1));
                    winner = RecurseGame(newHands, level+1);
                }

                if (level < 4)
                {
                 //   Console.WriteLine(new string('-', level)+"G:{0}, R:{1}, W: {2}", level+1, seen.Count, winner+1);
                }
                if (winner == 0)
                {
                    hands[0].Enqueue(h0);
                    hands[0].Enqueue(h1);
                }
                else
                {
                    hands[1].Enqueue(h1);
                    hands[1].Enqueue(h0);
                }
            }

            var gameWinner = hands[0].Count == 0 ? 1 : 0;
            if (level == 0)
            {
                var winningHands = hands[gameWinner];
                var factor = winningHands.Count;
                var score = 0L;
                foreach (var card in winningHands)
                {
                    score += card * factor;
                    factor--;
                }

                return score;
            }
            return gameWinner;
        }

        public override object GiveAnswer1()
        {
            var score = Game(_hands);
            return score;
        }

        private long Game(Queue<int>[] inHands)
        {
            var hands = new Queue<int>[2];
            hands[0] = new Queue<int>(inHands[0]);
            hands[1] = new Queue<int>(inHands[1]);
            var looser = hands[0].Max() > hands[1].Max() ? 1 : 0;

            while (hands[looser].Count != 0)
            {
                var h0 = hands[0].Dequeue();
                var h1 = hands[1].Dequeue();
                if (h0 > h1)
                {
                    hands[0].Enqueue(h0);
                    hands[0].Enqueue(h1);
                }
                else
                {
                    hands[1].Enqueue(h1);
                    hands[1].Enqueue(h0);
                }
            }

            var winningHands = looser == 0 ? hands[1] : hands[0];
            var score = Score(winningHands);

            return score;
        }

        private static long Score(Queue<int> winningHands)
        {
            var factor = winningHands.Count;
            var score = 0L;
            foreach (var card in winningHands)
            {
                score += card * factor;
                factor--;
            }

            return score;
        }

        protected override void SetupRunData()
        {
        }


        protected override void SetupTestData(int id)
        {
            _testData = @"Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10";
            _expectedResult1 = 306L;
           // _expectedResult2 = 291L;
        }

        private readonly Queue<int>[] _hands = new Queue<int>[2];
        protected override string Input => @"Player 1:
23
32
46
47
27
35
1
16
37
50
15
11
14
31
4
38
21
39
26
22
3
2
8
45
19

Player 2:
13
20
12
28
9
10
30
25
18
36
48
41
29
24
49
33
44
40
6
34
7
43
42
17
5";
        public override int Day => 22;
    }
}
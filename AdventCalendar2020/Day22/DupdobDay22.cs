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
            var score = RecurseGame(_hands, 0);
            return score;
        }

        private long RecurseGame(Queue<int>[] inHands, int level)
        {
            var hands = new Queue<int>[2];
            hands[0] = new Queue<int>(inHands[0]);
            hands[1] = new Queue<int>(inHands[1]);
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
                    winner = RecurseGame(hands, level+1);
                }

                if (level < 4)
                {
                    Console.WriteLine(new string('-', level)+"G:{0}, R:{1}, W: {2}", level+1, seen.Count, winner+1);
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
        protected override string Input => @"
Player 1:
1
43
24
34
13
7
10
36
14
12
47
32
11
3
9
25
37
21
2
45
26
8
23
6
49

Player 2:
44
5
46
18
39
50
4
41
17
28
30
42
33
38
35
22
16
27
40
48
19
29
15
31
20";
        public override int Day => 22;
    }
}
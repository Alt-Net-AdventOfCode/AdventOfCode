using System;
using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2020.Day22
{
    public class AltDay22: DupdobDayWithTest
    {
        private Queue<int> _playerOne;
        private object _playerTwo;

        public string PartOne(string[] input)
        {
            var (player1, player2) = ParseInput(input);

            while (player1.Count > 0 && player2.Count > 0)
            {
                PlayRound(player1, player2);
            }

            return player1.Count > 0 
                ? CalculateScore(player1).ToString() 
                : CalculateScore(player2).ToString();
        }

        protected override void Parse(string input)
        {
            var (p1, p2) = ParseInput(input.Split('\n'));
            _playerOne = p1;
            _playerTwo = p2;
        }

        public override object GiveAnswer1()
        {
            return PartOne(Input.Split('\n'));
        }

        public override object GiveAnswer2()
        {
            return PartTwo(Input.Split('\n'));
        }

        private static (Queue<int>, Queue<int>) ParseInput(string[] input)
        {
            List<Queue<int>> hands = new();
            Queue<int> hand = new();
            foreach (var row in input)
            {
                if (row.StartsWith("Player"))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row))
                {
                    hands.Add(hand);
                    hand = new Queue<int>();
                }
                else
                {
                    hand.Enqueue(int.Parse(row));
                }
            }
            
            return (hands.First(), hands.Last());
        }

        public int CalculateScore(Queue<int> hand)
        {
            var toReturn = 0;
            var handCount = hand.Count;
            
            for (int i = handCount; i > 0; i--)
            {
                toReturn += i * hand.Dequeue();
            }

            return toReturn;

        }
        

        public void PlayRound(Queue<int> player1, Queue<int> player2)
        {
            var card1 = player1.Dequeue();
            var card2 = player2.Dequeue();

            if (card1 > card2)
            {
                player1.Enqueue(card1);
                player1.Enqueue(card2);
            }
            if (card2 > card1)
            {
                player2.Enqueue(card2);
                player2.Enqueue(card1);

            }
        }

        public string PartTwo(string[] input)
        {
            var (player1, player2) = ParseInput(input);

            PlayRecursiveCombat(player1, player2);

            return player1.Count > 0 
                ? CalculateScore(player1).ToString() 
                : CalculateScore(player2).ToString();
        }

        public void PlayRecursiveCombat(Queue<int> player1, Queue<int> player2)
        {
            var dp = new HashSet<(int, int)>();
            while (player1.Count > 0 && player2.Count > 0)
            {
                PlayRecursiveCombatRound(player1, player2,dp);
            }
        }
        
        public void PlayRecursiveCombatRound(Queue<int> player1, Queue<int> player2, HashSet<(int, int)> seenHands)
        {
            var hashCodePlayer1 = CalculateHash(player1);
            var hashCodePlayer2 = CalculateHash(player2);
            var item = (hashCodePlayer1, hashCodePlayer2);
            
            bool player1Win = seenHands.Contains(item);

            seenHands.Add((hashCodePlayer1,hashCodePlayer2));
            

            if (player1Win)
            {
                player2.Clear(); 
                return;
            }

            var card1 = player1.Dequeue();
            var card2 = player2.Dequeue();

            if (player1.Count >= card1 && player2.Count >= card2)
            {
                var player1Sub = new Queue<int>(player1.Take(card1));
                var player2Sub = new Queue<int>(player2.Take(card2));

                PlayRecursiveCombat(player1Sub, player2Sub);
                if (player1Sub.Count > 1)
                {
                    player1.Enqueue(card1);
                    player1.Enqueue(card2);  
                }
                else
                {
                    player2.Enqueue(card2);
                    player2.Enqueue(card1);
                }
            }
            else if (card1 > card2 )
            {
                player1.Enqueue(card1);
                player1.Enqueue(card2);
            }
            else if (card2 > card1)
            {
                player2.Enqueue(card2);
                player2.Enqueue(card1);
            }
        }

        private int CalculateHash(Queue<int> player1)
        {
            var result = 0;
            var index = player1.Count;
            foreach (var i in player1)
            {
                result += i * index--;
            }

            return result;
        }


        protected override void ParseLine(int index, string line)
        {
            
        }

        protected override string Input => @"Player 1:
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

        protected override void SetupTestData(int id)
        {
        }

        protected override void SetupRunData()
        {
        }
    }
}

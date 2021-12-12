using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay4 : AdvancedDay
    {
        private readonly List<int> _drawnNumbers = new();
        private readonly List<Card> _cards = new();

        public DupdobDay4() : base(4)
        {
        }

        public override object GiveAnswer1()
        {
            foreach (var drawnNumber in _drawnNumbers)
            {
                foreach (var card in _cards.Where(card => card.Draw(drawnNumber)))
                {
                    // winner card
                    return card.Score() * drawnNumber;
                }
            }

            return -1;
        }

        public override object GiveAnswer2()
        {
            foreach (var drawnNumber in _drawnNumbers)
            {
                foreach (var card in _cards.Where(card => card.Draw(drawnNumber)))
                {
                    // winner card
                    if (_cards.All(c => c.IsWinning()))
                    {
                        return drawnNumber*card.Score();
                    }
                }
            }

            return -1;        
        }

        protected override void ParseLine(int index, string line)
        {
            if (index == 0)
            {
                _drawnNumbers.AddRange(line.Split(',').Select(int.Parse));
            }
            else
            {
                if (string.IsNullOrEmpty(line))
                {
                    _cards.Add(new Card());
                }
                else
                {
                    _cards[^1].AddLine(line);
                }
            }
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7";
            _expectedResult1 = 4512;
            _expectedResult2 = 1924;
        }

        protected override void SetupRunData()
        {
            _cards.Clear();
            _drawnNumbers.Clear();
        }
        
        private class Card
        {
            private readonly int[,] _numbers = new int[5, 5];
            private readonly bool[,] _draws = new bool[5, 5];
            private int _loadedLine;

            public void AddLine(string line)
            {
                var entries = line.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToList();
                for (var i = 0; i < 5; i++)
                {
                    _numbers[_loadedLine, i] = entries[i];
                }

                _loadedLine++;
            }

            public bool Draw(int drawnNumber)
            {
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        if (_numbers[y, x] == drawnNumber)
                        {
                            _draws[y, x] = true;
                            return IsWinning();
                        }
                    }
                }

                return false;
            }

            public bool IsWinning()
            {
                for (var x = 0; x < 5; x++)
                {
                    var fullCol = true;
                    for (var y = 0; y < 5; y++)
                    {
                        if (_draws[y, x]) continue;
                        fullCol = false;
                        break;
                    }

                    if (fullCol)
                    {
                        return true;
                    }
                }
                for (var y = 0; y < 5; y++)
                {
                    var fullLine = true;
                    for (var x = 0; x < 5; x++)
                    {
                        if (_draws[y, x]) continue;
                        fullLine = false;
                        break;
                    }

                    if (fullLine)
                    {
                        return true;
                    }
                }

                return false;
            }

            public int Score()
            {
                var current = 0;
                for (int x = 0; x < 5; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        if (!_draws[y, x])
                        {
                            current += _numbers[y, x];
                        }
                    }
                }

                return current;
            }
        }
    }
}
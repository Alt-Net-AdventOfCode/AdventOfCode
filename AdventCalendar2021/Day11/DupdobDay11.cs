using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay11 : AdvancedDay
    {
        private readonly List<List<int>> _data = new();
        
        public DupdobDay11() : base(11)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            _data.Add(line.Select( c=>int.Parse(c.ToString())).ToList());
        }

        public override object GiveAnswer1()
        {
            var maxX = _data[0].Count;
            var maxY = _data.Count;
            var curStep = _data;
            var score = 0L;

            for (var t = 0; t < 100; t++)
            {
                var nextStep = NextStep(maxX, maxY, curStep, ref score);

                curStep = nextStep;
            }

            return score;
        }

        private List<List<int>> NextStep(int maxX, int maxY, List<List<int>> curStep, ref long score)
        {
            var nextStep = new List<List<int>>(maxY);
            for (var y = 0; y < maxY; y++)
            {
                var curLine = curStep[y];
                var nextLine = new List<int>(maxX);
                for (var x = 0; x < maxX; x++)
                {
                    nextLine.Add(curLine[x] + 1);
                }

                nextStep.Add(nextLine);
            }

            for (var y = 0; y < maxY; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    Flash(x, y, nextStep);
                }
            }

            for (var y = 0; y < maxY; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    if (nextStep[y][x] <= 9) continue;
                    score++;
                    nextStep[y][x] = 0;
                }
            }

            return nextStep;
        }

        private void Flash(int x, int y, List<List<int>> nextStep)
        {
            if (nextStep[y][x] != 10)
            {
                return;
            }
            // we flash, and we increase the energy level of neighbors
            nextStep[y][x] = 11;
            for (var nY = Math.Max(y - 1, 0); nY < Math.Min(y + 2, nextStep.Count); nY++)
            {
                for (var nX = Math.Max(x-1,0); nX < Math.Min(x+2, nextStep[y].Count); nX++)
                {
                    if (nX == x && nY == y)
                        continue;
                    if (nextStep[nY][nX] <= 9)
                    {
                        nextStep[nY][nX]++;
                        Flash(nX,nY,nextStep);
                    }
                }
            }
        }

        public override object GiveAnswer2()
        {
            var maxX = _data[0].Count;
            var maxY = _data.Count;
            var curStep = _data;
            var t = 0;

            for (; ; t++)
            {
                var score = 0L;
                var nextStep = NextStep(maxX, maxY, curStep, ref score);
                if (score == maxX * maxY)
                {
                    break;
                }
                curStep = nextStep;
            }

            return t+1;
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526";
            _expectedResult1 = 1656L;
            _expectedResult2 = 195;
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
    }
}
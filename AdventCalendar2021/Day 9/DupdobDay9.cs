using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay9 : AdvancedDay
    {
        private readonly List<List<int>> _data = new();

        public override object GiveAnswer1()
        {
            var score = 0;
            for (var y = 0; y < _data.Count; y++)
            {
                for (var x = 0; x < _data[y].Count; x++)
                {
                    var height = _data[y][x];
                    if ((y>0 && height>=_data[y-1][x]) ||
                        (x>0 && height>=_data[y][x-1]) ||
                        (y<_data.Count-1 && height>=_data[y+1][x]) ||
                        (x<_data[y].Count-1 && height>=_data[y][x+1]))
                        continue;
                    score += 1 + height;
                }
            }

            return score;
        }

        public override object GiveAnswer2()
        {
            var basinsSizes = new List<int>();
            for (var y = 0; y < _data.Count; y++)
            {
                for (var x = 0; x < _data[y].Count; x++)
                {
                    var height = _data[y][x];
                    if ((y>0 && height>=_data[y-1][x]) ||
                        (x>0 && height>=_data[y][x-1]) ||
                        (y<_data.Count-1 && height>=_data[y+1][x]) ||
                        (x<_data[y].Count-1 && height>=_data[y][x+1]))
                        continue;
                    // we have a low point, we need to scan for the basin
                    basinsSizes.Add(Basin(x,y));
                }
            }
            basinsSizes.Sort();
            return basinsSizes[^1]*basinsSizes[^2]*basinsSizes[^3];
        }

        private int Basin(int x, int y)
        {
            var bitmap = new bool[_data.Count, _data[0].Count];
            return FloodScan(x, y, bitmap);
        }

        private int FloodScan(int x, int y, bool[,] bitmap)
        {
            var local = 1;
            bitmap[y, x] = true;
            if (x > 0 && _data[y][x - 1] != 9 && !bitmap[y,x-1])
            {
                local += FloodScan(x - 1, y, bitmap);
            }

            if (y > 0 && _data[y - 1][x] != 9 && !bitmap[y-1, x])
            {
                local += FloodScan(x, y - 1, bitmap);
            }

            if (x < _data[0].Count-1 && _data[y][x+1] != 9 && !bitmap[y, x+1])
            {
                local += FloodScan(x+1, y, bitmap);
            }

            if (y < _data.Count-1 && _data[y + 1][x] != 9 && !bitmap[y+1, x])
            {
                local += FloodScan(x, y + 1, bitmap);
            }
            return local;
        }

        protected override void ParseLine(int index, string line)
        {
            var heights = line.Select(c => int.Parse(c.ToString())).ToList();
            _data.Add(heights);
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"2199943210
3987894921
9856789892
8767896789
9899965678";
            _expectedResult1 = 15;
            _expectedResult2 = 1134;
        }

        protected override void SetupRunData()
        {
            _data.Clear();
        }
        
        public DupdobDay9() : base(9)
        {
        }

    }
}
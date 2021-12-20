using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay20: AdvancedDay
    {
        private string _data;
        private readonly Dictionary<(int x, int y), int> _bitmaps = new ();
        public DupdobDay20() : base(20)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            if (_data == null)
            {
                _data = line;
                return;
            }
            if (_data.Length<512)
            {
                _data += line;
                return;
            }
            else if (string.IsNullOrEmpty(line))
            {
                return;
            }

            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '#')
                {
                    _bitmaps[(i, index)] = 1;
                }
            }
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield break;
            yield return (@"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..##
#..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###
.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#.
.#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#.....
.#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#..
...####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.....
..##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###", 35);
        }
        
        public override object GiveAnswer1()
        {
            var current = _bitmaps;
            for (var i = 0; i < 2; i++)
            {
                var next = new Dictionary<(int x, int y), int>();
                Step(current, next, i);
                current = next;
            }

            return current.Values.Sum();
        }

        public override object GiveAnswer2()
        {
            var current = _bitmaps;
            for (var i = 0; i < 50; i++)
            {
                var next = new Dictionary<(int x, int y), int>();
                Step(current, next, i);
                current = next;
            }

            return current.Values.Sum();
            
        }

        private void Step(Dictionary<(int x, int y),int> current, Dictionary<(int x, int y),int> next, int turn)
        {
            var minX = current.Keys.Min(k => k.x);
            var maxX = current.Keys.Max(k => k.x);
            var minY = current.Keys.Min(k => k.y);
            var maxY = current.Keys.Max(k => k.y);
            var def = (turn % 2 == 1);
            for(var y = minY-1; y <= maxY+1; y++)
            for(var x = minX-1; x <= maxX+1; x++)
            {
                var value = 0;
                for (var y1 = y-1; y1 <= y+1; y1++)
                {
                    for (var x1 = x-1; x1 <= x+1; x1++)
                    {
                        value <<= 1;
                        if (current.ContainsKey((x1,y1))
                        || def && (x1<minX || x1>maxX || y1<minY || y1>maxY))
                        {
                            value += 1;
                        }
                    }
                }

                var nextVal = _data[value] == '.' ? 0 : 1;
                if (nextVal == 1)
                {
                    next[(x, y)] = 1;
                }
            }
        }

        protected override void CleanUp()
        {
            _bitmaps.Clear();
            _data = null;
        }
    }
}
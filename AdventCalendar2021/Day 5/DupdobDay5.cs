using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventCalendar2021
{
    public class DupdobDay5 : AdvancedDay
    {
        private List<Line> _lines = new();
        public DupdobDay5() : base(5)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var entry = new Line();
            entry.Parse(line);
            _lines.Add(entry);
        }

        public override object GiveAnswer1()
        {
            Dictionary<(int x, int y), int> map = new Dictionary<(int x, int y), int>();
            foreach (var line in _lines)
            {
                
                if (line.IsHorizontal)
                {
                    var start = Math.Min(line.X1, line.X2);
                    var end = Math.Max(line.X1, line.X2);
                    var y = line.Y1;
                    for (var x = start; x <= end; x++)
                    {
                        var point = (x, y);
                        if (!map.ContainsKey(point))
                        {
                            map[point] = 1;
                        }
                        else
                        {
                            map[point]++;
                        }
                    }
                }
                else if (line.IsVertical)
                {
                    var start = Math.Min(line.Y1, line.Y2);
                    var end = Math.Max(line.Y1, line.Y2);
                    var x = line.X1;
                    for (var y = start; y <= end; y++)
                    {
                        var point = (x, y);
                        if (!map.ContainsKey(point))
                        {
                            map[point] = 1;
                        }
                        else
                        {
                            map[point]++;
                        }
                    }
                }
            }
            return map.Values.Count(t => t>1);
        }

        public override object GiveAnswer2()
        {
            Dictionary<(int x, int y), int> map = new Dictionary<(int x, int y), int>();
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;
            foreach (var line in _lines)
            {
                var startX = line.X1;
                var endX = line.X2;
                var incX = startX == endX ? 0 : startX<endX ? 1 : -1;
                var startY = line.Y1;
                var endY = line.Y2;
                var incY = startY == endY ? 0 : startY<endY ? 1 : -1;
                var x = startX;
                var y = startY;
                do
                {
                    var point = (x, y);
                    if (!map.ContainsKey(point))
                    {
                        map[point] = 1;
                    }
                    else
                    {
                        map[point]++;
                    }

                    x += incX;
                    y += incY;
                } while (!(x == endX+incX && y== endY+incY));

            }

            return map.Values.Count(t => t>1);
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2";
            _expectedResult1 = 5;
            _expectedResult2 = 12;
        }

        protected override void SetupRunData()
        {
            _lines.Clear();
        }
        
        private class Line
        {
            public int X1;
            public int X2;
            public int Y1;
            public int Y2;
            public bool IsHorizontal => Y1 == Y2;
            public bool IsVertical => X1 == X2;

            public void Parse(string line)
            {
                var pairs = line.Split("->").SelectMany( p => p.Split(',')).Select(int.Parse).ToList();
                X1 = pairs[0];
                Y1 = pairs[1];
                X2 = pairs[2];
                Y2 = pairs[3];
            }
            
        }
    }
}
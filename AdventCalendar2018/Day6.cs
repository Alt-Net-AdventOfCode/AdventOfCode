using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2018
{
    public static class Day6
    {
        private struct Point
        {
            private int X;
            private int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int ManhattanDistanceTo(Point other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
            }
        }

        private static void MainDay6()
        {
            var minX = int.MaxValue;
            var maxX = 0;
            var minY = int.MaxValue;
            var maxY = 0;

            var split = input.Split(Environment.NewLine);
            var bombs = new List<Point>(split.Length);
            foreach (var text in split)
            {
                var coords = text.Split(',').Select(int.Parse).ToList();
                var x = coords[0];
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                var y = coords[1];
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);
                
                var point = new Point(x, y);
                bombs.Add(point);
            }
            var cellCount = new int[bombs.Count];
            var lessThanAthousand = 0;
            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    var point = new Point(x, y);
                    // identify closest point
                    var closestBomb = -1;
                    var closestDist = int.MaxValue;
                    var totalDist = 0;
                    for (var i = 0; i < cellCount.Length; i++)
                    {
                        var dist = bombs[i].ManhattanDistanceTo(point);
                        totalDist += dist;
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestBomb = i;
                        }
                        else if (dist == closestDist)
                        {
                            // multiple bombs at same distance: no winner
                            closestBomb = -1;
                        }
                    }

                    if (closestBomb >= 0)
                    {
                        cellCount[closestBomb]++;
                    }

                    if (totalDist < 10000)
                    {
                        lessThanAthousand++;
                    }
                }
            }
            
            Console.WriteLine($"Result= {cellCount.Max()}");
            Console.WriteLine($"Result2: {lessThanAthousand}");
        }

        private static readonly string input = 
@"275, 276
176, 108
270, 134
192, 224
252, 104
240, 271
144, 220
341, 303
344, 166
142, 347
207, 135
142, 353
343, 74
90, 210
82, 236
124, 295
41, 226
298, 109
276, 314
50, 303
131, 42
119, 335
275, 125
113, 289
347, 230
192, 329
158, 316
154, 356
171, 350
165, 59
257, 129
306, 55
334, 203
55, 63
268, 198
44, 103
230, 199
41, 181
357, 328
331, 85
256, 290
168, 290
353, 77
81, 328
136, 316
138, 213
352, 271
139, 222
139, 318
194, 239";
    }
}
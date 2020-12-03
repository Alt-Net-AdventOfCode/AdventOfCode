using System;

namespace AdventCalendar2018
{
    public static class Day11
    {
        private static void MainDay11()
        {
            const int serial = 5719;
            var power = new int[300, 300];
            for (var x = 0; x < 300; x++)
            {
                for (var y = 0; y < 300; y++)
                {
                    power[x, y] = Power(x, y, serial);
                }
            }

            var maxGrid = Int32.MinValue;
            var maxX = 0;
            var maxY = 0;
            var maxRect = 1;

            for (int rect = 1; rect < 300; rect++)
            {
                Console.WriteLine($"Rect {rect}");
                for (var x = 0; x < 301-rect; x++)
                {
                    for (var y = 0; y < 301-rect; y++)
                    {
                        var res = 0;
                        for (var i = x ; i < x + rect; i++)
                        {
                            for (var j = y ; j < y + rect; j++)
                            {
                                res += power[i, j];
                            }
                        }

                        if (res > maxGrid)
                        {
                            maxGrid = res;
                            maxX = x;
                            maxY = y;
                            maxRect = rect;
                        }
                    }
                }
            }
            
            Console.WriteLine($"Result :{maxX}, {maxY}, {maxRect}");
        }

        private static int Power(int x, int y, int serial)
        {
            var rackId = x + 10;
            var power = rackId * y;
            power += serial;
            power *= rackId;
            power /= 100;
            power %= 10;
            return power - 5;
        }
    }
}
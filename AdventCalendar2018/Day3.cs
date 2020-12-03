using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    public static class Day3
    {
        private static void MainDay3()
        {
            var map = new int[1000, 1000];
            var template = new Regex("\\#(\\d+) \\@ (\\d+),(\\d+): (\\d+)x(\\d+)");
            var overlap = new HashSet<int>();
            for (;;)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                var parsing = template.Match(line);
                var id = int.Parse(parsing.Groups[1].Value);
                var x = int.Parse(parsing.Groups[2].Value);
                var y = int.Parse(parsing.Groups[3].Value);
                var width = int.Parse(parsing.Groups[4].Value);
                var height = int.Parse(parsing.Groups[5].Value);
                for (var i = x; i < x + width; i++)
                {
                    for (int j = y; j < y + height; j++)
                    {
                        if (map[j, i] != 0)
                        {
                            overlap.Add(id);
                            overlap.Add(map[j, i]);
                            // overlap
                            map[j, i] = -1;
                        }
                        else
                        {
                            map[j, i] = id;
                        }
                    }
                }
            }

            var count = 0;
            foreach (var entry in map)
            {
                if (entry == -1)
                {
                    count++;
                }
            }

            Console.WriteLine(count);

            for (int i = 1; i < overlap.Count+2; i++)
            {
                if (!overlap.Contains(i))
                {
                    Console.WriteLine($"Non overlap id {i}");
                }
            }
        }
        
    }
}
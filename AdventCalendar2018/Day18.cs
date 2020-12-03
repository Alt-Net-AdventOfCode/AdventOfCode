using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventCalendar2018
{
    public static class Day18
    {
        private static void MainDay18()
        {
            var map = Input.Split(Environment.NewLine);

            var hits = new Dictionary<Map, int>();
            var target = 1000000000;
            for (var t = 0; t < target; t++)
            {
                var next = new string[map.Length];
                for (var y = 0; y < map.Length; y++)
                {
                    var nextLine = new StringBuilder(map[y].Length);
                    for (var x = 0; x < map[y].Length; x++)
                    {
                        var (empty, lumberyard, trees) = Neighbors(map, x, y);
                        var nextChar = map[y][x];
                        if (nextChar == '.')
                        {
                            if (trees >= 3)
                            {
                                nextChar = '|';
                            }
                        }
                        else if (nextChar == '|')
                        {
                            if (lumberyard >= 3)
                            {
                                nextChar = '#';
                            }
                        }
                        else if (nextChar == '#')
                        {
                            if (trees < 1 || lumberyard < 1)
                            {
                                nextChar = '.';
                            }
                        }

                        nextLine.Append(nextChar);
                    }

                    next[y] = nextLine.ToString();
                }
                var record = new Map(next);
                if (hits.ContainsKey(record))
                {
                    Dump(map);

                    var repeat = hits[record];
                    Console.WriteLine($"Generation {t} is same than {repeat}!");
                    // find final state
                    int id = (target - repeat) % (t - repeat) + repeat -1 ;
                    Console.WriteLine($"Final state is at time {id}");
                    foreach (var entry in hits)
                    {
                        if (entry.Value == id)
                        {
                            Console.WriteLine($"Found");
                            map = entry.Key.Data;
                            break;
                        }
                    }
                    break;
                }
                // compare
                hits[record] = t;
                map = next;
            }

            var lumbers = 0;
            var forest = 0;
            foreach (var line in map)
            {
                foreach (var car in line)
                {
                    if (car == '#')
                    {
                        lumbers++;
                    }
                    else if (car == '|')
                    {
                        forest++;
                    }
                }
            }
            Console.WriteLine($"Result : {forest*lumbers}");
        }

        private static void Dump(string[] map)
        {
            Console.WriteLine();
            foreach (var line in map)
            {
                Console.WriteLine(line);
            }
        }

        private static (int, int, int) Neighbors(IReadOnlyList<string> map, int x, int y)
        {
            var empty = 0;
            var lumber = 0;
            var trees = 0;
            if (x > 0)
            {
                if (y > 0)
                {
                    switch (map[y - 1][x - 1])
                    {
                        case '.':
                            empty++;
                            break;
                        case '#':
                            lumber++;
                            break;
                        default:
                            trees++;
                            break;
                    }
                }
                switch (map[y][x - 1])
                {
                    case '.':
                        empty++;
                        break;
                    case '#':
                        lumber++;
                        break;
                    default:
                        trees++;
                        break;
                }

                if (y < map.Count - 1)
                {
                    switch (map[y +1][x - 1])
                    {
                        case '.':
                            empty++;
                            break;
                        case '#':
                            lumber++;
                            break;
                        default:
                            trees++;
                            break;
                    }                    
                }
            }
            if (y > 0)
            {
                switch (map[y - 1][x])
                {
                    case '.':
                        empty++;
                        break;
                    case '#':
                        lumber++;
                        break;
                    default:
                        trees++;
                        break;
                }
            }

            if (y < map.Count - 1)
            {
                switch (map[y +1][x])
                {
                    case '.':
                        empty++;
                        break;
                    case '#':
                        lumber++;
                        break;
                    default:
                        trees++;
                        break;
                }                    
            }
            
            if (x < map[y].Length-1)
            {
                if (y > 0)
                {
                    switch (map[y - 1][x + 1])
                    {
                        case '.':
                            empty++;
                            break;
                        case '#':
                            lumber++;
                            break;
                        default:
                            trees++;
                            break;
                    }
                }
                switch (map[y][x + 1])
                {
                    case '.':
                        empty++;
                        break;
                    case '#':
                        lumber++;
                        break;
                    default:
                        trees++;
                        break;
                }

                if (y < map.Count - 1)
                {
                    switch (map[y +1][x + 1])
                    {
                        case '.':
                            empty++;
                            break;
                        case '#':
                            lumber++;
                            break;
                        default:
                            trees++;
                            break;
                    }                    
                }
            }
            return (empty, lumber, trees);
        }

        private class Map
        {
            private bool Equals(Map other)
            {
                for (var i = 0; i < map.Length; i++)
                {
                    if (map[i] != other.map[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Map) obj);
            }

            public override int GetHashCode()
            {
                return map.Aggregate(0, (current, line) => current ^ line.GetHashCode());
            }

            private readonly string[] map;

            public string[] Data => map;

            public Map(string[] map)
            {
                this.map = map;
            }
        }
        private const string Demo = 
@".#.#...|#.
.....#|##|
.|..|...#.
..|#.....#
#.#|||#|#|
...#.||...
.|....|...
||...#|.#|
|.||||..|.
...#.|..|.";
        private const string Input = 
@".#|#.##....#|....|.#.#.|||.#.|....||....|...|..#..
..|#||.|#..|...|#|..#...|#...#..#..|.....||..#.|#.
#|||#..||.....||.#................|..#.##|.#...#.|
|#..#.|...##...#..#|#|#..|#.#...|....#..#...##....
.###.........|.||#...#|.|#.||||#..|...||....#..#..
###.|..|#|...|..||..##.....|..#.|.#.............|.
..|.|.||.#....|...|....#|.........##||..#||..|.##.
#||#|...#|..|.|.||#...#|...|#.......|...#.....|...
....||.....|.|.....#...|.......|...|..|...|......|
#......#..#|#|..|....#.|.|.#...#.#.|..#.|.....#.#.
.|#...|...........#|.#....#.#...#.|..|...|....|.|.
..||.#.|...||#|....#.#..||#..#...#|..#..|..#|.....
|..|.|..#...|.....#.|..|#.||..#|.|.||#|#..|#...##|
..|..|#......||##..|........#.|...#.|.|#.#...||..#
#.|...#.||#..|.|..|..|.#....|.||....|.|....#....#.
#||.|.#..#..|...#....##|#..#...#.#...|.#...#.....#
#.|.##.|##..#.##|##........#.|...#...|..#|.#|#|...
.|#|....|.#...#..|||.#.||..#||.||.|..#.|....|..##.
|.#.||#|.##.|.||.....#...#.#..###|.#......||#|....
.|.#..|#||......|##..##.#|..#|.|#.|.|#......|#.|#.
#..|........|||..|###..|#..|||#.|.|.....#|..|...|#
..####||#......|#||..###.|...|....#..|.#|.||....||
|##.......|||..........|..||.#.|#.......##...|...|
|.......#......####|#|....#....|......#.|#.###...#
#|.#.|||...|..|.....#....|...|......|#|#|......||.
...#.|......#..||||.#|.....|.|.|||.|.|.|#|.#...#.#
#.#.##.|.#|.|...|...|...#|...#.|#..|..##.|....#..|
|...#.......#....#....#.#....#.#|.|#||.|.|.|#...#.
#..|.||..|.#..|.#.....#|##.|.|....|....||.......|.
..||.#..|#|.###....#.#|..#|.#..........#...|...#|.
|#||.|.#..|....|....#.#||#.|......#..|#.#.|||||#|.
.|#.|#.##.....#.|.#.....|....|.#..#.#..|#.#.....|.
#.||.#.......|..|......|#||.|..#....#...|...|...|.
|.....#.|.....#||.....##...#.#...||.|..#........|.
||#..|.##.#...........#..|..|.|..#....|...#..||.#.
..||.##.##.|.||......#...|.#.#.#..#.#...##.#.|.#..
.|.#......#|#||.|.#|......||.#.|.|..|....#...||...
....|.##.....|#|####.#..#..#|.....|.#.#|......|...
...#..|......#....|#.#...|...|.#.#.......#.#.##..#
.|||#.||||...|..|#||.|.#|#||..|..#..|..|..#||.....
.....|..#..|#|.||.#||.||......|||..|..#|.|##......
.#...#|..#..|||..||.|..|.#.#.......||..|...|.|....
.##.||..|..||.|.......#.|||.|.|..|.#.#..|.||.|#|||
.|..##|..#.#|#|....|.#.#.#|#.#|.##|........###...#
..#..|#|...#.........#.#.####..#.#..#..#||#|...#|#
#.|...|.......|.#.#..#.|#..#|#|..#..|.....|..|...|
.##.|..#.....|...#..|#..|.|.#..##.#.|..#.|..|.##..
....|..|.|..||....|...|.....#..|.|.....|.#|......#
...##.|#..#..|.#|.##....|.#...||#|.....#...##.#|..
.|....##.....||...#.#.....#|#...#...#|.|..#.#.#.##";
    }
}
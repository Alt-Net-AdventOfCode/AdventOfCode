using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.X86;

namespace AdventCalendar2019.Day10
{
    public class DupdobDay10
    {

        public static void GiveAnswers()
        {
            var engine = new DupdobDay10();
            engine.Parse();
            
            Console.WriteLine("Answer 1:{0}", engine.FindMaxVisibleAsteroids());
            Console.WriteLine("Answer 2:{0}", engine.FindDestroyedAsteroidAtRank(200));
        }

        private int FindMaxVisibleAsteroids()
        {
            var maxAsteroids = MaxAsteroids(out var best);
            Console.WriteLine($"{best.x},{best.y}");

            return maxAsteroids;
        }

        private int MaxAsteroids(out (int x, int y) best)
        {
            var maxAsteroids = 0;
            best = (0, 0);
            for (var i = 0; i < _asteroids.Count; i++)
            {
                var cache = new List<(int vx, int vy)>();
                var seenAsteroids = 0;
                var current = _asteroids[i];
                for (var j = 0; j < _asteroids.Count; j++)
                {
                    if (i == j)
                    {
                        seenAsteroids = cache.Count;
                        cache = new List<(int vx, int vy)>();
                        continue;
                    }

                    (int vx, int vy) newVec = (_asteroids[j].x - current.x, _asteroids[j].y - current.y);

                    // see if we already have an asteroid in this line of sight
                    if (cache.Any(line => line.vx * newVec.vy == line.vy * newVec.vx))
                    {
                        continue;
                    }

                    cache.Add(newVec);
                }

                seenAsteroids += cache.Count;
                if (maxAsteroids < seenAsteroids)
                {
                    maxAsteroids = seenAsteroids;
                    best = current;
                }
            }

            return maxAsteroids;
        }

        private int FindDestroyedAsteroidAtRank(int rank)
        {
            // build adequate structure
            (int x, int y) laserBase;
            MaxAsteroids(out laserBase);
            var cache = new List<List<(int vx, int vy)>>();
            foreach (var asteroid in _asteroids)
            {
                if (asteroid == laserBase)
                {
                    continue;
                }
                (int vx, int vy) newVec = (asteroid.x - laserBase.x, asteroid.y - laserBase.y);
                var found = false;
                foreach (var list in cache)
                {
                    var refVec = list[0];
                    if (newVec.vy * refVec.vy > 0 || (newVec.vy == 0 && refVec.vy == 0 && newVec.vx * refVec.vx > 0))
                    {
                        if (refVec.vx * newVec.vy == refVec.vy * newVec.vx)
                        {
                            found = true;
                            list.Add(newVec);
                            break;
                        }
                    }
                }

                if (!found)
                {
                    cache.Add(new List<(int vx, int vy)>{newVec});
                }
            }
            // sort each cache entry according to distance
            foreach (var entry in cache)
            {
                entry.Sort((tuple, other) => (tuple.vx*tuple.vx+tuple.vy*tuple.vy).CompareTo(other.vx*other.vx+other.vy*other.vy));
            }

            // sort each axe
            cache.Sort((list, otherList) =>
            {
                var first = list[0];
                var other = otherList[0];
                if (first.vx * other.vx < 0)
                {
                    return first.vx > other.vx ? -1 : 1;
                }

                if (first.vx == 0)
                {
                    if (first.vy < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return other.vx < 0 ? 1 : -1;
                    }
                }
                else
                {
                    return first.vx * other.vy > first.vy * other.vx ? -1 : 1;
                }
            });
            // kill asteroids
            var listIndex = 0;
            var kill = 1;
            while (rank>kill)
            {
                Console.WriteLine($"Kill {kill} at {cache[listIndex][0].vx+laserBase.x}:{cache[listIndex][0].vy+laserBase.y}");
                cache[listIndex].RemoveAt(0);
                if (cache[listIndex].Count == 0)
                {
                    cache.RemoveAt(listIndex);
                }
                else
                {
                    listIndex++;;
                }

                listIndex %= cache.Count;
                kill++;
            }

            var last = cache[listIndex][0];
            Console.WriteLine($"Kill {kill} at {cache[listIndex][0].vx+laserBase.x}:{cache[listIndex][0].vy+laserBase.y}");
            return (last.vx+laserBase.x)*100+(last.vy+laserBase.y);
        }
        private void Parse(string input = Input)
        {
            var lines = input.Split('\n').ToArray();
            for (var y = 0; y < lines.Length; y++)
            {
                var curLine = lines[y];
                for (var x = 0; x < curLine.Length; x++)
                {
                    if (curLine[x] == '#')
                    {
                        _asteroids.Add((x, y));
                    }
                }
            }
        }
        
        
        private readonly List<(int x, int y)> _asteroids = new List<(int x, int y)>();
        
            private const string Input =
@"#....#.....#...#.#.....#.#..#....#
#..#..##...#......#.....#..###.#.#
#......#.#.#.....##....#.#.....#..
..#.#...#.......#.##..#...........
.##..#...##......##.#.#...........
.....#.#..##...#..##.....#...#.##.
....#.##.##.#....###.#........####
..#....#..####........##.........#
..#...#......#.#..#..#.#.##......#
.............#.#....##.......#...#
.#.#..##.#.#.#.#.......#.....#....
.....##.###..#.....#.#..###.....##
.....#...#.#.#......#.#....##.....
##.#.....#...#....#...#..#....#.#.
..#.............###.#.##....#.#...
..##.#.........#.##.####.........#
##.#...###....#..#...###..##..#..#
.........#.#.....#........#.......
#.......#..#.#.#..##.....#.#.....#
..#....#....#.#.##......#..#.###..
......##.##.##...#...##.#...###...
.#.....#...#........#....#.###....
.#.#.#..#............#..........#.
..##.....#....#....##..#.#.......#
..##.....#.#......................
.#..#...#....#.#.....#.........#..
........#.............#.#.........
#...#.#......#.##....#...#.#.#...#
.#.....#.#.....#.....#.#.##......#
..##....#.....#.....#....#.##..#..
#..###.#.#....#......#...#........
..#......#..#....##...#.#.#...#..#
.#.##.#.#.....#..#..#........##...
....#...##.##.##......#..#..##....";
    }
}
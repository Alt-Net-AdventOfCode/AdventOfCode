using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdventCalendar2018
{
    public static class Day15
    {
        private static void MainDay15()
        {
            var elvesPower = 13;
            var oldElvesPower = 3;
            var oldPowerFailed = true;
            int round ;
            List<Unit> units;
            do
            {
                units = new List<Unit>();
                round = TestGame(units, elvesPower);
 /*               if (round == 0 && oldPowerFailed)
                {
                    // elves have a loss
                    // => increase power
                    oldElvesPower = elvesPower;
                    elvesPower += 10;
                }
                else if (round > 0 && oldPowerFailed && elvesPower - oldElvesPower == 1)
                {
                    // found it
                    break;
                }
                else if (round > 0 && !oldPowerFailed)
                {
                    var t = elvesPower;
                    elvesPower = Math.Min(elvesPower, oldElvesPower)-1;
                    oldElvesPower = t;
                    oldPowerFailed = true;
                }
                else
                {
                    var t = elvesPower;
                    elvesPower = (oldElvesPower + elvesPower) / 2;
                    oldElvesPower = t;
                    oldPowerFailed = round==0;
                }
                */
            } while (false);

            // compute score
            var nbUnit = 0;
            var totalHealth = 0;
            foreach (var unit in units)
            {
                if (!unit.Dead)
                {
                    nbUnit++;
                    totalHealth += unit.Health;
                }
            }
            Console.WriteLine($"Result: {round * totalHealth}");
        }

        private static int TestGame(List<Unit> units, int elvesPower)
        {
            Console.WriteLine($"Trying power {elvesPower}.");
            var map = Input.Split(Environment.NewLine);
            // identify units
            for (var yIndex = 0; yIndex < map.Length; yIndex++)
            {
                for (var xIndex = 0; xIndex < map[yIndex].Length; xIndex++)
                {
                    if (map[yIndex][xIndex] == 'E' || map[yIndex][xIndex] == 'G')
                    {
                        var unit = new Unit(xIndex, yIndex, map[yIndex][xIndex]);
                        if (unit.Type == 'E')
                        {
                            unit.Power = elvesPower;
                        }
                        units.Add(unit);
                    }
                }
            }

            var oneMove = true;
            var round = -2;
            while (oneMove)
            {
                oneMove = false;
                var moveToClosest = false;
                units.Sort();
                foreach (var unit in units)
                {
                    moveToClosest = unit.MoveToClosest(map) || moveToClosest;
                    oneMove = unit.Fight(units, map) || oneMove || moveToClosest;
                }

                foreach (var unit in units)
                {
                    if (unit.Type == 'E' && unit.Dead)
                    {
                        DumpMap(map);
                        return 0;
                    }
                }
                round++;
            }

            return round;
        }

        private static void DumpMap(string[] map)
        {
            Console.WriteLine();
            foreach (var line in map)
            {
                Console.WriteLine(line);
            }
        }

        private class Unit : IComparable<Unit>
        {
            private int Id;
            private static int autoId;
            private int power = 3;

            public int Power
            {
                get => power;
                set => power = value;
            }

            private int health = 200;

            public int Health => health;

            private int x;
            private int y;
            private char type;

            public char Type => type;

            public bool Dead => health == 0;
            public Unit(int x, int y, char type)
            {
                this.x = x;
                this.y = y;
                this.type = type;
                Id = autoId++;
            }

            public bool MoveToClosest(string[] map)
            {
                var result = false;
                if (Dead)
                    return result;
                var start = new Cell(x ,y , 0);
                if (!InRange(start, map))
                {
                    // move
                    var cells = new List<Cell>();
                    var altMap = ParseMap(map, cells);
                    altMap[y, x] = start;
                    cells.Add(start);
                    var inRangeCells = new List<Cell>();
                    var closest = FindNearestInRange(map, cells, altMap, inRangeCells);
                    if (closest != null)
                    {
                        // find next move
                        while (closest.predecessor != start)
                        {
                            closest = closest.predecessor;
                        }

                        RemoveFromMap(map);
                        x = closest.x;
                        y = closest.y;
                        UpdateMap(closest.x, closest.y, type, map);
                        result = true;                        
                    }
                    else
                    {
                        return false;
                    }
                }
                // find weaker enemy
                return result;
            }

            private void RemoveFromMap(IList<string> map)
            {
                UpdateMap(x, y, '.', map);
            }

            private Cell FindNearestInRange(IReadOnlyList<string> map, ICollection<Cell> cells, Cell[,] altMap, ICollection<Cell> inRangeCells)
            {
                while (cells.Count > 0)
                {
                    var min = cells.Aggregate((current, next) => next.dist < current.dist ? next : current);
                    if (min.dist == int.MaxValue)
                    {
                        // cell is unreachable!
                        break;
                    }

                    // there is a border around the playing field, no need to have boundary check, 
                    Check(altMap, min, min.x, min.y - 1);
                    Check(altMap, min, min.x - 1, min.y);
                    Check(altMap, min, min.x + 1, min.y);
                    Check(altMap, min, min.x, min.y + 1);

                    cells.Remove(min);
                    altMap[min.y, min.x] = null;
                    // keep only cells in range
                    if (InRange(min, map))
                    {
                        inRangeCells.Add(min);
                    }
                }

                if (inRangeCells.Count == 0)
                {
                    // no one in range
                    return null;
                }

                Cell closest = null;
                if (inRangeCells.Count > 1)
                {
                    var closests = new List<Cell>();
                    inRangeCells.Aggregate((current, next) =>
                    {
                        if (closests.Count == 0)
                        {
                            closests.Add(current);
                        }

                        if (next.dist > current.dist)
                        {
                            return current;
                        }

                        if (next.dist < current.dist)
                        {
                            closests.Clear();
                        }

                        closests.Add(next);
                        return next;
                    });
                    closest = closests.Min();
                }
                else
                {
                    closest = inRangeCells.First();
                }

                return closest;
            }

            private bool InRange(Cell min, IReadOnlyList<string> map)
            {
                var enemy = type == 'G' ? 'E' : 'G';
                return map[min.y - 1][min.x] == enemy ||
                       map[min.y][min.x - 1] == enemy ||
                       map[min.y][min.x + 1] == enemy ||
                       map[min.y + 1][min.x] == enemy;
            }

            private static Cell[,] ParseMap(IReadOnlyList<string> map, ICollection<Cell> cells)
            {
                var altMap = new Cell[map.Count, map[0].Length];
                for (var i = 0; i < map.Count; i++)
                {
                    var line = map[i];
                    for (var j = 0; j < line.Length; j++)
                    {
                        if (line[j] == '.')
                        {
                            var nextCell = new Cell(j, i, int.MaxValue);
                            altMap[i, j] = nextCell;
                            cells.Add(nextCell);
                        }
                    }
                }

                return altMap;
            }

            private static void DumpPath(string[] map, Cell start, Cell destination)
            {
                if (destination.dist == int.MaxValue)
                {
                    Console.WriteLine("No accessible range");
                    return;
                }
                var mapCopy = (string[])map.Clone();
                Console.WriteLine($"Path from {start.x}:{start.y} to {destination.x}:{destination.y}.");
                // trace path for debug purpose
                const char c = '*';
                while (destination!=null && destination!=start)
                {
                    UpdateMap(destination.x, destination.y, c, mapCopy);
                    destination = destination.predecessor;
                }
                DumpMap(mapCopy);
            }

            private static void UpdateMap(int x,int y, char c, IList<string> mapCopy)
            {
                var line = mapCopy[y];
                mapCopy[y] = line.Substring(0, x) + c + line.Substring(x + 1);
            }

            private static void Check(Cell[,] map, Cell current, int x, int y)
            {
                var neighbour = map[y, x];
                if (neighbour == null || neighbour.dist < current.dist + 1)
                {
                    // no neighbour
                    return;
                }

                if (neighbour.dist == current.dist + 1 && neighbour.predecessor != null &&
                    neighbour.predecessor.CompareTo(current) < 0)
                {
                    return;
                }
                neighbour.dist = current.dist + 1;
                neighbour.predecessor = current;
            }
            
            public int CompareTo(Unit other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                var yComparison = y.CompareTo(other.y);
                return yComparison != 0 ? yComparison : x.CompareTo(other.x);
            }

            public bool Fight(IEnumerable<Unit> units, IList<string> map)
            {
                if (Dead)
                    return false;
                var enemyType = type == 'E' ? 'G' : 'E';
                Unit enemyUnit = null;
                foreach (var unit in units)
                {
                    if (unit.type == enemyType && !unit.Dead)
                    {
                        if (Math.Abs(unit.x - x) + Math.Abs(unit.y - y) == 1)
                        {
                            if ( enemyUnit == null || (unit.health < enemyUnit.health))
                            {
                                enemyUnit = unit;
                            }
                        }
                    }
                } 

                if (enemyUnit != null)
                {
                    enemyUnit.TakeHit(power, map);
                    return true;
                }

                return false;
            }

            private void TakeHit(int hit, IList<string> map)
            {
                health -= hit;
                if (health < 0)
                {
                    health = 0;
                    RemoveFromMap(map);
                }
            }
        }

        private class Cell : IComparable<Cell>
        {
            public int CompareTo(Cell other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                var yComparison = y.CompareTo(other.y);
                if (yComparison != 0) return yComparison;
                return x.CompareTo(other.x);
            }

            public readonly int x;
            public readonly int y;
            public int dist;
            public Cell predecessor;

            public Cell(int x, int y, int dist)
            {
                this.x = x;
                this.y = y;
                this.dist = dist;
            }
        }

        private const string Demo =
@"#######
#G..#E#
#E#E.E#
#G.##.#
#...#E#
#...E.#
#######";

        private const string Demo2 =
@"#######
#E..EG#
#.#G.E#
#E.##E#
#G..#.#
#..E#.#
#######";

        private const string Demo3 =
@"#######
#E.G#.#
#.#G..#
#G.#.G#
#G..#.#
#...E.#
#######";

        private const string Input = 
@"################################
##########G###.#################
##########..G#G.G###############
##########G......#########...###
##########...##.##########...###
##########...##.#########G..####
###########G....######....######
#############...............####
#############...G..G.......#####
#############.............######
############.............E######
######....G..G.........E....####
####..G..G....#####.E.G.....####
#####...G...G#######........####
#####.......#########........###
####G.......#########.......####
####...#....#########.#.....####
####.#..#...#########E#..E#..###
####........#########..E.#######
###......#..G#######....########
###.......G...#####.....########
##........#............#########
#...##.....G......E....#########
#.#.###..#.....E.......###.#####
#######................###.#####
##########.......E.....###.#####
###########...##........#...####
###########..#####.............#
############..#####.....#......#
##########...######...........##
#########....######..E#....#####
################################";
    }
}
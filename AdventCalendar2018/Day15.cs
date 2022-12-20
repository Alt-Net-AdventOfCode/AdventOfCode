using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2018
{
    public static class Day15
    {
        private static void MainDay15()
        {
            const int elvesPower = 13;
            var units = new List<Unit>();
            var round = TestGame(units, elvesPower);


            // compute score
            var totalHealth = units.Where(unit => !unit.Dead).Sum(unit => unit.Health);
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
            public int Power { get; set; } = 3;

            public int Health { get; private set; } = 200;

            private int _x;
            private int _y;

            public char Type { get; }

            public bool Dead => Health == 0;
            public Unit(int x, int y, char type)
            {
                _x = x;
                _y = y;
                Type = type;
            }

            public bool MoveToClosest(string[] map)
            {
                if (Dead)
                    return false;
                var start = new Cell(_x ,_y , 0);
                if (InRange(start, map))
                {
                    return false;
                }
                // move
                var cells = new List<Cell>();
                var altMap = ParseMap(map, cells);
                altMap[_y, _x] = start;
                cells.Add(start);
                var inRangeCells = new List<Cell>();
                var closest = FindNearestInRange(map, cells, altMap, inRangeCells);
                if (closest != null)
                {
                    // find next move
                    while (closest.Predecessor != start)
                    {
                        closest = closest.Predecessor;
                    }

                    RemoveFromMap(map);
                    _x = closest.X;
                    _y = closest.Y;
                    UpdateMap(closest.X, closest.Y, Type, map);
                }
                else
                {
                    return false;
                }
                // find weaker enemy
                return true;
            }

            private void RemoveFromMap(IList<string> map)
            {
                UpdateMap(_x, _y, '.', map);
            }

            private Cell FindNearestInRange(IReadOnlyList<string> map, ICollection<Cell> cells, Cell[,] altMap, ICollection<Cell> inRangeCells)
            {
                while (cells.Count > 0)
                {
                    var min = cells.Aggregate((current, next) => next.Dist < current.Dist ? next : current);
                    if (min.Dist == int.MaxValue)
                    {
                        // cell is unreachable!
                        break;
                    }

                    // there is a border around the playing field, no need to have boundary check, 
                    Check(altMap, min, min.X, min.Y - 1);
                    Check(altMap, min, min.X - 1, min.Y);
                    Check(altMap, min, min.X + 1, min.Y);
                    Check(altMap, min, min.X, min.Y + 1);

                    cells.Remove(min);
                    altMap[min.Y, min.X] = null;
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

                Cell closest;
                if (inRangeCells.Count > 1)
                {
                    var closestCells = new List<Cell>();
                    inRangeCells.Aggregate((current, next) =>
                    {
                        if (closestCells.Count == 0)
                        {
                            closestCells.Add(current);
                        }

                        if (next.Dist > current.Dist)
                        {
                            return current;
                        }

                        if (next.Dist < current.Dist)
                        {
                            closestCells.Clear();
                        }

                        closestCells.Add(next);
                        return next;
                    });
                    closest = closestCells.Min();
                }
                else
                {
                    closest = inRangeCells.First();
                }

                return closest;
            }

            private bool InRange(Cell min, IReadOnlyList<string> map)
            {
                var enemy = Type == 'G' ? 'E' : 'G';
                return map[min.Y - 1][min.X] == enemy ||
                       map[min.Y][min.X - 1] == enemy ||
                       map[min.Y][min.X + 1] == enemy ||
                       map[min.Y + 1][min.X] == enemy;
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

            private static void UpdateMap(int x,int y, char c, IList<string> mapCopy)
            {
                var line = mapCopy[y];
                mapCopy[y] = line.Substring(0, x) + c + line.Substring(x + 1);
            }

            private static void Check(Cell[,] map, Cell current, int x, int y)
            {
                var neighbour = map[y, x];
                if (neighbour == null || neighbour.Dist < current.Dist + 1)
                {
                    // no neighbour
                    return;
                }

                if (neighbour.Dist == current.Dist + 1 && neighbour.Predecessor != null &&
                    neighbour.Predecessor.CompareTo(current) < 0)
                {
                    return;
                }
                neighbour.Dist = current.Dist + 1;
                neighbour.Predecessor = current;
            }
            
            public int CompareTo(Unit other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                var yComparison = _y.CompareTo(other._y);
                return yComparison != 0 ? yComparison : _x.CompareTo(other._x);
            }

            public bool Fight(IEnumerable<Unit> units, IList<string> map)
            {
                if (Dead)
                    return false;
                var enemyType = Type == 'E' ? 'G' : 'E';
                Unit enemyUnit = null;
                foreach (var unit in units)
                {
                    if (unit.Type == enemyType && !unit.Dead)
                    {
                        if (Math.Abs(unit._x - _x) + Math.Abs(unit._y - _y) == 1)
                        {
                            if ( enemyUnit == null || (unit.Health < enemyUnit.Health))
                            {
                                enemyUnit = unit;
                            }
                        }
                    }
                } 

                if (enemyUnit != null)
                {
                    enemyUnit.TakeHit(Power, map);
                    return true;
                }

                return false;
            }

            private void TakeHit(int hit, IList<string> map)
            {
                Health -= hit;
                if (Health < 0)
                {
                    Health = 0;
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
                var yComparison = Y.CompareTo(other.Y);
                return yComparison != 0 ? yComparison : X.CompareTo(other.X);
            }

            public readonly int X;
            public readonly int Y;
            public int Dist;
            public Cell Predecessor;

            public Cell(int x, int y, int dist)
            {
                this.X = x;
                this.Y = y;
                this.Dist = dist;
            }
        }

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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AdventCalendar2019.Day24
{
    public class DupdobDay24
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay24();

            runner.ParseInput();
            
            Console.WriteLine("Answer 1: {0}", runner.FindRepeatingPattern(true));
            runner.ParseInput();
            Console.WriteLine("Answer 2: {0}", runner.HowManyBugs(200));
        }

        private int HowManyBugs(int time)
        {
            var maps = new Dictionary<int, char[,]>();
            var bugs = 0;
            var minLevel = -1;
            var maxLevel = 1;
            for (var i = 0; i < time; i++)
            {
                bugs = 0;
                var nextMaps = new Dictionary<int, char[,]>();
                for (var level = minLevel; level <= maxLevel; level++)
                {
                    var bugsAtLevel = 0;
                    for (var x = 0; x < 5; x++)
                    {
                        for (var y = 0; y < 5; y++)
                        {
                            if (x == 2 && y == 2)
                            {
                                continue;
                            }
                            var count = GetNeighbours((level, x, y)).Count(coords => GetCellMultiLevel(maps, coords) == '#');

                            bool isAlive;
                            if (GetCellMultiLevel(maps, (level, x, y)) == '.')
                            {
                                isAlive = (count >= 1 && count <= 2);
                            }
                            else
                            {
                                isAlive = count == 1;
                            }

                            if (isAlive)
                                bugsAtLevel++;
                            
                            SetCellMultipleLevel(nextMaps, (level, x, y), isAlive ? '#' : '.');
                        }
                    }

                    if (bugsAtLevel == 0)
                    {
                        nextMaps.Remove(level);
                    }
                    else if (level == minLevel)
                    {
                        minLevel--;
                    }
                    else if (level == maxLevel)
                    {
                        maxLevel++;
                    }
                    bugs += bugsAtLevel;
                }
                maps = nextMaps;
            }

            return bugs;
        }

        private int FindRepeatingPattern(bool debug = false)
        {
            var patterns = new HashSet<int>();
            for (;;)
            {
                var newMap = new Dictionary<(int x, int y), char>();
                var score = 0;
                var bit = 1;
                if (debug)
                {
                    for (var y = 0; y < _height; y++)
                    {
                        for (var x = 0; x < _width; x++)
                        {
                            Console.Write(GetCell((x,y)));
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
                for (var y = 0; y < _height; y++)
                {
                    for (var x= 0; x <_width; x++)
                    {
                        var populatedCellCount = 0;
                        if (GetCell((x, y-1)) == '#')
                        {
                            populatedCellCount++;
                        }

                        if (GetCell((x+1, y)) == '#')
                        {
                            populatedCellCount++;
                        }

                        if (GetCell((x, y+1)) == '#')
                        {
                            populatedCellCount++;
                        }

                        if (GetCell((x - 1, y)) == '#')
                        {
                            populatedCellCount++;
                        }

                        var cell = '.';
                        if (_map[(x, y)] == '#')
                        {
                            cell = populatedCellCount == 1 ? '#' : '.';
                        }
                        else
                        {
                            cell = (populatedCellCount == 1 || populatedCellCount == 2) ? '#' : '.';
                        }

                        newMap[(x, y)] = cell;
                        if (_map[(x,y)] == '#')
                        {
                            score += bit;
                        }

                        bit <<= 1;    
                    }
                }

                if (patterns.Contains(score))
                {
                    return score;
                }

                patterns.Add(score);
                _map = newMap;
            }
        }

        private IEnumerable<(int level, int x, int y)> GetNeighbours((int level, int x, int y) cell)
        {
            // get left neighbours
            if (cell.x == 0)
            {
                // we are on the left borders => left neighbour is next level
                yield return (cell.level + 1, 1, 2);
            }
            else if (cell.y == 2 && cell.x == 3)
            {
                // we are next to the middle ==> 5 neighbours
                yield return (cell.level - 1, 4, 0);
                yield return (cell.level - 1, 4, 1);
                yield return (cell.level - 1, 4, 2);
                yield return (cell.level - 1, 4, 3);
                yield return (cell.level - 1, 4, 4);
            }
            else
            {
                yield return (cell.level, cell.x - 1, cell.y);
            }
            // get top neighbours
            if (cell.y == 0)
            {
                // we are on the top, top neighbour is level up
                yield return (cell.level + 1, 2, 1);
            }
            else if (cell.y == 3 && cell.x == 2)
            {
                // we are next to the middle ==> 5 neighbours
                yield return (cell.level - 1, 0, 4);
                yield return (cell.level - 1, 1, 4);
                yield return (cell.level - 1, 2, 4);
                yield return (cell.level - 1, 3, 4);
                yield return (cell.level - 1, 4, 4);
            }
            else
            {
                yield return (cell.level, cell.x, cell.y - 1);
            }
            // right
            if (cell.x == 4)
            {
                // we are on the right borders => left neighbour is next level
                yield return (cell.level + 1, 3, 2);
            }
            else if (cell.y == 2 && cell.x == 1)
            {
                // we are next to the middle ==> 5 neighbours
                yield return (cell.level - 1, 0, 0);
                yield return (cell.level - 1, 0, 1);
                yield return (cell.level - 1, 0, 2);
                yield return (cell.level - 1, 0, 3);
                yield return (cell.level - 1, 0, 4);
            }
            else
            {
                yield return (cell.level, cell.x + 1, cell.y);
            }           
            // get bottom neighbours
            if (cell.y == 4)
            {
                // we are on the bottom => neighbour is level up
                yield return (cell.level + 1, 2, 3);
            }
            else if (cell.y == 1 && cell.x == 2)
            {
                // we are next to the middle ==> 5 neighbours
                yield return (cell.level - 1, 0, 0);
                yield return (cell.level - 1, 1, 0);
                yield return (cell.level - 1, 2, 0);
                yield return (cell.level - 1, 3, 0);
                yield return (cell.level - 1, 4, 0);
            }
            else
            {
                yield return (cell.level, cell.x, cell.y + 1);
            }
        }
        
        private void ParseInput(string input = Input)
        {
            var lines = input.Split('\n');
            _height = lines.Length;
            _width = lines[0].Length;
            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    _map[(x, y)] = line[x];
                }
            }
        }

        private char GetCell((int x, int y) coordinates)
        {
            return _map.TryGetValue(coordinates, out var cell) ? cell : '.';
        }

        private char GetCellMultiLevel(IDictionary<int, char[,]> maps, (int level, int x, int y) coordinates)
        {
            if (!maps.ContainsKey(coordinates.level))
            {
                if (coordinates.level == 0)
                {
                    return GetCell((coordinates.x, coordinates.y));
                }
                return '.';
            }
            return maps[coordinates.level][coordinates.x, coordinates.y];
        }

        private static void CheckLevelExists(IDictionary<int, char[,]> maps, (int level, int x, int y) coordinates)
        {
            var (level, _, _) = coordinates;
            if (maps.ContainsKey(level)) return;
            // need to initialize the level
            var newLevel = new char[5, 5];
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    newLevel[i, j] = '.';
                }
            }
            maps[level]=newLevel;
        }

        private static void SetCellMultipleLevel(IDictionary<int, char[,]> maps, (int level, int x, int y) coordinates, char cell)
        {
            CheckLevelExists(maps, coordinates);
            maps[coordinates.level][coordinates.x, coordinates.y] = cell;
        }

        private Dictionary<(int x, int y), char> _map = new Dictionary<(int x, int y), char>();
        private int _height;
        private int _width;
        private const string Input = 
@"##.#.
.##..
##.#.
.####
###..";
    }
}
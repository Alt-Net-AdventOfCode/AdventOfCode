using System;
using System.Collections.Generic;

namespace AdventCalendar2019.Day24
{
    public class DupdobDay24
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay24();

            runner.ParseInput();
            
            Console.WriteLine("Answer 1: {0}", runner.FindRepeatingPattern(true));
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

        private char GetCell((int x, int y) coord)
        {
            return _map.TryGetValue(coord, out var cell) ? cell : '.';
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
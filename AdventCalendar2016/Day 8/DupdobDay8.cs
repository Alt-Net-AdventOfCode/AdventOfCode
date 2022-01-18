using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay8 : Algorithm
    {
        private readonly Regex _rectangle = new("rect (\\d*)x(\\d*)", RegexOptions.Compiled);
        private readonly Regex _rotateRow = new("rotate row y=(\\d*) by (\\d*)", RegexOptions.Compiled);
        private readonly Regex _rotateColumn = new("rotate column x=(\\d*) by (\\d*)", RegexOptions.Compiled);

        private readonly List<Func<bool[,], bool[,]>> _actions = new();
        private bool[,] _display;

        private static bool[,] Lit(bool[,] screen, int width, int height)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    screen[x, y] = true;
                }
            }

            return screen;
        }

        protected override void ParseLine(string line, int index, int lineCount)
        {
            var match = _rectangle.Match(line);
            if (match.Success)
            {
                _actions.Add( (s) => Lit(s,  int.Parse(match.Groups[1].Value),  int.Parse(match.Groups[2].Value)) );
            }
            else
            {
                match = _rotateColumn.Match(line);
                if (match.Success)
                {
                    _actions.Add( (s) => RotateColumn(s,  int.Parse(match.Groups[1].Value),  int.Parse(match.Groups[2].Value)) );
                }
                else
                {
                    match = _rotateRow.Match(line);
                    if (match.Success)
                    {
                        _actions.Add( (s) => RotateRow(s,  int.Parse(match.Groups[1].Value),  int.Parse(match.Groups[2].Value)) );
                    }
                    else
                    {
                        throw new Exception($"Failed to parse {line}.");
                    }
                }
            }
        }
        
        private static void PrintMap(bool[,] map)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                for (var x = 0; x < map.GetLength(0); x++)
                {
                    Console.Write(map[x,y] ? '#' : '.');
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static bool[,] CopyMap(bool[,] map)
        {
            var result = new bool[map.GetLength(0), map.GetLength(1)];
            for (var x = 0; x < map.GetLength(0); x++)
            {
                for (var y = 0; y < map.GetLength(1); y++)
                {
                    result[x, y] = map[x, y];
                }
            }

            return result;
        }
        
        private static bool[,] RotateColumn(bool[,] bools, int column, int offset)
        {
            var result = CopyMap(bools);    
            var length = bools.GetLength(1);
            for (var y = 0; y < length; y++)
            {
                result[column, (y + offset) % length] = bools[column, y];
            }

            return result;
        }

        private static bool[,] RotateRow(bool[,] bools, int row, int offset)
        {
            var result = CopyMap(bools);
            var length = bools.GetLength(0);
            for (var x = 0; x < length; x++)
            {
                result[(x + offset) % length, row] = bools[x, row];
            }

            return result;
        }

        public override object GetAnswer1()
        {
            var screen = new bool[50, 6];
            screen = _actions.Aggregate(screen, (current, t) => t(current));

            _display = screen;
            var count = 0;
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 6; y++)
                {
                    if (screen[x, y])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public override object GetAnswer2()
        {
            // process need manual review
            PrintMap(_display);
            Console.WriteLine("Type the LETTERS you see (5x6 pixels):");
            // answer is "EOARGPHYAO";
            return Console.ReadLine();
        }

        public override void SetupRun(DayEngine dayEngine)
        {
            dayEngine.Day = 8;
            dayEngine.RegisterTestData(1, @"rect 3x2
rotate column x=1 by 1
rotate row y=0 by 4
rotate column x=1 by 1", 6);
        }
    }
}
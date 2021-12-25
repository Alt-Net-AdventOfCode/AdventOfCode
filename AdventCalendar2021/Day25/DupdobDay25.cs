using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventCalendar2021
{
    public class DupdobDay25: AdvancedDay
    {
        private List<string> _data = new();
        public DupdobDay25() : base(25)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            _data.Add(line);
        }

        protected override void CleanUp()
        {
            _data.Clear();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"v...>>.vv>
.vv>>.vv..
>>.>v>...v
>>v>>.>.v.
v>v.vv.v..
>.>>..v...
.vv..>.>v.
v.v..>>v.v
....v..v.>", 58);
        }

        public override object GiveAnswer1()
        {
            bool moved;
            var map = _data;
            var count = 0;
            do
            {
                map = MoveSouthward(MoveEastward(map, out moved), ref moved);
                count++;
            } while (moved);

            Console.WriteLine();
            return count;
        }

        private static List<string> MoveEastward(List<string> map, out bool moved)
        {
            var result = new List<string>(map.Count);
            moved = false;
            foreach (var line in map)
            {
                var length = line.Length;
                var newLine = new StringBuilder();
                for (var i = 0; i < length; i++)
                {
                    var currentChar = line[i];
                    var prevChar = line[(i + length - 1) % length];
                    var nextChar = line[(i + 1) % length];
                    switch (currentChar)
                    {
                        case '>' when nextChar == '.':
                            // it can move
                            newLine.Append('.');
                            moved = true;
                            break;
                        case '.' when prevChar == '>':
                            // it moves
                            newLine.Append('>');
                            break;
                        default:
                            newLine.Append(currentChar);
                            break;
                    }
                }
                result.Add(newLine.ToString());
            }
            return result;
        }
        
        private static List<string> MoveSouthward(List<string> map, ref bool moved)
        {
            var result = new List<string>(map.Count);
            var height = map.Count;
            for(var j = 0; j<height; j++)
            {
                var length = map[j].Length;
                var newLine = new StringBuilder();
                for (var i = 0; i < length; i++)
                {
                    var currentChar = map[j][i];
                    var prevChar = map[(j + height - 1) % height][i];
                    var nextChar = map[(j + 1) % height][i];
                    switch (currentChar)
                    {
                        case 'v' when nextChar == '.':
                            // it can move
                            newLine.Append('.');
                            moved = true;
                            break;
                        case '.' when prevChar == 'v':
                            // it moves
                            newLine.Append('v');
                            break;
                        default:
                            newLine.Append(currentChar);
                            break;
                    }
                }
                result.Add(newLine.ToString());
            }
            return result;
        }
    }
}
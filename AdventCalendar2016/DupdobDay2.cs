using System;
using System.Collections.Generic;
using System.Text;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay2 : DupdobDayWithTest
    {
        private const string Keypad = @"  1
 234
56789
 ABC
  D";

        protected override void ParseLine(int index, string line)
        {
            if (index == 0)
            {
                _lines.Clear();
            }
            _lines.Add(line);
        }

        public override object GiveAnswer1()
        {
            var x = 1;
            var y = 1;
            var code = 0;
            foreach (var line in _lines)
            {
                foreach (var step in line)
                {
                    switch (step)
                    {
                        case 'U':
                            y = Math.Max(0, y - 1);
                            break;
                        case 'D':
                            y = Math.Min(2, y + 1);
                            break;
                        case 'L':
                            x = Math.Max(0, x - 1);
                            break;
                        case 'R':
                            x = Math.Min(2, x + 1);
                            break;
                    }
                }

                code = code * 10 + x + y * 3 + 1;
            }

            return code;
        }

        public override object GiveAnswer2()
        {
            var x = 0;
            var y = 2;
            var code = new StringBuilder();
            var lastDigit = '5';
            foreach (var line in _lines)
            {
                foreach (var step in line)
                {
                    var digit = ' ';
                    var nextX = x;
                    var nextY = y;
                    switch (step)
                    {
                        case 'U':
                            nextY =y-1;
                            break;
                        case 'D':
                            nextY = y+1;
                            break;
                        case 'L':
                            nextX = x-1;
                            break;
                        case 'R':
                            nextX = x+1;
                            break;
                    }

                    digit = EntryAt(nextX, nextY);
                    if (digit != ' ')
                    {
                        x = nextX;
                        y = nextY;
                        lastDigit = digit;
                    }
                }

                code.Append(lastDigit);

            }

            return code.ToString();
        }

        private static char EntryAt(int x, int y)
        {
            var padLines = Keypad.Split('\n');
            if (y < 0 || y >= padLines.Length)
            {
                return ' ';
            }

            if (x < 0 || x >= padLines[y].Length)
            {
                return ' ';
            }

            return padLines[y][x];
        }
        
        protected override string Input => @"DUURRDRRURUUUDLRUDDLLLURULRRLDULDRDUULULLUUUDRDUDDURRULDRDDDUDDURLDLLDDRRURRUUUDDRUDDLLDDDURLRDDDULRDUDDRDRLRDUULDLDRDLUDDDLRDRLDLUUUDLRDLRUUUDDLUURRLLLUUUUDDLDRRDRDRLDRLUUDUDLDRUDDUDLLUUURUUDLULRDRULURURDLDLLDLLDUDLDRDULLDUDDURRDDLLRLLLLDLDRLDDUULRDRURUDRRRDDDUULRULDDLRLLLLRLLLLRLURRRLRLRDLULRRLDRULDRRLRURDDLDDRLRDLDRLULLRRUDUURRULLLRLRLRRUDLRDDLLRRUDUDUURRRDRDLDRUDLDRDLUUULDLRLLDRULRULLRLRDRRLRLULLRURUULRLLRRRDRLULUDDUUULDULDUDDDUDLRLLRDRDLUDLRLRRDDDURUUUDULDLDDLDRDDDLURLDRLDURUDRURDDDDDDULLDLDLU
LURLRUURDDLDDDLDDLULRLUUUDRDUUDDUDLDLDDLLUDURDRDRULULLRLDDUDRRDRUDLRLDDDURDUURLUURRLLDRURDRLDURUDLRLLDDLLRDRRLURLRRUULLLDRLULURULRRDLLLDLDLRDRRURUUUDUDRUULDLUDLURLRDRRLDRUDRUDURLDLDDRUULDURDUURLLUDRUUUUUURRLRULUDRDUDRLLDUDUDUULURUURURULLUUURDRLDDRLUURDLRULDRRRRLRULRDLURRUULURDRRLDLRUURUDRRRDRURRLDDURLUDLDRRLDRLLLLRDUDLULUDRLLLDULUDUULLULLRLURURURDRRDRUURDULRDDLRULLLLLLDLLURLRLLRDLLRLUDLRUDDRLLLDDUDRLDLRLDUDU
RRDDLDLRRUULRDLLURLRURDLUURLLLUUDDULLDRURDUDRLRDRDDUUUULDLUDDLRDULDDRDDDDDLRRDDDRUULDLUDUDRRLUUDDRUDLUUDUDLUDURDURDLLLLDUUUUURUUURDURUUUUDDURULLDDLDLDLULUDRULULULLLDRLRRLLDLURULRDLULRLDRRLDDLULDDRDDRURLDLUULULRDRDRDRRLLLURLLDUUUDRRUUURDLLLRUUDDDULRDRRUUDDUUUDLRRURUDDLUDDDUDLRUDRRDLLLURRRURDRLLULDUULLURRULDLURRUURURRLRDULRLULUDUULRRULLLDDDDURLRRRDUDULLRRDURUURUUULUDLDULLUURDRDRRDURDLUDLULRULRLLURULDRUURRRRDUDULLLLLRRLRUDDUDLLURLRDDLLDLLLDDUDDDDRDURRL
LLRURUDUULRURRUDURRDLUUUDDDDURUUDLLDLRULRUUDUURRLRRUDLLUDLDURURRDDLLRUDDUDLDUUDDLUUULUUURRURDDLUDDLULRRRUURLDLURDULULRULRLDUDLLLLDLLLLRLDLRLDLUULLDDLDRRRURDDRRDURUURLRLRDUDLLURRLDUULDRURDRRURDDDDUUUDDRDLLDDUDURDLUUDRLRDUDLLDDDDDRRDRDUULDDLLDLRUDULLRRLLDUDRRLRURRRRLRDUDDRRDDUUUDLULLRRRDDRUUUDUUURUULUDURUDLDRDRLDLRLLRLRDRDRULRURLDDULRURLRLDUURLDDLUDRLRUDDURLUDLLULDLDDULDUDDDUDRLRDRUUURDUULLDULUUULLLDLRULDULUDLRRURDLULUDUDLDDRDRUUULDLRURLRUURDLULUDLULLRD
UURUDRRDDLRRRLULLDDDRRLDUDLRRULUUDULLDUDURRDLDRRRDLRDUUUDRDRRLLDULRLUDUUULRULULRUDURDRDDLDRULULULLDURULDRUDDDURLLDUDUUUULRUULURDDDUUUURDLDUUURUDDLDRDLLUDDDDULRDLRUDRLRUDDURDLDRLLLLRLULRDDUDLLDRURDDUDRRLRRDLDDUDRRLDLUURLRLLRRRDRLRLLLLLLURULUURRDDRRLRLRUURDLULRUUDRRRLRLRULLLLUDRULLRDDRDDLDLDRRRURLURDDURRLUDDULRRDULRURRRURLUURDDDUDLDUURRRLUDUULULURLRDDRULDLRLLUULRLLRLUUURUUDUURULRRRUULUULRULDDURLDRRULLRDURRDDDLLUDLDRRRRUULDDD";
        public override int Day => 2;
        protected override void SetupTestData()
        {
            TestData = @"ULL
RRDDD
LURDL
UUUUD";
            ExpectedResult1 = 1985;
            ExpectedResult2 = "5DB3";
        }

        protected override void CleanUp()
        {
        }

        private readonly List<string> _lines = new List<string>();
    }
}
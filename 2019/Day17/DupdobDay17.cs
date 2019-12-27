using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventCalendar2019.Day17
{
    public  class DupdobDay17
    {
        private Dictionary<long,long> _opCodes;
        private long[] _opCodesOriginal;
        private long _pointer;
        private long _inIndex;
        private long _relativeBase;
        private (int x, int y) _droid;
        private Dictionary<(int x, int y), char> _map;

        private readonly (int dx, int dy)[] _directions =  {(0,-1),(1,0),(0,1),(-1, 0)};
        private int _maxX;
        private int _maxY;

        private bool Halted { get; set; } = true;

        public static void GiveAnswers()
        {
            var puzzle = new DupdobDay17();
            puzzle.ParseInput();
            Console.WriteLine($"Answer 1: {puzzle.FindShortestPathToOxygen()}");
            Console.WriteLine($"Answer 2: {puzzle.ProgramPath()}");
        }

        private int FindShortestPathToOxygen()
        {
            var distance = ScanShip();
            return distance;
        }

        private void ParseInput(string input = Input)
        {
            _opCodesOriginal = input.Split(',').Select(long.Parse).ToArray();
        }

        private int ScanShip(string forcedMap = "")
        {
            _map = new Dictionary<(int x, int y), char>();
            (int x, int y) pos = (0, 0);

            static long InputProvider(long index)
            {
                return 0;
            }
            
            bool OutputHandler(long l)
            {
                var output = (char) l;
                Console.Write(output);
                switch (output)
                {
                    case '\n':
                        pos.y++;
                        pos.x = -1;
                        break;
                    case '#':
                        _map[pos] = output;
                        break;
                    case '.':
                        _map[pos] = output;
                        break;
                    case '^':
                    case '>':
                    case '<':
                    case 'v':
                        _droid = pos;
                        _map[pos] = output;
                        break;
                }

                pos.x++;
                return false;
            }

            if (string.IsNullOrEmpty(forcedMap))
            {
                RunProgram(InputProvider, OutputHandler);
            }
            else
            {
                foreach (var car in forcedMap)
                {
                    OutputHandler(car);
                }
            }
            

            _maxX = _map.Keys.Max(pos => pos.x)+1;
            _maxY = _map.Keys.Max(pos => pos.y)+1;
            var crosses = 0;
            for (var x = 1; x < _maxX; x++)
            {
                for (var y = 1; y < _maxY; y++)
                {
                    if (GetTypeAt((x, y)) == '#' &&
                        GetTypeAt((x - 1, y)) == '#' &&
                        GetTypeAt((x + 1, y)) == '#' &&
                        GetTypeAt((x, y - 1)) == '#' &&
                        GetTypeAt((x, y + 1)) == '#')
                    {
                        crosses+=x*y;
                    }
                }
            }
            Console.WriteLine();
            return crosses;
        }

        private int ProgramPath()
        {
            // build path
            var path = new StringBuilder();
            var direction = _map[_droid] switch
            {
                '^' => 0,
                '<' => 3,
                'v' => 2,
                '>' => 1,
                _ => 0
            };

            var pos = _droid;
            var mark = ' ';
            while ( (mark = FindNextDirection(ref pos, ref direction)) !='S')
            {
                path.Append(mark);
            }
            Console.WriteLine();
            Console.WriteLine(path);
            var fixedPath = path.ToString();
            var keepSearching = true;
            var fullProgram = new StringBuilder();
            // need to find patterns
            for (var lenA = 2; lenA < path.Length-4 && keepSearching; lenA++)
            {
                var patternA = fixedPath.Substring(0, lenA);
                if (CompressPattern(patternA).Length > 20)
                {
                    Console.WriteLine("Failed!");
                    break;
                }
                var startB = lenA;
                for(;;)
                {
                    for (var lenB = 2; lenB < path.Length-startB && keepSearching; lenB++)
                    {
                        var patternB = fixedPath.Substring(startB, lenB);
                        if (lenB>20 && CompressPattern(patternB).Length > 20)
                        {
                            break;
                        }
                
                        var startC = startB + lenB;
                        for(;;)
                        {
                            for (var lenC = 2; lenC < path.Length - startC && keepSearching; lenC++)
                            {
                                var patternC = fixedPath.Substring(startC, lenC);
                                if (lenC>20 && CompressPattern(patternC).Length > 20)
                                {
                                    break;
                                }

                                // know we need to see if we can get a solution
                                if (!Solved(fixedPath, patternA, patternB, patternC, out var solution, 0)) continue;
                                keepSearching = false;
                                fullProgram.AppendLine(solution);
                                fullProgram.AppendLine(CompressPattern(patternA));
                                fullProgram.AppendLine(CompressPattern(patternB));
                                fullProgram.AppendLine(CompressPattern(patternC));
                                Console.WriteLine(fullProgram);
                            }
                            if (fixedPath.Substring(startC, lenB) == patternB)
                            {
                                startC += lenB;
                            }
                            else if (fixedPath.Substring(startC, lenA) == patternA)
                            {
                                startC += lenA;
                            }
                            else
                            {
                                break;
                            }
                            
                        }
                    }

                    if (fixedPath.Substring(startB, lenA) == patternA)
                    {
                        startB += lenA;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            fullProgram.AppendLine("n");
            // run the program
            var input = fullProgram.ToString();
            _opCodesOriginal[0] = 2;
            RunProgram(l => input[(int)l], l =>
            {
                if (l < 256)
                {
                    Console.Write((char)l);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(l);
                }
                return false;
            });
            return 0;
        }

        private static string CompressPattern(string pattern)
        {
            var forward = 0;
            var response = new StringBuilder();
            foreach (var t in pattern)
            {
                if (t == 'F')
                {
                    forward++;
                }
                else
                {
                    if (response.Length > 0)
                    {
                        response.Append(',');
                    }
                    if (forward > 0)
                    {
                        response.Append(forward);
                        forward = 0;
                        response.Append(',');
                    }

                    response.Append(t);
                }
            }

            if (forward <= 0) return response.ToString();
            if (response.Length > 0)
            {
                response.Append(',');
            }
            response.Append(forward);

            return response.ToString();
        }

        private bool Solved(string fixedPath, string patternA, string patternB, string patternC, out string solution, int depth)
        {
            if (string.IsNullOrEmpty(fixedPath))
            {
                solution = string.Empty;
                return true;
            }

            depth++;
            if (depth > 11)
            {
                solution = string.Empty;
                return false;
            }
            if (fixedPath.StartsWith(patternA) && Solved(fixedPath.Substring(patternA.Length), patternA, patternB, patternC, out solution, depth))
            {
                if (!string.IsNullOrEmpty(solution))
                {
                    solution = "A," + solution;
                }
                else
                {
                    solution = "A";
                }
                return true;
            }
            if (fixedPath.StartsWith(patternB) && Solved(fixedPath.Substring(patternB.Length), patternA, patternB, patternC, out solution, depth))
            {
                if (!string.IsNullOrEmpty(solution))
                {
                    solution = "B," + solution;
                }
                else
                {
                    solution = "B";
                }
                return true;
            }

            if (fixedPath.StartsWith(patternC) &&
                Solved(fixedPath.Substring(patternC.Length), patternA, patternB, patternC, out solution, depth))
            {
                if (!string.IsNullOrEmpty(solution))
                {
                    solution = "C," + solution;
                }
                else
                {
                    solution = "C";
                }

                return true;
            }

            solution = string.Empty;
            return false;
        }

        private char FindNextDirection(ref (int x, int y) pos, ref int direction)
        {
            // try straight
            
            if (GetNextRoom(pos, direction) == '#')
            {
                pos.x += _directions[direction].dx;
                pos.y += _directions[direction].dy;
                return 'F';
            }

            if (GetNextRoom(pos, (direction + 3) % 4) == '#')
            {
                direction = (direction + 3) % 4;
                return 'L';
            }
            if (GetNextRoom(pos, (direction + 1) % 4) == '#')
            {
                direction = (direction + 1) % 4;
                return 'R';
            }
            // finish
            return 'S';
        }

        private int GetNextRoom((int x, int y) pos, int direction)
        {
            var (x, y) = pos;
            return GetTypeAt((x + _directions[direction].dx, y + _directions[direction].dy));
        }

        private int GetTypeAt((int x, int y) pos)
        {
            return _map.TryGetValue(pos, out var cell) ? cell : 0;
        }

        private void RunProgram(Func<long, long> inputProvider, Func<long, bool> outputHandler)
        {
            _opCodes = new Dictionary<long, long>();
            for (var i = 0; i < _opCodesOriginal.Length; i++)
            {
                _opCodes[i] = _opCodesOriginal[i];
            }

            _pointer = 0;
            _inIndex = 0;
            _relativeBase = 0;
            Halted = false;
            ContinueProgram(inputProvider, outputHandler);
        }

        private void ContinueProgram(Func<long, long> inputProvider, Func<long, bool> outputHandler)
        {
            while (_opCodes.ContainsKey(_pointer))
            {
                var opcode = _opCodes[_pointer];
                var xMode = (opcode / 100) % 10;
                var yMode = (opcode / 1000) % 10;
                var zMode = (opcode / 10000) % 10;
                opcode %= 100;
                switch (opcode)
                {
                    case 1:
                        Set(_pointer + 3, Get(_pointer + 1, xMode) + Get(_pointer + 2, yMode), zMode);
                        _pointer += 4;
                        break;
                    case 2:
                        Set(_pointer + 3, Get(_pointer + 1, xMode) * Get(_pointer + 2, yMode), zMode);
                        _pointer += 4;
                        break;
                    case 3:
                        Set(_pointer + 1, inputProvider(_inIndex++), xMode);
                        _pointer += 2;
                        break;
                    case 4:
                        var output = Get(_pointer + 1, xMode);
                        _pointer += 2;
                        if (outputHandler(output))
                        {
                            return;
                        }

                        break;
                    case 5:
                        if (Get(_pointer + 1, xMode) != 0)
                        {
                            _pointer = Get(_pointer + 2, yMode);
                        }
                        else
                        {
                            _pointer += 3;
                        }

                        break;
                    case 6:
                        if (Get(_pointer + 1, xMode) == 0)
                        {
                            _pointer = Get(_pointer + 2, yMode);
                        }
                        else
                        {
                            _pointer += 3;
                        }

                        break;
                    case 7:
                        Set(_pointer + 3, Get(_pointer + 1, xMode) < Get(_pointer + 2, yMode) ? 1 : 0, zMode);
                        _pointer += 4;
                        break;
                    case 8:
                        Set(_pointer + 3, Get(_pointer + 1, xMode) == Get(_pointer + 2, yMode) ? 1 : 0, zMode);
                        _pointer += 4;
                        break;
                    case 9:
                        _relativeBase += Get(_pointer + 1, xMode);
                        _pointer += 2;
                        break;
                    case 99:
                        Halted = true;
                        return;
                }
            }
            throw new ApplicationException("Ran out of memory");
        }

        private long Get(long i, long mode)
        {
            if (mode % 2 == 0)
            {
                i = Get(i, 1);
                if (mode == 2)
                {
                    i += _relativeBase;
                }
            }
            _opCodes.TryGetValue(i, out var val);
            return val;
        }

        private void Set(long i, long value, long mode)
        {
            i = Get(i, 1);
            if (mode == 2)
            {
                i += _relativeBase;
            }
            _opCodes[i] = value;
        }

        private const string Input = 
@"1,330,331,332,109,3093,1102,1,1182,15,1101,1428,0,24,1002,0,1,570,1006,570,36,102,1,571,0,1001,570,-1,570,1001,24,1,24,1105,1,18,1008,571,0,571,1001,15,1,15,1008,15,1428,570,1006,570,14,21101,0,58,0,1106,0,786,1006,332,62,99,21102,333,1,1,21101,0,73,0,1106,0,579,1101,0,0,572,1101,0,0,573,3,574,101,1,573,573,1007,574,65,570,1005,570,151,107,67,574,570,1005,570,151,1001,574,-64,574,1002,574,-1,574,1001,572,1,572,1007,572,11,570,1006,570,165,101,1182,572,127,102,1,574,0,3,574,101,1,573,573,1008,574,10,570,1005,570,189,1008,574,44,570,1006,570,158,1105,1,81,21102,340,1,1,1105,1,177,21101,477,0,1,1106,0,177,21102,1,514,1,21101,0,176,0,1105,1,579,99,21101,0,184,0,1106,0,579,4,574,104,10,99,1007,573,22,570,1006,570,165,102,1,572,1182,21102,375,1,1,21102,211,1,0,1106,0,579,21101,1182,11,1,21101,222,0,0,1105,1,979,21102,1,388,1,21101,0,233,0,1105,1,579,21101,1182,22,1,21101,244,0,0,1105,1,979,21101,0,401,1,21102,255,1,0,1105,1,579,21101,1182,33,1,21102,1,266,0,1106,0,979,21102,414,1,1,21101,0,277,0,1106,0,579,3,575,1008,575,89,570,1008,575,121,575,1,575,570,575,3,574,1008,574,10,570,1006,570,291,104,10,21101,1182,0,1,21102,1,313,0,1105,1,622,1005,575,327,1102,1,1,575,21101,327,0,0,1105,1,786,4,438,99,0,1,1,6,77,97,105,110,58,10,33,10,69,120,112,101,99,116,101,100,32,102,117,110,99,116,105,111,110,32,110,97,109,101,32,98,117,116,32,103,111,116,58,32,0,12,70,117,110,99,116,105,111,110,32,65,58,10,12,70,117,110,99,116,105,111,110,32,66,58,10,12,70,117,110,99,116,105,111,110,32,67,58,10,23,67,111,110,116,105,110,117,111,117,115,32,118,105,100,101,111,32,102,101,101,100,63,10,0,37,10,69,120,112,101,99,116,101,100,32,82,44,32,76,44,32,111,114,32,100,105,115,116,97,110,99,101,32,98,117,116,32,103,111,116,58,32,36,10,69,120,112,101,99,116,101,100,32,99,111,109,109,97,32,111,114,32,110,101,119,108,105,110,101,32,98,117,116,32,103,111,116,58,32,43,10,68,101,102,105,110,105,116,105,111,110,115,32,109,97,121,32,98,101,32,97,116,32,109,111,115,116,32,50,48,32,99,104,97,114,97,99,116,101,114,115,33,10,94,62,118,60,0,1,0,-1,-1,0,1,0,0,0,0,0,0,1,32,0,0,109,4,1201,-3,0,587,20102,1,0,-1,22101,1,-3,-3,21102,1,0,-2,2208,-2,-1,570,1005,570,617,2201,-3,-2,609,4,0,21201,-2,1,-2,1105,1,597,109,-4,2106,0,0,109,5,1202,-4,1,630,20101,0,0,-2,22101,1,-4,-4,21102,1,0,-3,2208,-3,-2,570,1005,570,781,2201,-4,-3,652,21001,0,0,-1,1208,-1,-4,570,1005,570,709,1208,-1,-5,570,1005,570,734,1207,-1,0,570,1005,570,759,1206,-1,774,1001,578,562,684,1,0,576,576,1001,578,566,692,1,0,577,577,21101,0,702,0,1106,0,786,21201,-1,-1,-1,1105,1,676,1001,578,1,578,1008,578,4,570,1006,570,724,1001,578,-4,578,21102,731,1,0,1105,1,786,1106,0,774,1001,578,-1,578,1008,578,-1,570,1006,570,749,1001,578,4,578,21102,756,1,0,1105,1,786,1105,1,774,21202,-1,-11,1,22101,1182,1,1,21101,0,774,0,1106,0,622,21201,-3,1,-3,1105,1,640,109,-5,2105,1,0,109,7,1005,575,802,20102,1,576,-6,20102,1,577,-5,1106,0,814,21101,0,0,-1,21102,1,0,-5,21101,0,0,-6,20208,-6,576,-2,208,-5,577,570,22002,570,-2,-2,21202,-5,37,-3,22201,-6,-3,-3,22101,1428,-3,-3,2102,1,-3,843,1005,0,863,21202,-2,42,-4,22101,46,-4,-4,1206,-2,924,21101,1,0,-1,1105,1,924,1205,-2,873,21101,0,35,-4,1105,1,924,2102,1,-3,878,1008,0,1,570,1006,570,916,1001,374,1,374,2102,1,-3,895,1101,0,2,0,1202,-3,1,902,1001,438,0,438,2202,-6,-5,570,1,570,374,570,1,570,438,438,1001,578,558,921,21002,0,1,-4,1006,575,959,204,-4,22101,1,-6,-6,1208,-6,37,570,1006,570,814,104,10,22101,1,-5,-5,1208,-5,45,570,1006,570,810,104,10,1206,-1,974,99,1206,-1,974,1101,1,0,575,21102,973,1,0,1105,1,786,99,109,-7,2106,0,0,109,6,21101,0,0,-4,21102,0,1,-3,203,-2,22101,1,-3,-3,21208,-2,82,-1,1205,-1,1030,21208,-2,76,-1,1205,-1,1037,21207,-2,48,-1,1205,-1,1124,22107,57,-2,-1,1205,-1,1124,21201,-2,-48,-2,1106,0,1041,21102,-4,1,-2,1106,0,1041,21101,0,-5,-2,21201,-4,1,-4,21207,-4,11,-1,1206,-1,1138,2201,-5,-4,1059,2102,1,-2,0,203,-2,22101,1,-3,-3,21207,-2,48,-1,1205,-1,1107,22107,57,-2,-1,1205,-1,1107,21201,-2,-48,-2,2201,-5,-4,1090,20102,10,0,-1,22201,-2,-1,-2,2201,-5,-4,1103,2101,0,-2,0,1106,0,1060,21208,-2,10,-1,1205,-1,1162,21208,-2,44,-1,1206,-1,1131,1105,1,989,21102,1,439,1,1105,1,1150,21101,0,477,1,1106,0,1150,21102,514,1,1,21101,0,1149,0,1105,1,579,99,21101,0,1157,0,1106,0,579,204,-2,104,10,99,21207,-3,22,-1,1206,-1,1138,2102,1,-5,1176,2101,0,-4,0,109,-6,2106,0,0,20,13,24,1,32,7,30,1,3,1,1,1,30,1,3,1,1,1,30,1,3,1,1,1,30,1,1,5,30,1,1,1,1,1,26,13,24,1,5,1,1,1,1,1,1,1,24,1,5,1,1,1,1,1,1,1,24,1,5,1,1,1,1,1,1,1,24,1,5,5,1,1,24,1,7,1,3,1,14,5,5,1,7,1,3,13,2,1,3,1,5,1,7,1,15,1,2,1,3,1,5,1,7,1,15,1,2,1,3,1,5,1,7,1,15,1,2,1,3,1,5,1,7,13,3,1,2,1,9,1,19,1,3,1,2,11,19,1,3,1,32,1,3,1,26,7,3,1,26,1,9,1,24,11,1,1,24,1,1,1,7,1,1,1,24,1,1,1,3,7,24,1,1,1,3,1,3,1,26,13,26,1,3,1,3,1,1,1,26,1,3,5,1,1,26,1,9,1,26,1,9,1,26,1,9,1,26,13,34,1,1,1,34,1,1,1,34,1,1,1,34,1,1,1,34,1,1,1,30,5,1,1,30,1,5,1,30,1,5,1,30,1,5,1,30,7";
    }
    
}
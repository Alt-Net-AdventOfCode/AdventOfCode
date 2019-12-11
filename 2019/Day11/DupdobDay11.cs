using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace AdventCalendar2019.Day11
{
    public  class DupdobDay11
    {
        private Dictionary<long,long> _opCodes;
        private long[] _opCodesOriginal;
        private long _pointer;
        private long _inIndex;
        private long _relativeBase;
        public bool Halted { get; private set; } = true;

        public static void GiveAnswers()
        {
            var puzzle = new DupdobDay11();
            puzzle.ParseInput();
            Console.WriteLine($"Answer 1: {puzzle.CountPaintedSquares()}");
            Console.WriteLine($"Answer 2: {puzzle.CountPaintedSquares(1)}");
        }

        private void ParseInput(string input = Input)
        {
            _opCodesOriginal = input.Split(',').Select(long.Parse).ToArray();
        }

        public long CountPaintedSquares(int firstCell = 0)
        {
            var hull = new Dictionary<(int x, int y), long>();
            (int x, int y) curPos = (0, 0);
            var nextIsPaint = true;
            var direction = 0;
            var moves = new (int dx, int dy)[]{(0, -1), (1, 0), (0, 1), (-1, 0)};
            var onFirstCell = true;
            
            long InputProvider(long index)
            {
                if (onFirstCell)
                {
                    onFirstCell = false;
                    return firstCell;
                }
                hull.TryGetValue(curPos, out var val);
                return val;
            }

            bool OutputHandler(long output)
            {
                if (nextIsPaint)
                {
                    hull[curPos] = output;
                }
                else
                {
                    direction += output == 0 ? 3 : 1;
                    direction %= 4;
                    curPos.x += moves[direction].dx;
                    curPos.y += moves[direction].dy;
                }

                nextIsPaint = !nextIsPaint;
                return false;
            }

            var memory = RunProgram(InputProvider, OutputHandler);
    
            
            // print paint result
            var minX = hull.Keys.Min(x => x.x);
            var maxX = hull.Keys.Max(x => x.x);
            var minY = hull.Keys.Min(x => x.x);
            var maxY = hull.Keys.Max(x => x.y);
            var text= new StringBuilder();
            for (var yIndex = minY; yIndex <= maxY; yIndex++)
            {
                for (var xIndex = minX; xIndex <= maxX; xIndex++)
                {
                    hull.TryGetValue((xIndex, yIndex), out var cell);
                    text.Append(cell > 0 ? '#' : ' ');
                }
                text.AppendLine();
            }
            Console.WriteLine(text);
            return hull.Count;
        }

        private long RunProgram(Func<long, long> inputProvider, Func<long, bool> outputHandler)
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
            return 0;
        }

        public void ContinueProgram(Func<long, long> inputProvider, Func<long, bool> outputHandler)
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
@"3,8,1005,8,328,1106,0,11,0,0,0,104,1,104,0,3,8,102,-1,8,10,101,1,10,10,4,10,108,1,8,10,4,10,101,0,8,28,1006,0,13,3,8,102,-1,8,10,101,1,10,10,4,10,1008,8,1,10,4,10,1002,8,1,54,1,1103,9,10,1006,0,97,2,1003,0,10,1,105,6,10,3,8,102,-1,8,10,1001,10,1,10,4,10,1008,8,1,10,4,10,1001,8,0,91,3,8,102,-1,8,10,101,1,10,10,4,10,1008,8,0,10,4,10,102,1,8,113,2,109,5,10,1006,0,96,1,2,5,10,3,8,1002,8,-1,10,101,1,10,10,4,10,1008,8,0,10,4,10,102,1,8,146,2,103,2,10,1006,0,69,2,9,8,10,1006,0,25,3,8,102,-1,8,10,1001,10,1,10,4,10,1008,8,0,10,4,10,101,0,8,182,3,8,1002,8,-1,10,101,1,10,10,4,10,108,1,8,10,4,10,1001,8,0,203,2,5,9,10,1006,0,0,2,6,2,10,3,8,102,-1,8,10,101,1,10,10,4,10,108,1,8,10,4,10,1002,8,1,236,2,4,0,10,3,8,1002,8,-1,10,1001,10,1,10,4,10,1008,8,0,10,4,10,1002,8,1,263,2,105,9,10,1,103,15,10,1,4,4,10,2,109,7,10,3,8,1002,8,-1,10,101,1,10,10,4,10,1008,8,0,10,4,10,1001,8,0,301,1006,0,63,2,105,6,10,101,1,9,9,1007,9,1018,10,1005,10,15,99,109,650,104,0,104,1,21102,387508441116,1,1,21102,1,345,0,1106,0,449,21102,1,387353256852,1,21102,1,356,0,1105,1,449,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,3,10,104,0,104,1,3,10,104,0,104,0,3,10,104,0,104,1,21101,179410308315,0,1,21102,1,403,0,1106,0,449,21101,206199495827,0,1,21102,414,1,0,1105,1,449,3,10,104,0,104,0,3,10,104,0,104,0,21102,718086758760,1,1,21102,1,437,0,1105,1,449,21101,838429573908,0,1,21102,448,1,0,1106,0,449,99,109,2,21202,-1,1,1,21102,1,40,2,21102,480,1,3,21101,470,0,0,1105,1,513,109,-2,2105,1,0,0,1,0,0,1,109,2,3,10,204,-1,1001,475,476,491,4,0,1001,475,1,475,108,4,475,10,1006,10,507,1102,0,1,475,109,-2,2106,0,0,0,109,4,2101,0,-1,512,1207,-3,0,10,1006,10,530,21101,0,0,-3,21202,-3,1,1,21201,-2,0,2,21102,1,1,3,21102,549,1,0,1105,1,554,109,-4,2106,0,0,109,5,1207,-3,1,10,1006,10,577,2207,-4,-2,10,1006,10,577,22102,1,-4,-4,1106,0,645,22102,1,-4,1,21201,-3,-1,2,21202,-2,2,3,21101,596,0,0,1106,0,554,22101,0,1,-4,21102,1,1,-1,2207,-4,-2,10,1006,10,615,21101,0,0,-1,22202,-2,-1,-2,2107,0,-3,10,1006,10,637,21201,-1,0,1,21101,637,0,0,106,0,512,21202,-2,-1,-2,22201,-4,-2,-4,109,-5,2106,0,0";
    }
    
}
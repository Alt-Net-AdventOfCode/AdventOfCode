// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
// ---
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay15: SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 15;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                ##########
                                                #..O..O.O#
                                                #......O.#
                                                #.OO..O.O#
                                                #..O@..O.#
                                                #O#..O...#
                                                #O..O..O.#
                                                #.OO.O.OO#
                                                #....O...#
                                                ##########
                                                
                                                <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                                                vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                                                ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                                                <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                                                ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                                                ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                                                >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                                                <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                                                ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                                                v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                                                
                                                """, 10092, 1);
        dayAutomatonBase.RegisterTestResult(9021, 2);
    }
    private readonly (int dy, int dx)[] _vectors = [(0, 1), (1, 0), (0,-1), (-1, 0)];
    private const string Symbols = ">v<^";

    public override object GetAnswer1()
    {
        (int y, int x) robot = (0, 0);
        var map = new List<List<char>>();
        // build initial map
        for (var y = 0; y < _map.Count; y++)
        {
            var curLine = _map[y];
            for (var x = 0; x < curLine.Count; x++)
            {
                if (curLine[x] == '@')
                {
                    robot = (y, x);
                }
            }
            map.Add([..curLine]);
        }
        
        // move
        foreach (var index in _instructions.Select(instruction => Symbols.IndexOf(instruction)).Where(index => index >= 0))
        {
            if (MoveTo(map, robot, _vectors[index], out var newPos))
            {
                map[robot.y][robot.x] = '.';
            }

            robot = newPos;
        }

        var score = PrintAndScore(map);
        return score;
    }

    private static long PrintAndScore(List<List<char>> map)
    {
        var score = 0L;
        // compute score
        for (var y = 0; y < map.Count; y++)
        {
            var curLine = map[y];
            for (var x = 0; x < curLine.Count; x++)
            {
                Console.Write(curLine[x]);
                if (curLine[x] is not 'O' and not '[') continue;
                score += y * 100 + x;
            }
            Console.WriteLine();
        }
        return score;
    }

    private bool MoveTo(List<List<char>> map, (int y, int x) robot, (int dy, int dx) vector, out (int y, int x) newPos)
    {
        (int y, int x) move = (vector.dy+robot.y, vector.dx+robot.x);
        switch (map[move.y][move.x])
        {
            case '#':
                // wall => nothing
                newPos = robot;
                return false;
            case '.':
                // empty => move
                break;
            case 'O':
            // box, need to check if we can move it in the same direction
            if (!MoveTo(map, move, vector, out newPos))
            {
                newPos = robot;
                return false;
            }
            break;
        }
        // move the item
        map[move.y][move.x] = map[robot.y][robot.x];
        newPos = move;
        return true;
    }

    public override object GetAnswer2()
    {
        (int y, int x) robot = (0, 0);
        var map = new List<List<char>>();
        for (var y = 0; y < _map.Count; y++)
        {
            var curLine = _map[y];
            var line = new StringBuilder(curLine.Count*2);
            for (var x = 0; x < curLine.Count; x++)
            {
                switch (curLine[x])
                {
                    case '#':
                    {
                        line.Append('#');
                        line.Append('#');
                        break;
                    }
                    case '.':
                    {
                        line.Append('.');
                        line.Append('.');
                        break;
                    }
                    case 'O':
                    {
                        line.Append('[');
                        line.Append(']');
                        break;
                    }
                    case '@':
                    {
                        line.Append('@');
                        line.Append('.');
                        robot = (y, 2*x);
                        break;
                    }
                }
            
            }
            map.Add(line.ToString().ToList());
        }
        
        PrintAndScore(map);
        foreach (var instruction in _instructions)
        {
            var index = Symbols.IndexOf(instruction);
            if (index < 0)
            {
                // ignore eol
                continue;
            }

            if (MoveLargeBoxTo(map, robot, _vectors[index], out var newPos, true))
            {
                MoveLargeBoxTo(map, robot, _vectors[index], out _, false);
                map[newPos.y][newPos.x] = map[robot.y][robot.x];
                map[robot.y][robot.x] = '.';
            }

            robot = newPos;
        }

        var score = PrintAndScore(map);
        return score;
    }

    private bool MoveLargeBoxTo(List<List<char>> map, (int y, int x) robot, (int dy, int dx) vector, out (int y, int x) newPos, bool attempt)
    {
        if (map[robot.y][robot.x] == '.')
        {
            // we do not move empty cell
            newPos = robot;
            return false;
        }
        (int y, int x) move = (vector.dy+robot.y, vector.dx+robot.x);
        switch (map[move.y][move.x])
        {
            case '#':
                // wall => nothing
                newPos = robot;
                return false;
            case '.':
                // empty => move
                break;
            case '[':
            case ']':
                // box, need to check if we can move it in the same direction
                var other = map[move.y][move.x] == '[' ? 1 : -1;
                if (vector.dx == 0)
                {
                    if (!MoveLargeBoxTo(map, move, vector, out var newPos2, attempt) 
                        || !MoveLargeBoxTo(map, (move.y, move.x+other), vector, out _, attempt))
                    {
                        newPos = robot;
                        return false;
                    }

                    if (!attempt)
                    {
                        // move the whole box
                        map[newPos2.y][newPos2.x] = map[move.y][move.x];
                        map[newPos2.y][newPos2.x+other] = map[move.y][move.x+other];
                        map[move.y][move.x] = '.';
                        map[move.y][move.x+other] = '.';
                    }
                }
                else
                {
                    // we need to be able to move the other side of the box
                    if (!MoveLargeBoxTo(map, (move.y, move.x+vector.dx), vector, out var newPos2, attempt))
                    {
                        newPos = robot;
                        return false;
                    }

                    if (!attempt)
                    {
                        map[robot.y][newPos2.x] = map[robot.y][move.x+vector.dx];
                        map[robot.y][move.x+vector.dx] = map[robot.y][move.x];
                        map[robot.y][move.x] = '.';
                        
                    }
                }
                break;
        }
        // move the item
        newPos = move;
        return true;
    }

    private bool _capturingMap = true;
    private readonly List<List<char>> _map = [];
    private string _instructions = string.Empty;
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line))
        {
            _capturingMap = false;
            return;
        }
        while (_capturingMap)
        {
            _map.Add(line.ToList());
            return;
        }
        _instructions += line;
    }
}
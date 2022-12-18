// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
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

using AoC;
using AoCAlgorithms;

namespace AdventCalendar2022;

public class DupdobDay17 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 17;
        automaton.RegisterTestData(">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>");
        automaton.RegisterTestData(">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>",2);
        automaton.RegisterTestResult(3068);
        automaton.RegisterTestResult(1514285714288L,2);
    }

    public override object GetAnswer1()
    {
        const int numberOfRounds = 2022;
        var chamber = new  List<char[]> { "#########".ToArray() };
        var topY = 0;
        var index = 0;
        for (var i = 0; i < numberOfRounds; i++)
        {
            var shape = _shapes[i % _shapes.Length];
            while (chamber.Count < topY + 4)
            {
                chamber.Add("#.......#".ToArray());
            }
            var posY = chamber.Count;
            var posX = 3;
            while (true)
            {
                var dir = _instructions[index++];
                var newX = posX+(dir == '>' ? 1 : -1);
                if (!DetectCollision(chamber, shape, newX, posY))
                {
                    posX = newX;
                }

                if (DetectCollision(chamber, shape, posX, posY - 1))
                {
                    // add rock to chamber
                    AddRockToChamber(shape, posX, posY, chamber);
                    topY = Math.Max(topY, posY + shape.Length-1);
                    break;
                }
                
                if (index == _instructions.Length)
                {
                    index = 0;
                }
                posY--;
            }
        }
        return topY;
    }

    private static void AddRockToChamber(IReadOnlyList<string> shape, int posX, int posY, IList<char[]> chamber)
    {
        for (var y = 0; y < shape.Count; y++)
        {
            if (posY + y >= chamber.Count)
            {
                chamber.Add("#.......#".ToArray());
            }

            for (var x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] == '#')
                {
                    chamber[posY + y][posX + x] = '#';
                }
            }
        }
    }

    private static bool DetectCollision(IReadOnlyList<char[]> chamber, IReadOnlyList<string> shape, int posX, int posY)
    {
        for (var y = 0; y < shape.Count; y++)
        {
            if (posY+y>=chamber.Count)
                return false;
            for (var x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] == '#' && chamber[posY + y][posX + x] == '#')
                {
                    return true;
                }
            }
        }

        return false;
    }


    public override object GetAnswer2()
    {
        const int numberOfRounds = 1000000;
        var chamber = new  List<char[]> { "#########".ToArray() };
        var cycleEvents = new List<(int rockId, int shapeId, int topY)>();
        var topY = 0;
        var index = 0;
        (int rocks, int height) cycle = (0, 0);
        (int rocks, int height) init = (0, 0);
        var cycleFound = false;
        var rockHeights = new List<int>();
        for (var i = 0; i < numberOfRounds && !cycleFound; i++)
        {
            var shapeId = i % _shapes.Length;
            var shape = _shapes[shapeId];
            while (chamber.Count < topY + 4)
            {
                chamber.Add("#.......#".ToArray());
            }
            var posY = chamber.Count;
            var posX = 3;
            rockHeights.Add(topY);
            while (!cycleFound)
            {
                var dir = _instructions[index++];
                var newX = posX+(dir == '>' ? 1 : -1);
                if (index == _instructions.Length)
                {
                    index = 0;
                    Console.WriteLine($"Recycle at {i} with shape {shapeId} top at {topY}");
                    cycleEvents.Add((i, shapeId, topY));
                    if (FoundCycle(cycleEvents, out cycle, out init))
                    {
                        rockHeights = rockHeights.TakeLast(cycle.rocks).ToList();
                        var start = rockHeights[0];
                        for (var j = 0; j < rockHeights.Count; j++)
                        {
                            rockHeights[j] -= start;
                        }
                        cycleFound = true;
                        break;
                    }
                }
                if (!DetectCollision(chamber, shape, newX, posY))
                {
                    posX = newX;
                }

                if (DetectCollision(chamber, shape, posX, posY - 1))
                {
                    // add rock to chamber
                    AddRockToChamber(shape, posX, posY, chamber);
                    topY = Math.Max(topY, posY + shape.Length-1);
                    break;
                }
                
                if (index == _instructions.Length)
                {
                    index = 0;
                }
                posY--;
            }
        }
        // we can compute
        var target = 1000000000000L;
        var resultingHeight = 0L;
        target -= init.rocks;
        resultingHeight += init.height;
        resultingHeight += (target / cycle.rocks) * cycle.height;
        var phase = (int)(target % cycle.rocks);
        resultingHeight += rockHeights[phase];
        return resultingHeight-1;
    }

    private static bool FoundCycle(IReadOnlyList<(int rockId, int shapeId, int topY)> cycleEvents, out (int length, int height) cycle, out (int length, int height) init)
    {
        init = (0, 0);
        cycle = (0, 0);
        
        if (cycleEvents.Count % 3 != 0)
        {
            return false;
        }
        // assumption: run starts at a (potentially) truncated cycle.
        var cycleLength = cycleEvents.Count / 3;
        for (var i = 0; i < cycleLength; i++)
        {
            if (cycleEvents[i].shapeId != cycleEvents[i + cycleLength].shapeId ||
                cycleEvents[i].shapeId != cycleEvents[i + 2 * cycleLength].shapeId)
            {
                return false;
            }
        }

        cycle.length = cycleEvents[cycleLength * 2].rockId - cycleEvents[cycleLength].rockId;
        cycle.height = cycleEvents[cycleLength * 2].topY - cycleEvents[cycleLength].topY;
        init.length = cycleEvents[cycleLength].rockId;
        init.height = cycleEvents[cycleLength].topY;
        return true;
    }

    private string _instructions = string.Empty;
    private readonly string[][] _shapes = { new []{"####"}, new []{".#.","###",".#."}, new []{"###","..#","..#"},
        new []{"#","#","#","#"}, new []{"##","##"} };
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        _instructions = line;
    }
}

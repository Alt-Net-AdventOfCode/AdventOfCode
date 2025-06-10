// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2023;

public class DupdobDay10 : SolverWithLineParser
{
    private Dictionary<(int y, int x), Node> _map = new();
    
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 10;
        dayAutomatonBase.RegisterTestDataAndResult(@"..F7.
.FJ|.
SJ.L7
|F--J
LJ...", 8, 1);

        dayAutomatonBase.RegisterTestDataAndResult(@"...........
.S-------7.
.|F-----7|.
.||.....||.
.||.....||.
.|L-7.F-J|.
.|..|.|..|.
.L--J.L--J.
...........
", 23, 1);
        dayAutomatonBase.RegisterTestResult(4, 2);
        dayAutomatonBase.RegisterTestDataAndResult(@".F----7F7F7F7F-7....
.|F--7||||||||FJ....
.||.FJ||||||||L7....
FJL7L7LJLJ||LJ.L-7..
L--J.L7...LJS7F-7L7.
....F-J..F7FJ|L7L7L7
....L7.F7||L7|.L7L7|
.....|FJLJ|FJ|F7|.LJ
....FJL-7.||.||||...
....L---J.LJ.LJLJ...", 8, 2);
        dayAutomatonBase.RegisterTestResult(70);
    }

    public override object GetAnswer1()
    {
        // we need to find the starting poing
        var startingNode = _map.Values.First(p=>p.IsStart);
        // now we need to find two nodes connected to it
        var neighbours = _map.Values.Where(p => p.First == startingNode.Coordinates || p.Second == startingNode.Coordinates).ToList();
        startingNode.Connect(neighbours[0].Coordinates, neighbours[1].Coordinates);
        // now we need to compute the loop
        var length = 1;
        var node = _map[startingNode.First];
        var prevNode = startingNode;
        startingNode.IsMainLoop = true;
        while (node != startingNode)
        {
            node.IsMainLoop = true;
            // we ensure we move forward
            if (node.First == prevNode.Coordinates)
            {
                prevNode = node;
                node = _map[node.Second];
            }
            else
            {
                prevNode = node;
                node = _map[node.First];
            }

            length++;
        }
        return length/2;
    }

    private readonly (char first, char last)[] _wallConfigurations = {('F','J'),('L', '7'),('J', 'F'),('7','L') };
    
    public override object GetAnswer2()
    {
        var maxX = _map.Values.Where(p => p.IsMainLoop).Max(p => p.Coordinates.x);
        var minX = _map.Values.Where(p => p.IsMainLoop).Min(p => p.Coordinates.x);
        var maxY = _map.Values.Where(p => p.IsMainLoop).Max(p => p.Coordinates.y);
        var minY = _map.Values.Where(p => p.IsMainLoop).Min(p => p.Coordinates.y);

        var inner = 0;
        for(var y = minY; y<=maxY; y++)
        {
            var border = 0;
            var alongAWall = false;
            char wallStart=' ';
            for (var x = minX ; x <= maxX; x++)
            {
                if ( _map.TryGetValue((y,x), out var node) && node.IsMainLoop)
                {
                    switch (node.Symbol)
                    {
                        // we cross a wall
                        case '|':
                        {
                            border++;
                            continue;
                        }
                        case '-':
                            // we are along a wall
                            continue;
                        default:
                        {
                            if (alongAWall)
                            {
                                // end of the wall
                                if (_wallConfigurations.Contains((wallStart, node.Symbol)))
                                {
                                    border++;
                                }
                                alongAWall = false;
                            }
                            else
                            {
                                // we keep the first wall we met
                                wallStart = node.Symbol;
                                alongAWall = true;
                            }

                            break;
                        }
                    }
                }
                else
                {
                    if (border % 2 == 1)
                    {
                        Console.WriteLine($"Found at ({y},{x}).");
                        inner++;
                    }
                }
            }
        }

        return inner;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        for (var x = 0; x < line.Length; x++)
        {
            if (line[x] == '.')
            {
                continue;
            }

            var node = new Node((index, x), line[x]);
            _map[(index, x)] = node;
        }
    }

    private class Node
    {
        private const string Symbols = "-|F7LJ";
        private static readonly (int dy, int dx)[] Firsts = { (0, -1), (-1, 0), (0, 1), (0, -1), (0, 1), (0, -1) }; 
        private static readonly (int dy, int dx)[] Seconds = { (0, 1), (1, 0), (1, 0), (1, 0), (-1, 0), (-1, 0) }; 
        public Node((int y, int x) coordinates, char type)
        {
            Coordinates = coordinates;
            if (type == 'S')
            {
                IsStart = true;
                return;
            }

            Symbol = type;
            var index = Symbols.IndexOf(type);
            
            First = (coordinates.y + Firsts[index].dy, coordinates.x + Firsts[index].dx);
            Second = (coordinates.y + Seconds[index].dy, coordinates.x + Seconds[index].dx);
        }

        public char Symbol { get; }
        public (int y, int x) Coordinates { get; }
        public (int y, int x) First { get; private set; }
        public (int y, int x) Second { get; private set; }
        public bool IsMainLoop { get; set; }
        public bool IsStart { get; }

        public void Connect((int y, int x) first, (int y, int x) second)
        {
            First = first;
            Second = second;
        }
    }
}
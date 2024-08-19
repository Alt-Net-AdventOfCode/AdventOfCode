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

namespace AdventCalendar2023;

public class DupdobDay17 : SolverWithLineParser
{
    private readonly List<string> _map = new();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 17;
        automatonBase.RegisterTestDataAndResult(@"2413432311323
3215453535623
3255245654254
3446585845452
4546657867536
1438598798454
4457876987766
3637877979653
4654967986887
4564679986453
1224686865563
2546548887735
4322674655533", 102,1);
        automatonBase.RegisterTestResult(94, 2);

        automatonBase.RegisterTestDataAndResult(@"111111111111
999999999991
999999999991
999999999991
999999999991", 71, 2);
    }

    public override object GetAnswer1()
    {
        int[] PathConstraint(int straight, int currentDir)
        {
            var possibleDir = straight == 3 ? new[] { (currentDir + 1) % 4, (currentDir + 3) % 4 } : new[] { currentDir, (currentDir + 1) % 4, (currentDir + 3) % 4 };
            return possibleDir;
        }

        return MinimalHeatLoss(PathConstraint);
    }

    public override object GetAnswer2()
    {
        // we must keep in same direction for at least 4 steps and at max 10
        int[] PathConstraint(int straight, int currentDir)
        {
            var possibleDir = straight  < 4 ?  new[] { currentDir } : 
                straight == 10 ? new [] { (currentDir + 1) % 4, (currentDir + 3) % 4 } : 
                new[] { currentDir, (currentDir + 1) % 4, (currentDir + 3) % 4 };
            return possibleDir;
        }

        return MinimalHeatLoss(PathConstraint);
    }

    private object MinimalHeatLoss(Func<int, int, int[]> pathConstraint)
    {
        var priorityQueue = new List<((int y, int x) pos, (int l, int dir) route, int distance)>();
        var distances = new Dictionary<((int y, int x), (int straitghs, int dir)), int>();
        var previous = new Dictionary<(int y, int x), ((int y, int x) pos, int dir)>();
        var width = _map[0].Length;
        var height = _map.Count;
        var minimalHeatLoss = int.MaxValue;
        // we start top left, pretending to come from below 
        priorityQueue.Add(((0,0), (0,-1), 0));
        distances[((0, 0), (0,-1))] = 0;
        while (priorityQueue.Count>0)
        {
            var minDist = int.MaxValue;
            ((int y, int x) pos, (int l, int dir) route, int distance) next = ((0, 0), (0, 0), 0);
            // find the closest point
            foreach (var (pos, route, dist) in priorityQueue)
            {
                if (dist >= minDist) continue;
                minDist = dist;
                next = (pos, route, dist);
            }

            priorityQueue.Remove(next);
            var currentDir = next.route.dir;
            var straight = next.route.l;
            var possibleDir = currentDir == -1 ? new []{0,1} : pathConstraint(straight, currentDir);

            if (next.pos == (height - 1, width - 1))
            {
                if (possibleDir.Length == 1)
                {
                    continue;
                }
                // found a path to end
                if (minDist < minimalHeatLoss)
                {
                    minimalHeatLoss = minDist;
                }
            
                if (priorityQueue.Count > 0)
                {
                    continue;
                }
                var route = BuildRoute(previous);
                PrintMapWithRoute(route);
                return minimalHeatLoss;
            }
            
            // try close cells
            foreach (var direction in possibleDir)
            {
                var nextPos = FindNeighbours(next.pos, direction, width, height);
                if (nextPos == null)
                {
                    continue;
                }

                var nextDistance = minDist + _map[nextPos.Value.y][nextPos.Value.x] - '0';
                var routeL = currentDir==direction ? next.route.l+1 : 1;
                // if we have not less more heat than the shortest found path (if any) and
                // if we have not visited the cell or we have found a shorter path to it
                if (nextDistance <= minimalHeatLoss 
                    && (!distances.TryGetValue((nextPos.Value,( routeL,direction)), out var currentDist) || nextDistance < currentDist))
                {
                    // stores the new heat less
                    distances[(nextPos.Value, (routeL, direction))] = nextDistance;
                    // keeps track of our path for debugging purposes
                    previous[nextPos.Value] = (next.pos, direction);
                    // keep on navigating from here
                    priorityQueue.Add((nextPos.Value, (routeL, direction), nextDistance));
                }
            }
        }
        //PrintMapWithRoute(BuildRoute(previous));
        return minimalHeatLoss;
    }

    private void PrintMapWithRoute(List<((int, int) pos, int dir)> route)
    {
        var width = _map[0].Length;
        var height = _map.Count;
        var output = new StringBuilder((width + 2) * height);
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                output.Append(route.Any(v => v.pos == (y, x))
                    ? Arrows[route.First(v => v.pos == (y, x)).dir]
                    : _map[y][x]);
            }

            output.AppendLine();
        }
        Console.Write(output);
    }

    private List<((int, int) pos, int dir)> BuildRoute(Dictionary<(int y, int x), ((int y, int x) pos, int dir)> previous)
    {
        var width = _map[0].Length;
        var height = _map.Count;
        var route = new List<((int, int) pos, int dir)>();
        var pos = (height-1, width-1);
        while (pos.Item1 !=0 || pos.Item2!=0)
        {
            route.Insert(0, (pos, previous[pos].dir));
            pos = previous[pos].pos;
        }

        return route;
    }

    private static (int y, int x)? FindNeighbours((int y, int x) pos, int dir, int width, int height)
    {
        pos = (pos.y+Vectors[dir].dy , pos.x + Vectors[dir].dx);
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
        {
            return null;
        }

        return pos;
    }
    
    private static readonly (int dy, int dx)[] Vectors = { (0, 1), (1, 0), (0, -1), (-1, 0) };
    private static readonly char[] Arrows = { '>', 'v', '<', '^' };

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line);
    }
}
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

using AoC;

namespace AdventCalendar2024;

public class DupdobDay16 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 16;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                ###############
                                                #.......#....E#
                                                #.#.###.#.###.#
                                                #.....#.#...#.#
                                                #.###.#####.#.#
                                                #.#.#.......#.#
                                                #.#.#####.###.#
                                                #...........#.#
                                                ###.#.#####.#.#
                                                #...#.....#.#.#
                                                #.#.#.###.#.#.#
                                                #.....#...#.#.#
                                                #.###.#.#.#.#.#
                                                #S..#.....#...#
                                                ###############
                                                """, 7036, 1);
        dayAutomatonBase.RegisterTestResult(45,2);
    }

    private record Point(int Y, int X);
    private record DeerState(Point Position, int Direction);
    private readonly (int dy, int dx)[] _vectors = [(0, 1), (1, 0), (0,-1), (-1, 0)];

    public override object GetAnswer1()
    {
        var width = _map[0].Length;
        var height = _map.Count;
        var distances = new Dictionary<DeerState, int>(height*width);
        var previousStates = new Dictionary<DeerState, List<DeerState>>(height*width);
        var queue = new PriorityQueue<DeerState, int>();
        var deerState = new DeerState(_start, 0);
        queue.Enqueue(deerState, 0);
        distances[deerState] = 0;
        _previousStates = previousStates;
        while (queue.Count>0)
        {
            deerState = queue.Dequeue();
            var distance = distances[deerState];
            // try to move forward
            var next = NextCell(deerState);
            if (next is { X: >= 0, Y: >= 0 } && next.X < width && next.Y < height && _map[next.Y][next.X] == '.')
            {
                var nextState = deerState with { Position = next };
                var oneStep = distance + 1;
                if (!distances.TryGetValue(nextState, out var nextDistance) || nextDistance >= oneStep)
                {
                    if (_previousStates.TryGetValue(nextState, out var previousState))
                    {
                        previousState.Add(deerState);
                    }
                    else
                    {
                        _previousStates[nextState] = [deerState];
                    }
                    // found a shortest path
                    distances[nextState] = oneStep;
                    queue.Enqueue(nextState, oneStep);
                }
            }
            // now we rotate
            for (var i = 1; i < 4; i++)
            {
                var nextState = deerState with { Direction = (deerState.Direction+i) % 4 };
                var oneStep = distance + (i>2 ? 1000 : i * 1000);
                if (distances.TryGetValue(nextState, out var nextDistance) && nextDistance <= oneStep)
                {
                    continue;
                }
                // found a shortest path
                if (_previousStates.TryGetValue(nextState, out var previousState))
                {
                    previousState.Add(deerState);
                }
                else
                {
                    _previousStates[nextState] = [deerState];
                }
                distances[nextState] = oneStep;
                queue.Enqueue(nextState, oneStep);
            }
        }
        // we need to try all possible end directions
        var minDistance = int.MaxValue;
        for (var direction = 0; direction < 4; direction++)
        {
            var tryState = new DeerState(_end, direction);
            if (distances.TryGetValue( tryState, out var nextDistance) &&
                nextDistance < minDistance)
            {
                minDistance = nextDistance;
                deerState = tryState;
            }
        }

        _finaleState = deerState;
        return minDistance;
    }

    private Point NextCell(DeerState deerState) => new(deerState.Position.Y+_vectors[deerState.Direction].dy, deerState.Position.X+_vectors[deerState.Direction].dx);

    public override object GetAnswer2()
    {
        var positions = new HashSet<Point>();
        var statesToVisit = new Queue<DeerState>();
        statesToVisit.Enqueue(_finaleState);
        var visitedStates = new HashSet<DeerState>();
        while (statesToVisit.Count>0)
        {
            var state = statesToVisit.Dequeue();
            positions.Add(state.Position);
            if (visitedStates.Add(state) && _previousStates.TryGetValue(state, out var previousState))
            {
                foreach (var deerState in previousState)
                {
                    statesToVisit.Enqueue(deerState);
                }                
            }
        }
        return positions.Count;
    }

    private readonly List<string> _map = [];
    private Point _end = null!;
    private Point _start = null!;
    private Dictionary<DeerState, List<DeerState>> _previousStates = null!;
    private DeerState _finaleState = null!;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var end = line.IndexOf('E');
        if (end >= 0)
        {
            _end= new Point(index, end);
            line = line.Replace('E','.');
        }
        else
        {
            end = line.IndexOf('S');
            if (end >= 0)
            {
                _start = new Point(index, end);
                line = line.Replace('S','.');
            }
        }
        _map.Add(line);
    }
}
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

namespace AdventCalendar2023;

public class DupdobDay21: SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 21;
        dayAutomatonBase.RegisterTestDataAndResult(@"6
...........
.....###.#.
.###.##..#.
..#.#...#..
....#.#....
.##..S####.
.##..#...#.
.......##..
.##.#.####.
.##..##.##.
...........", 16, 1);
    }

    public override object GetAnswer1()
    {
        _map.TrimExcess();
        _startY = _map.FindIndex(s => s.Contains('S'));
        _startX = _map[_startY].IndexOf('S');
        _height = _map.Count;
        _width = _map[0].Length;
        
        var distances = ComputeDistances(_startY, _startX, _steps);

        return distances;
    }

    private int ComputeDistances(int startY, int startX, int max)
    {
        var distances = new Dictionary<(int, int), int>(_height*_width);
        (int y, int x) current = (startY, startX);
        distances[current] = 0;
        var queue = new PriorityQueue<(int y, int x), int>();
        queue.Enqueue(current, distances[current]);
        while (queue.Count > 0)
        {
            current = queue.Dequeue();
            var nextDist = distances[current]+1;
            if (nextDist > max)
            {
                continue;
            }
            Check((current.y, current.x - 1), distances, nextDist, queue);
            Check((current.y, current.x + 1), distances, nextDist, queue);
            Check((current.y-1, current.x), distances, nextDist, queue);
            Check((current.y+1, current.x), distances, nextDist, queue);
        }
        var parity = max % 2;

        return distances.Count(d => d.Value % 2 == parity);
    }

    private void Check((int y, int x) pos, Dictionary<(int, int), int> distances, int nextDistance,
        PriorityQueue<(int y, int x), int> queue)
    {
        var y = pos.y <0 ? pos.y - (pos.y/_height-1)*_height : pos.y;
        var x = pos.x <0 ? pos.x - (pos.x/_width-1)*_width : pos.x;
        if (_map[y % _height][x% _width]=='#') return;
        if (distances.TryGetValue(pos, out var dist) && dist <= nextDistance) return;
        distances[pos] = nextDistance;
        queue.Enqueue(pos, nextDistance);
    }

    public override object GetAnswer2()
    {
        var maxSteps = 26501365;
        var maximumRepeat = (maxSteps-_width/2)/_width;
        var factorX = 9;
        var factorY = 9;
        
        var sequenceOfReachableDistances = new List<int>();
        for (var x = 0; x <= 3; x++)
        {
            var steps = x * _width + _width / 2;
            var distances = ComputeDistances(_startY+(factorY-1)/2*_height, _startX+(factorX-1)/2*_width, steps);
            sequenceOfReachableDistances.Add(distances);
        }

        // we observe there is a logical progression in # of reachable square. This makes sense as the map is a repetitive pattern
        var hierarchicalSequence = new List<List<long>> { sequenceOfReachableDistances.Select(v => (long)v).ToList() };
        // find  the steps at each level, until we find a constant one
        while (hierarchicalSequence.Last().Count>1)
        {
            var nextList = new List<long>();
            for (var i = 0; i < hierarchicalSequence.Last().Count-1; i++)   
                nextList.Add(hierarchicalSequence.Last()[i+1]-hierarchicalSequence.Last()[i]);
            hierarchicalSequence.Add(nextList);
        }
        // now we can build the sequence
        while (hierarchicalSequence[0].Count <= maximumRepeat)
        {
            for (var i = hierarchicalSequence.Count - 2; i >=0; i--)
            {
                hierarchicalSequence[i].Add(hierarchicalSequence[i].Last() + hierarchicalSequence[i + 1].Last());
            }
        }
        return hierarchicalSequence[0].Last();
    }

    private readonly List<string> _map = new();
    private int _steps = 64;
    private int _startY;
    private int _startX;
    private int _height;
    private int _width;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (index == 0 && int.TryParse(line, out var steps))
        {
            _steps = steps;
            return;
        }
        _map.Add(line);
    }
}
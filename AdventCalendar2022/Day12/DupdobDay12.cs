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

namespace AdventCalendar2022;

public class DupdobDay12 : SolverWithLineParser
{
    private readonly List<List<char>> _list = new();
    private (int x, int y) _start;
    private (int x, int y) _end;

    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 12;
        // ReSharper disable StringLiteralTypo
        automaton.RegisterTestData(@"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi");
        automaton.RegisterTestResult(31);
        automaton.RegisterTestResult(29, 2);
    }

    public override object GetAnswer1()
    {
        var current = _start;
        return FindDistanceFromThisStart(current);
    }

    private int FindDistanceFromThisStart((int x, int y) current)
    {
        var distances = new Dictionary<(int x, int y), int>();
        var queue = new List<(int x, int y)>();
        distances[current] = 0;
        queue.Add(current);
        while (queue.Count > 0)
        {
            var minDist = int.MaxValue;
            foreach (var coordinates in queue.Where(coordinates => distances[coordinates] < minDist))
            {
                current = coordinates;
                minDist = distances[coordinates];
            }

            queue.Remove(current);
            if (current == _end) return distances[current];

            foreach (var next in EnumerateNeighbours(current))
            {
                if (distances.ContainsKey(next) && distances[next] <= minDist + 1) continue;
                distances[next] = minDist + 1;
                queue.Add(next);
            }
        }

        return int.MaxValue;
    }

    private char GetHeight((int x, int y ) cell)
    {
        return _list[cell.y][cell.x];
    }

    private IEnumerable<(int x, int y)> EnumerateNeighbours((int x, int y) cell)
    {
        var currentHeight = GetHeight(cell) + 1;
        if (cell.x > 0 && GetHeight((cell.x - 1, cell.y)) <= currentHeight) yield return (cell.x - 1, cell.y);
        if (cell.y > 0 && GetHeight((cell.x, cell.y - 1)) <= currentHeight) yield return (cell.x, cell.y - 1);
        if (cell.x < _list[cell.y].Count - 1 && GetHeight((cell.x + 1, cell.y)) <= currentHeight)
            yield return (cell.x + 1, cell.y);
        if (cell.y < _list.Count - 1 && GetHeight((cell.x, cell.y + 1)) <= currentHeight)
            yield return (cell.x, cell.y + 1);
    }

    public override object GetAnswer2()
    {
        var minDist = int.MaxValue;
        for (var y = 0; y < _list.Count; y++)
        for (var x = 0; x < _list[y].Count; x++)
        {
            var current = (x, y);
            if (GetHeight(current) == 'a') minDist = Math.Min(minDist, FindDistanceFromThisStart(current));
        }

        return minDist;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line)) return;

        var heights = new List<char>();
        for (var i = 0; i < line.Length; i++)
        {
            var car = line[i];
            switch (car)
            {
                case 'S':
                    _start = (i, index);
                    car = 'a';
                    break;
                case 'E':
                    _end = (i, index);
                    car = 'z';
                    break;
            }

            heights.Add(car);
        }

        _list.Add(heights);
    }
}
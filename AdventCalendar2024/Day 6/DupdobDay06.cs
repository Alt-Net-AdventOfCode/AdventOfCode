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

public class DupdobDay06: SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 6;
        automatonBase.RegisterTestDataAndResult(
            """
            ....#.....
            .........#
            ..........
            ..#.......
            .......#..
            ..........
            .#..^.....
            ........#.
            #.........
            ......#...
            """, 41, 1);
        automatonBase.RegisterTestResult(6, 2);
    }

    private readonly List<string> _map = [];
    private (int x, int y) _start;
    private const int Dir = 0;
    private readonly (int dx, int dy)[] _vectors = [(0, -1), (1, 0), (0, 1), (-1, 0)];
    private readonly HashSet<(int x, int y)> _visited = [];
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line);
        var start = line.IndexOf('^');
        if (start > -1)
        {
            _start = (start, index);
        }
    }
    
    public override object GetAnswer1()
    {
        var pos = _start;
        var right = _map[0].Length-1;
        var bottom = _map.Count-1;
        var dir = Dir;
        while (true)
        {
            _visited.Add(pos);
            (int x, int y) next = (pos.x+_vectors[dir].dx, pos.y+_vectors[dir].dy);
            if (next.x < 0 || next.y < 0 || next.x > right || next.y > bottom)
            {
                // we exit the area
                break;
            }
            if (_map[next.y][next.x] == '#')
            {
                // we turn right
                dir = (dir +1) % _vectors.Length;
            }
            else
            {
                pos = next;
            }
        }
        return _visited.Count;
    }

    private readonly int[] _masks = [1,2,4,8];
    public override object GetAnswer2()
    {
        var options = 0;
        var right = _map[0].Length-1;
        var bottom = _map.Count-1;
        foreach (var attempt in _visited)
        {
            if (attempt == _start)
            {
                continue;
            }
            
            var original = _map[attempt.y];
            _map[attempt.y] = (attempt.x > 0 ? original[..attempt.x]: "")+'#'+ (attempt.x<right ? original[(attempt.x+1)..] : "");
            // now we try a visit
            var visited = new Dictionary<(int x, int y), int>();
            var pos = _start;
            var dir = Dir;
            while (true)
            {
                var mask = _masks[dir];
                var dirs = visited.GetValueOrDefault(pos);
                if ((dirs & mask) == mask)
                {
                    // we have a loop
                    options++;
                    break;
                } 
                
                dirs |= mask;
                visited[pos] = dirs;
                
                (int x, int y) next = (pos.x+_vectors[dir].dx, pos.y+_vectors[dir].dy);
                if (next.x < 0 || next.y < 0 || next.x > right || next.y > bottom)
                {
                    // we exit the area
                    break;
                }
                if (_map[next.y][next.x] == '#')
                {
                    // we turn right
                    dir = (dir +1) % _vectors.Length;
                }
                else
                {
                    pos = next;
                }
            }
            
            // restore line
            _map[attempt.y] = original;
        }
        return options;
    }
}
// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2017;

public class DupdobDay14: SolverWithDataAsLines
{
    private string _prefix = null!;

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 14;
        automatonBase.RegisterTestDataAndResult("flqrgnkx", 8108, 1).RegisterTestResult(1242, 2);
    }

    private readonly bool[,] _map = new bool[128,128];

    public override object GetAnswer1()
    {
        var result = 0;
        
        for (var i = 0; i < 128; i++)
        {
            var hash = HashRound($"{_prefix}-{i}");
            for (var j = 0; j < hash.Count; j++)
            {
                var mask = 128;
                for (var k = 0; k < 8; k++)
                {
                    if ((hash[j] & mask) == mask)
                    {
                        _map[i, j * 8 + k] = true;
                        result++;
                    }

                    mask >>= 1;
                }
            }
            
        }

        return result;
    }

    private readonly (int Dy, int Dx)[] _vectors = [(0, 1), (1, 0), (0, -1), (-1, 0)];
    public override object GetAnswer2()
    {
        var area = 0;
        var visited = new HashSet<(int y, int x)>();
        for (var y = 0; y < _map.GetLength(1); y++)
        {
            for (var x = 0; x < _map.GetLength(0); x++)
            {
                if (!_map[y, x] || !visited.Add((y, x)))
                {
                    continue;
                }
                // scan neighbors
                var pending = new Queue<(int y, int x)>();
                pending.Enqueue((y,x));
                area++;
                while (pending.TryDequeue(out var current))
                {
                    foreach (var valueTuple  in _vectors)
                    {
                        (int y, int x) next = (current.y + valueTuple.Dy, current.x + valueTuple.Dx);
                        if (next.x<0 || next.y<0 || next.x>=_map.GetLength(0) || next.y>=_map.GetLength(1)
                            || !_map[next.y, next.y] || !visited.Add(next))
                        {
                            continue;
                        }
                        pending.Enqueue(next);
                    }
                }
            }
        }

        return area;
    }
    
    private static List<int> HashRound(string text)
    {
        var list = Enumerable.Range(0, 256).ToList();
        var current = 0;
        var skip = 0;
        var input = text.ToList(); 
        input.AddRange([(char)17, (char)31, (char)73, (char)47, (char)23]);
        for (var j = 0; j < 64; j++)
        {
            foreach (var length in input)
            {
                var next = list.ToList();
                var end = current + length - 1;
                for (var i = 0; i < length; i++)
                {
                    next[current++ % list.Count] = list[end-- % list.Count];
                }

                current += skip;
                skip++;
                list = next;
            }
        }

        var hash = new List<int>(16);
        for (var i = 0; i < 16; i++)
        {
            var local = 0;
            for (var j = i * 16; j < (i + 1) * 16; j++)
            {
                local ^= list[j];
            }

            hash.Add(local);
        }
        return hash;
    }
    
    protected override void ParseLines(string[] lines)
    {
        _prefix = lines[0];
    }
}
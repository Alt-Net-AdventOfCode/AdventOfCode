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

public class DupdobDay22 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 22;
        dayAutomatonBase.RegisterTestDataAndResult(@"1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9", 5, 1);
        dayAutomatonBase.RegisterTestResult(7, 2);
    }
    
    private const int X = 0;
    private const int Y = 1;
    private const int Z = 2;

    private static int Orientation(int[] a, int[] b, int[] c)
    {
        return Math.Sign((b[Y]-a[Y])*(c[X]-b[X])-(b[X]-a[X])*(c[Y]-b[Y]));
    }

    private static bool IsOInSegment(int[] begin, int[] end, int[] point)
    {
        return point[X] <= Math.Max(begin[X], end[X]) && point[X] >= Math.Min(begin[X], end[X])
                                                      && point[Y] <= Math.Max(begin[Y], end[Y]) &&
                                                      point[Y] >= Math.Min(begin[Y], end[Y]);
    }
    
    private bool IsSupportedBy((int[] begin, int[] end) support, (int[] begin, int[] end) falling)
    {
        if (Math.Max(support.begin[Z], support.end[Z])+1 != Math.Min(falling.begin[Z], falling.end[Z]))
        {
            // support is below or above
            return false;
        }
        
        var o1= Orientation(support.begin, support.end, falling.begin);
        var o2= Orientation(support.begin, support.end, falling.end);
        var o3= Orientation(falling.begin, falling.end, support.begin);
        var o4= Orientation(falling.begin, falling.end, support.end);
        // do segment crosses?
        if (o1!=o2 && o3!=o4) return true;
        // we may still have 3 points aligned
        if (o1 == 0 && IsOInSegment(support.begin, support.end, falling.begin))
            return true;
        if (o2 == 0 && IsOInSegment(support.begin, support.end, falling.end))
            return true;
        if (o3 == 0 && IsOInSegment(falling.begin, falling.end, support.begin))
            return true;
        if (o4 == 0 && IsOInSegment(falling.begin, falling.end, support.end))
            return true;
        // nope, no form of crossing
        return false;
    }
    
    public override object GetAnswer1()
    {
        var orderedBlocks  = _bricks.OrderBy(b =>Math.Min(b.start[Z], b.end[Z])).ToList();
        for (var i = 0; i < orderedBlocks.Count; i++)
        {
            var curBlock = orderedBlocks[i];
            bool canGoDown;
            var supportedBy = new List<int>();
            var z = Math.Min(curBlock.start[Z], curBlock.end[Z]);
            do
            {
                for (var j = 0; j < i; j++)
                {
                    if (IsSupportedBy(orderedBlocks[j], curBlock))
                    {
                        supportedBy.Add(j);
                    }
                }

                canGoDown = z>0 && supportedBy.Count==0;
                if (canGoDown)
                {
                    z--;
                    curBlock.start[Z]--;
                    curBlock.end[Z]--;
                }
            } while (canGoDown);
            _supportDictionary.Add(i, supportedBy);
        }
        
        // now we need to count removable blocks
        var result = 0;
        for(var i=0;i< _bricks.Count;i++)
        {
            if (_supportDictionary.Values.All(list => list.Count != 1 || list[0] != i))
            {
                result++;
                _supports.Add(i);
            }
        }
        return result;
    }

    public override object GetAnswer2()
    {
        var result = 0L;
        for (var i = 0; i < _bricks.Count; i++)
        {
            if (_supports.Contains(i))
            {
                // removing this has no effect, we skip
                continue;
            }
            // removing this one would have an effect
            var removed = new HashSet<int>();
            var modified = removed.Add(i);
            while (modified)
            {
                modified = false;
                for (var j = 0; j < _bricks.Count; j++)
                {
                    if (removed.Contains(j)) continue;
                    if (_supportDictionary.TryGetValue(j, out var list) && list.Count>0 && list.All( t => removed.Contains(t)))
                    {
                        modified = removed.Add(j);
                    }
                }
            }
            result += removed.Count-1;
        }
        return result;
    }

    private readonly List<(int[] start, int[] end)> _bricks = new();
    private readonly Dictionary<int, List<int>> _supportDictionary = new();
    private readonly HashSet<int> _supports = new();
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var blocks = line.Split("~");
        _bricks.Add((blocks[0].Split(',').Select(int.Parse).ToArray(),blocks[1].Split(',').Select(int.Parse).ToArray()));
    }
}
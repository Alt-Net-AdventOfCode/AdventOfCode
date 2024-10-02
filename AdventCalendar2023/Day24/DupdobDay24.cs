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

using System.Collections;
using System.Runtime.InteropServices;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay24 : SolverWithLineParser
{
    private long _maxDim=400000000000000;
    private long _minDim=200000000000000;
    private readonly List<List<long>> _hailStones = new();
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 24;
        automatonBase.RegisterTestDataAndResult(@"7,27
19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3", 2, 1);
    }

    public override object GetAnswer1()
    {
        var trajectories = new List<(double[] begin, double[]end)>(_hailStones.Count);
        // we need to compute hailstone trajectory in the window
        foreach (var hailStone in _hailStones)
        {
            var x = hailStone[0];
            var y = hailStone[1];
            var dx = hailStone[3];
            var dy = hailStone[4];
            if (x < _minDim && dx < 0
                || x > _maxDim && dx > 0
                || y < _minDim && dy < 0
                || y > _maxDim && dy > 0)
            {
                // if will never cross the window of interest
                continue;
            }
            
            // compute need time to cross the window's border (if time is negative we never cross it)
            var minXTime = Math.Max(0, (_minDim - x + (dx>0 ? dx-1 : 0)) / dx);
            var maxXTime = Math.Max(0, (_maxDim - x - (dx<0 ? dx-1 : 0)) / dx);
            var minYTime = Math.Max(0, (_minDim - y+ (dy>0 ? dy-1 : 0)) / dy);
            var maxYTime = Math.Max(0, (_maxDim - y- (dy<0 ? dy-1 : 0)) / dy);
            if (minXTime > maxXTime)
            {
                (minXTime, maxXTime) = (maxXTime, minXTime);
            }

            if (minYTime > maxYTime)
            {
                (minYTime, maxYTime) = (maxYTime, minYTime);
            }
            var (minTime, maxTime) = (Math.Max(minXTime, minYTime)+1, Math.Min(maxXTime, maxYTime));
            if (maxTime < minTime)
            {
                Console.Write("HailStone {0}:{1}; {2}:{3} does not enter the cross window.", x, y, dx, dy);
                continue;
            }
            trajectories.Add((new double[]{ x+dx*minTime, y+dy*minTime}, new double[] {x+dx*maxTime, y+dy*maxTime}));
        }

        var result = 0;
        for (var i = 0; i < trajectories.Count; i++)
        {
            for (var j = i+1; j < trajectories.Count; j++)
            {
                if (SegmentsCross(trajectories[i], trajectories[j]))
                {
                    result++;
                }
            }
        }

        return result;
    }
    
    private const int X = 0;
    private const int Y = 1;
    private const int Z = 2;

    private static int Orientation(IList<double> a, IList<double> b, IList<double> c)
    {
        return Math.Sign((b[Y]-a[Y])*(c[X]-b[X])-(b[X]-a[X])*(c[Y]-b[Y]));
    }

    private static bool IsInSegment(IList<double> begin, IList<double> end, IList<double> point)
    {
        return point[X] <= Math.Max(begin[X], end[X]) && point[X] >= Math.Min(begin[X], end[X])
                                                      && point[Y] <= Math.Max(begin[Y], end[Y]) &&
                                                      point[Y] >= Math.Min(begin[Y], end[Y]);
    }
    
    private bool SegmentsCross((IList<double> begin, IList<double> end) first, (IList<double> begin, IList<double> end) second)
    {
        var o1= Orientation(first.begin, first.end, second.begin);
        var o2= Orientation(first.begin, first.end, second.end);
        var o3= Orientation(second.begin, second.end, first.begin);
        var o4= Orientation(second.begin, second.end, first.end);
        // do segment crosses?
        if (o1!=o2 && o3!=o4) return true;
        // we may still have 3 points aligned
        if (o1 == 0 && IsInSegment(first.begin, first.end, second.begin))
            return true;
        if (o2 == 0 && IsInSegment(first.begin, first.end, second.end))
            return true;
        if (o3 == 0 && IsInSegment(second.begin, second.end, first.begin))
            return true;
        if (o4 == 0 && IsInSegment(second.begin, second.end, first.end))
            return true;
        // nope, no form of crossing
        return false;
    }

    public override object GetAnswer2()
    {
        return null;
    }
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var fields = line.Split('@').SelectMany(t => t.Split(',').Select(long.Parse)).ToList();
        if (fields.Count == 2)
        {
            (_minDim, _maxDim) = (fields[0], fields[1]);
            return;
        }
        _hailStones.Add(fields);
    }
}

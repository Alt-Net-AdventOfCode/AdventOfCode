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
        automatonBase.RegisterTestResult(47, 2);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        for (var i = 0; i < _hailStones.Count; i++)
        {
            for (var j = i+1; j < _hailStones.Count; j++)
            {
                var crossing = ComputeCrossing(_hailStones[i], _hailStones[j]);
                if (crossing!= null && crossing.Value.x>=_minDim && crossing.Value.x<=_maxDim && crossing.Value.y>=_minDim && crossing.Value.y<=_maxDim)
                {
                    result++;
                }
            }
        }

        return result;
    }

    private (long x, long y, decimal time)? ComputeCrossing(IList<long> hailStoneA, IList<long> hailStoneB)
    {
        if (hailStoneA[Dx] == 0 || hailStoneB[Dx] == 0)
        {
            return null;
        }
        var slopeA = (decimal)hailStoneA[Dy] / hailStoneA[Dx];
        var slopeB = (decimal)hailStoneB[Dy] / hailStoneB[Dx];
        if (slopeA == slopeB)
        {
            return null;
        }
        var dYa= hailStoneA[Y]-hailStoneA[X]*slopeA;
        var dYb= hailStoneB[Y]-hailStoneB[X]*slopeB;

        var crossingX = (dYb - dYa) / (slopeA - slopeB);
        var time = (crossingX -hailStoneA[X])/hailStoneA[Dx];
        if (time < 0 || (crossingX - hailStoneB[X])/hailStoneB[Dx] < 0)
        {
            return null;
        }

        try
        {
            var result = ((long)crossingX, (long)(hailStoneA[Y] + (crossingX - hailStoneA[X]) * slopeA), time);
            return result;

        }
        catch (OverflowException)
        {
            return null;
        }
    }
    
    private const int X = 0;
    private const int Dx = 3;
    private const int Y = 1;
    private const int Dy = 4;
    private const int Z = 2;
    private const int Dz = 5;

    public override object? GetAnswer2()
    {
        var maxMatches = 0;
        var maxSpeed = 500;
        var minSpeed = -maxSpeed;
        while (true)
        {
            var randomList = new List<List<long>>();
            var indexes = new HashSet<int>();
            var generator = new Random();
            while (randomList.Count < 4)
            {
                var index = generator.Next() % _hailStones.Count;
                if (indexes.Add(index))
                {
                    randomList.Add(_hailStones[index]);
                }
            }
        
            for (var dx=minSpeed; dx<=maxSpeed; dx++)
            for (var dy = minSpeed; dy <= maxSpeed; dy++)
            {
                var hailStone0 = randomList[0].ToList();
                hailStone0[Dx] += dx;
                hailStone0[Dy] += dy;
                var crossings = new List<(long x, long y, decimal time)>(randomList.Count-1);
                for (var i = 1; i < randomList.Count; i++)
                {
                    var nextStone = randomList[i].ToList();
                    nextStone[Dx] += dx;
                    nextStone[Dy] += dy;
                    var intersect = ComputeCrossing(nextStone, hailStone0);
                    if (intersect == null)
                    {
                        break;
                    }
                    if (crossings.Count>0 && (intersect.Value.x != crossings[0].x || intersect.Value.y != crossings[0].y))
                    {
                        break;
                    }

                    crossings.Add(intersect.Value);
                }
            
                maxMatches = Math.Max(maxMatches, crossings.Count);
                if (crossings.Count <randomList.Count-1)
                {
                    continue;
                }
                Console.WriteLine("Found common crossing at {0},{1} for dx{2}, dy{3}", crossings[0].x,crossings[0].y, dx, dy);
                for (var dz = minSpeed; dz <= maxSpeed; dz++)
                {
                    var z = (long) (crossings[0].time*(randomList[1][Dz]+dz)+randomList[1][Z]);
                    var isOk = true;
                    for (var i = 2; i < randomList.Count; i++)
                    {
                        if (z != (long)(crossings[i - 1].time * (randomList[i][Dz] + dz) + randomList[i][Z]))
                        {
                            isOk = false;
                            break;
                        }
                    }

                    if (isOk)
                    {
                        return z+crossings[0].x+crossings[0].y;
                    }
                }
            }
            // if we arrive here we failed to find a valid solution
            Console.WriteLine("Max matches was {0}.", maxMatches);
            
        }
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

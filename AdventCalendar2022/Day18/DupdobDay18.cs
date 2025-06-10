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

public class DupdobDay18 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 18;
        dayAutomaton.AddExample(@"2,2,2
1,2,2
3,2,2
2,1,2
2,3,2
2,2,1
2,2,3
2,2,4
2,2,6
1,2,5
3,2,5
2,1,5
2,3,5");        
        
        dayAutomaton.RegisterTestResult(64);
        dayAutomaton.RegisterTestResult(58,2);
    }

    public override object GetAnswer1()
    {
        _initialSurface = ComputeSurface(_obsidian);
        return _initialSurface;
    }

    private static int ComputeSurface(List<int[]> list)
    {
        var surface = 0;
        for (var i = 0; i < list.Count; i++)
        {
            var freeFaces = 6;
            for (var j = 0; j < list.Count && freeFaces > 0; j++)
            {
                if (i == j)
                    continue;
                var dist = 0;
                for (var k = 0; k < 3 && dist <= 1; k++)
                {
                    dist += Math.Abs(list[i][k] - list[j][k]);
                }

                if (dist == 1)
                    freeFaces--;
            }

            surface += freeFaces;
        }

        return surface;
    }

    public override object GetAnswer2()
    {
        var min = new int[3];
        var max = new int[3];
        for (var k = 0; k < 3; k++)
        {
            min[k] = int.MaxValue;
            max[k] = int.MinValue;
        }
        foreach (var cube in _obsidian)
        {
            for (var k = 0; k < 3; k++)
            {
                min[k] = Math.Min(min[k], cube[k]-1);
                max[k] = Math.Max(max[k], cube[k]+1);
            }
        }
    
        var world = new bool[max[0] - min[0] + 1, max[1] - min[1] + 1, max[2] - min[2] + 1];
        // we fill the world with existing cubes
        foreach (var cube in _obsidian)
        {
            world[cube[0]-min[0], cube[1]-min[1], cube[2]-min[2]] = true;
        }
        // now, flood will from one corner
        var seeds = new HashSet<(int x, int y, int z)>(100);
        seeds.Add((0, 0, 0));
        while (seeds.Count>0)
        {
            var point = seeds.First();
            seeds.Remove(point);
            foreach ((int x, int y, int z) next in EnumerateNeighbors(world, point))
            {
                world[next.x, next.y, next.z] = true;
                seeds.Add(next);
            }
        }
        // now find the empty cell;
        var empty = new List<int[]>();
        for (var x = world.GetLowerBound(0); x <= world.GetUpperBound(0); x++)
        {
            for (var y = world.GetLowerBound(1); y <= world.GetUpperBound(1); y++)
            {
                for (var z = world.GetLowerBound(2); z <= world.GetUpperBound(2); z++)
                {
                    if (!world[x, y, z])
                    {
                        empty.Add(new []{x,y,z});
                    }
                }
            }
        }
        return _initialSurface- ComputeSurface(empty);
    }

    private static IEnumerable<(int x, int y, int z)> EnumerateNeighbors(bool[,,] world, (int x, int y, int z) rock)
    {
        var next =rock with{x = rock.x-1};
        if (next.x >= world.GetLowerBound(0) && !world[next.x, next.y, next.z])
        {
            yield return next;
        }
        next =rock with{y = rock.y-1};
        if (next.y >= world.GetLowerBound(1) && !world[next.x, next.y, next.z])
        {
            yield return next;
        }
        next =rock with{z = rock.z-1};
        if (next.z >= world.GetLowerBound(2) && !world[next.x, next.y, next.z])
        {
            yield return next;
        }
        next =rock with{x = rock.x+1};
        if (next.x <= world.GetUpperBound(0) && !world[next.x, next.y, next.z])
        {
            yield return next;
        }
        next =rock with{y = rock.y+1};
        if (next.y <= world.GetUpperBound(1) && !world[next.x, next.y, next.z])
        {
            yield return next;
        }
        next =rock with{z = rock.z+1};
        if (next.z <= world.GetUpperBound(2) && !world[next.x, next.y, next.z])
        {
            yield return next;
        }
    }
    
    private readonly List<int []> _obsidian = new();
    private int _initialSurface;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }
        _obsidian.Add(line.Split(',').Select(int.Parse).ToArray());
    }
}
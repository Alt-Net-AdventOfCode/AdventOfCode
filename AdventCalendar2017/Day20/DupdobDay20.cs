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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2017;

public partial class DupdobDay20 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 20;
        automatonBase.RegisterTestDataAndResult("""
                                       p=< 3,0,0>, v=< 2,0,0>, a=<-1,0,0>
                                       p=< 4,0,0>, v=< 0,0,0>, a=<-2,0,0>
                                       """, 0, 1);

        automatonBase.RegisterTestDataAndResult("""
                                                p=<-6,0,0>, v=< 3,0,0>, a=< 0,0,0>    
                                                p=<-4,0,0>, v=< 2,0,0>, a=< 0,0,0>
                                                p=<-2,0,0>, v=< 1,0,0>, a=< 0,0,0>
                                                p=< 3,0,0>, v=<-1,0,0>, a=< 0,0,0>
                                                """, 1, 2);
    }

    public override object GetAnswer1()
    {
        var closestParticule = 0;
        for (var i = 1; i < _particles.Count; i++)
        {
            if (Compare(_particles[closestParticule], _particles[i]) > 0)
            {
                closestParticule = i;
            }
        }

        return closestParticule;
    }

    private int Compare((Triplet position, Triplet speed, Triplet aceleration) x,
        (Triplet position, Triplet speed, Triplet aceleration) y)
    {
        var magnitudeX = Amplitude(x.aceleration);
        var magnitudeY = Amplitude(y.aceleration);
        if (magnitudeX != magnitudeY)
        {
            return magnitudeX<magnitudeY ? -1 : 1;
        }            
        magnitudeX = Amplitude(x.speed);
        magnitudeY = Amplitude(y.speed);
        if (magnitudeX != magnitudeY)
        {
            return magnitudeX<magnitudeY ? -1 : 1;
        }
        magnitudeX = Amplitude(x.position);
        magnitudeY = Amplitude(y.position);
        if (magnitudeX != magnitudeY)
        {
            return magnitudeX<magnitudeY ? -1 : 1;
        }

        return 0;
    }

    private static double Amplitude(Triplet triplet) => Math.Sqrt(triplet.X * triplet.X + triplet.Y * triplet.Y + triplet.Z * triplet.Z);

    public override object GetAnswer2()
    {
        var collisions = ComputeCollisions();
        var collided = new HashSet<int>();
        foreach (var (key,list) in collisions.OrderBy(p => p.Key))
        {
            collisions[key] = list.Except(collided).ToList();
            if (collisions[key].Count == 1)
            {
                // nothing to collide with
                collisions[key] = [];
                continue;
            }

            foreach (var particle in list)
            {
                collided.Add(particle);
            }
        }

        return _particles.Count-collided.Count;
        
    }

    private Dictionary<int, List<int>> ComputeCollisions()
    {
        var collisions = new Dictionary<int, List<int>>();
        for (var i = 0; i < _particles.Count; i++)
        {
            var referenceParticles = _particles[i];
            // check for potential collisions
            for (var j = i + 1; j < _particles.Count; j++)
            {
                var currentParticles = _particles[j];
                var a = (referenceParticles.aceleration.X - currentParticles.aceleration.X) / 2.0;
                var b = (referenceParticles.speed.X + referenceParticles.aceleration.X/2.0) 
                        - (currentParticles.speed.X + currentParticles.aceleration.X/2.0);
                var c = (double)(referenceParticles.position.X - currentParticles.position.X);
                var delta = b * b- 4 * a * c;
                if (delta < 0)
                {
                    continue;
                }

                if (a == 0 && b == 0)
                {
                    if (c != 0)
                    {
                        continue;
                    }
                }
                
                var t1 = a == 0 ? -c/b : (Math.Sqrt(delta) - b) / (2 * a);
                var t2 = a == 0 ? -1 :  (-Math.Sqrt(delta) - b) / (2 * a);

                if (t1 == t2)
                {
                    t2 = -1;
                }
                foreach (var solution in new []{ t1, t2 })
                {
                    if (solution < 0 || solution - Math.Round(solution) != 0) 
                    {
                        continue;
                    }
                    // check if it works
                    if ( PositionAt(referenceParticles, solution) != PositionAt(currentParticles, solution))
                    {
                        continue;
                    }

                    var rounded = (int)solution;
                    if (!collisions.TryGetValue(rounded, out var list))
                    {
                        collisions[rounded] = list = [i];
                    }
                    else
                    {
                        list.Add(i);
                    }
                    list.Add(j);
                }
            }
        }


        return collisions;
    }

    private Triplet PositionAt((Triplet position, Triplet speed, Triplet aceleration) particle, double time)
    {
        var x = time*(time+1)*particle.aceleration.X/2+time*particle.speed.X+particle.position.X;
        var y = time*(time+1)*particle.aceleration.Y/2+time*particle.speed.Y+particle.position.Y;
        var z = time*(time+1)*particle.aceleration.Z/2+time*particle.speed.Z+particle.position.Z;
        return new Triplet((long)x, (long)y, (long)z);
    }

    private record Triplet(long X, long Y, long Z);
    
    private readonly Regex _parser =
        MyRegex();

    private readonly List<(Triplet position, Triplet speed, Triplet aceleration)> _particles = [];
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _parser.Match(line);
        if (!match.Success)
        {
            Console.WriteLine("Failed to parse {0}", line);
            return;
        }

        var position = new Triplet(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)); 
        var speed = new Triplet(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value)); 
        var acceleration = new Triplet(int.Parse(match.Groups[7].Value), int.Parse(match.Groups[8].Value), int.Parse(match.Groups[9].Value)); 
        _particles.Add((position, speed, acceleration));
    }

    [GeneratedRegex(@"p=< ?(-?\d+),(-?\d+),(-?\d+)>, v=< ?(-?\d+),(-?\d+),(-?\d+)>, a=< ?(-?\d+),(-?\d+),(-?\d+)>")]
    private static partial Regex MyRegex();
}
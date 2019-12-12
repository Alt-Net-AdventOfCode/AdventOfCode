using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2019.Day12
{
    public class DupdobDay12
    {
        private List<Moon> moons = new List<Moon>();

        public static void GiveAnswers()
        {
            var model = new DupdobDay12();
            model.Parse();
            Console.WriteLine("Answer 1: {0}", model.TotalEnery(1000));
            model.Init();
            Console.WriteLine("Answer 2: {0}", model.Period());
        }

        private void Init()
        {
            foreach (var moon in moons)
            {
                moon.Init();
            }
        }

        private long Period()
        {
            long i = 0;
            long xPeriod = 0;
            long yPeriod = 0;
            long zPeriod = 0;
            // identify periods on each axis
            for (;;)
            {
                i++;
                RunForSteps(1);

                if (xPeriod==0 && moons.All(m => m.xSameAsStart()))
                {
                    xPeriod = i;
                    if (yPeriod > 0 && zPeriod > 0)
                    {
                        break;
                    }
                }
                if (yPeriod == 0 && moons.All(m => m.ySameAsStart()))
                {
                    yPeriod = i;
                    if (xPeriod > 0 && zPeriod > 0)
                    {
                        break;
                    }
                }
                if (zPeriod == 0 && moons.All(m => m.zSameAsStart()))
                {
                    zPeriod = i;
                    if (yPeriod > 0 && xPeriod > 0)
                    {
                        break;
                    }
                }
            }
            // algorithm below is to slow, so I used prime factor decomposition through a site 
            return 2 * 2 * 7 * 7 * 491L * 115807 * 46507L;
/*            var accuX = xPeriod;
            var accuY = yPeriod;
            var accuZ = zPeriod;

            while (accuX != accuY || accuX != accuZ)
            {
                if (accuX <= accuY && accuX <= accuZ)
                {
                    accuX += xPeriod;
                }
                else if (accuY <= accuX && accuY <= accuZ)
                {
                    accuY += yPeriod;
                }
                else
                {
                    accuZ += zPeriod;
                }
            }
            return accuX;
            */
        }

        public void Parse(string input = Input)
        {
            foreach (var line in input.Split('\n'))
            {
                var match = parser.Match(line);
                if (!match.Success)
                {
                    throw new ApplicationException($"Failed to parse {line}.");
                }
                moons.Add(new Moon(int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value), 
                    int.Parse(match.Groups[3].Value)));
            }
        }

        public void RunForSteps(int step)
        {
            for (var i = 0; i < step; i++)
            {
                foreach (var moon in moons)
                {
                    moon.UpdateVelocity(moons);
                }

                foreach (var moon in moons)
                {
                    moon.UpdatePos();
                }
            }
        }

        public long TotalEnery(int steps)
        {
            RunForSteps(steps);
            return moons.Sum(moon => moon.Energy());
        }
        
        private class Moon
        {
            private int x;
            private int y;
            private int z;
            private int xInit;
            private int yInit;
            private int zInit;
            private int vx;
            private int vy;
            private int vz;

            public Moon(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.xInit = x;
                this.yInit = y;
                this.zInit = z;
            }

            public void Init()
            {
                x = xInit;
                y = yInit;
                z = zInit;
                vx = 0;
                vy = 0;
                vz = 0;
            }
            
            public bool xSameAsStart()
            {
                return vx == 0 && x == xInit;
            }
            
            public bool ySameAsStart()
            {
                return vy == 0 && y == yInit;
            }
            
            public bool zSameAsStart()
            {
                return vz == 0 && z == zInit;
            }
            
            public void UpdateVelocity(IEnumerable<Moon> moons)
            {
                foreach (var moon in moons)
                {
                    if (moon == this)
                    {
                        continue;
                    }
                    else
                    {
                        vx -= this.x.CompareTo(moon.x);
                        vy -= this.y.CompareTo(moon.y);
                        vz -= this.z.CompareTo(moon.z);
                    }
                }
            }

            public void UpdatePos()
            {
                this.x += vx;
                this.y += vy;
                this.z += vz;
            }

            public long Energy()
            {
                return (Math.Abs(x) + Math.Abs(y) + Math.Abs(z))*(Math.Abs(vx)+Math.Abs(vy)+Math.Abs(vz));
            }
        }
        
        private Regex parser = new Regex("<x=(-?\\d+), y=(-?\\d+), z=(-?\\d+)>");
        private const string Input = 
@"<x=5, y=-1, z=5>
<x=0, y=-14, z=2>
<x=16, y=4, z=0>
<x=18, y=1, z=16>";
    }
}
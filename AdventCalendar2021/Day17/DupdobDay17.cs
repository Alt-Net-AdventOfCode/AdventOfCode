using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventCalendar2021
{
    public class DupdobDay17: AdvancedDay
    {
        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;

        private int _minDx;
        private readonly Regex _parser = new Regex("target area: x=(-?\\d*)..(-?\\d*), y=(-?\\d*)..(-?\\d*)");
        public DupdobDay17() : base(17)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var match = _parser.Match(line);
            if (!match.Success)
            {
                throw new InvalidOperationException("Failed to parse");
            }

            _minX = int.Parse(match.Groups[1].Value);
            _maxX = int.Parse(match.Groups[2].Value);
            _minY = int.Parse(match.Groups[3].Value);
            _maxY = int.Parse(match.Groups[4].Value);
        }

        protected override IEnumerable<(string intput, object result)> GetTestData(bool secondQuestion)
        {
            yield return ("target area: x=20..30, y=-10..-5", secondQuestion ? 112 : 45);
        }

        public override object GiveAnswer1()
        {
            // find dx
            var edgeX = Math.Min(Math.Abs(_minX), Math.Abs(_maxX));
            var dX = 0;
            while (edgeX>0)
            {
                dX++;
                edgeX -= dX;
            }

            // scan for Dy;
            var dY = 0;
            for (;;)
            {
                var probe = new Probe
                {
                    Dx = dX,
                    Dy = dY
                };
                var inTarget = false;
                while (!Missed(probe))
                {
                    probe.Step();
                    if (!inTarget)
                    {
                        inTarget = InTarget(probe);
                    }
                }

                if (inTarget)
                {
                    break;
                }
                if (probe.X < _minX)
                {
                    // left
                    dY++;
                }
                else
                {
                    dY--;
                }
            }

            _minDx = dX;
            var highest = int.MinValue;
            // we have a starting dY;
            while (dY<1000)
            {
                var probe = new Probe
                {
                    Dx = dX,
                    Dy = dY
                };

                if (Shoot(probe, out var localMax))
                {
                    highest = Math.Max(highest, localMax);
                }

                dY++;
            }
            return highest;
        }

        private bool Shoot(Probe probe, out int localMax)
        {
            var inTarget = false;
            localMax = int.MinValue;
            while (!Missed(probe))
            {
                probe.Step();
                if (probe.Dy == 0)
                {
                    localMax = probe.Y;
                }

                inTarget = InTarget(probe);
                if (inTarget)
                {
                    break;
                }
            }

            return inTarget;
        }

        private bool Missed(Probe probe)
        {
            return probe.Y < _minY;
        }

        private bool InTarget(Probe probe)
        {
            return probe.X >= _minX && probe.X <= _maxX && probe.Y >= _minY && probe.Y <= _maxY;
        }

        private class Probe
        {
            public int X;
            public int Y;
            public int Dx;
            public int Dy;

            public void Step()
            {
                X += Dx;
                Y += Dy;
                switch (Dx)
                {
                    case > 0:
                        Dx--;
                        break;
                    case < 0:
                        Dx++;
                        break;
                }

                Dy--;
            }
        }
        // > 5852
        public override object GiveAnswer2()
        {
            var count = 0;
            for (var dX = _minDx; dX <= _maxX; dX++)
            {
                for (var dY = _minY; dY <= 1000; dY++)
                {
                    var probe = new Probe
                    {
                        Dx = dX,
                        Dy = dY
                    };
                    if (Shoot(probe, out var _))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        protected override void CleanUp()
        {
        }
    }
}
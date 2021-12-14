using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay13 : AdvancedDay
    {
        private readonly List<(int x, int y)> _dots = new();
        private readonly List<(bool horizontal, int axis)> _folds = new();
        private bool _readingDots = true;
        
        public DupdobDay13() : base(13)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                _readingDots = false;
                return;
            }

            if (_readingDots)
            {
                var coords = line.Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
                _dots.Add((coords[0], coords[1]));
            }
            else
            {
                var instructions = line.Split('=', StringSplitOptions.TrimEntries);
                _folds.Add((instructions[0][^1] == 'x', int.Parse(instructions[1])));
            }
        }

        public override object GiveAnswer1()
        {
            Fold(_folds[0]);
            return _dots.Distinct().Count();
        }

        private void Fold((bool horizontal, int axis) fold)
        {
            for (var i = 0; i < _dots.Count; i++)
            {
                var dot = _dots[i];
                if (fold.horizontal)
                {
                    if (dot.x > fold.axis)
                    {
                        dot.x = 2 * fold.axis - dot.x;
                    }
                }
                else
                {
                    if (dot.y > fold.axis)
                    {
                        dot.y = 2 * fold.axis - dot.y;
                    }
                }

                _dots[i] = dot;
            }
        }

        public override object GiveAnswer2()
        {
            foreach (var valueTuple in _folds)
            {
                Fold(valueTuple);
            }

            var width = _dots.Max(d => d.x);
            var height = _dots.Max(d => d.y);
            Console.WriteLine();
            for (var y = 0; y <= height; y++)
            {
                for (var x = 0; x <= width; x++)
                {
                    Console.Write(_dots.Contains((x,y)) ? '*' : ' ');
                }
                Console.WriteLine();
            }

            return null;
        }

        protected override void SetupTestData(int id)
        {
            _expectedResult1 = 17;
            // no expected result for step 2 as it requires visual inspection
            _testData = @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5";
        }

        protected override void SetupRunData()
        {
            _readingDots = true;
            _dots.Clear();
            _folds.Clear();
        }
    }
}
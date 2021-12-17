using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOCHelpers;

namespace AdventCalendar2020.Day17
{
    public class DupdobDay17: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '#')
                {
                    _activeCells.Add((i, index, 0));
                }
            }
        }

        public override object GiveAnswer1()
        {
            var map = _activeCells;
            for (var turn = 0; turn < _nbTurns; turn++)
            {
                _maxX = 0;
                _maxY = 0;
                _maxZ = 0;
                _minX = 0;
                _minY = 0;
                _minZ = 0;
                foreach (var (x, y, z) in map)
                {
                    _minX = Math.Min(_minX, x);
                    _minY = Math.Min(_minY, y);
                    _minZ = Math.Min(_minZ, z);
                    _maxX = Math.Max(_maxX, x);
                    _maxY = Math.Max(_maxY, y);
                    _maxZ = Math.Max(_maxZ, z);
                }

                var nextMap = new HashSet<(int x, int y, int z)>(map.Count);
                for (var z = _minZ - 1; z <= _maxZ + 1; z++)
                {
                    for (var y = _minY - 1; y <= _maxY + 1; y++)
                    {
                        for (var x = _minX - 1; x <= _maxX + 1; x++)
                        {
                            var countNeighbors = 0;
                            for (var zt = z - 1; zt <= z + 1; zt++)
                            {
                                for (var yt = y - 1; yt <= y + 1; yt++)
                                {
                                    for (var xt = x - 1; xt <= x + 1; xt++)
                                    {
                                        if (xt == x && yt == y && zt ==z)
                                        {
                                            continue;
                                        }
                                        
                                        if (map.Contains((xt, yt, zt)))
                                        {
                                            countNeighbors++;
                                            if (countNeighbors > 3)
                                            {
                                                break;
                                            }
                                        }
                                        if (countNeighbors > 3)
                                        {
                                            break;
                                        }
                                    }
                                    if (countNeighbors > 3)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (map.Contains((x, y, z)))
                            {
                                if (countNeighbors == 2 || countNeighbors ==3)
                                {
                                    nextMap.Add((x, y, z));
                                }
                            }
                            else if (countNeighbors == 3)
                            {
                                nextMap.Add((x, y, z));
                            }
                        }
                    }
                }

                map = nextMap;
            }
            
            return map.Count;
        }

        public override object GiveAnswer2()
        {
            var map = new HashSet<(int x, int y, int z, int w)>();
            foreach (var (x, y, z) in _activeCells)
            {
                map.Add((x,y,z, 0));
            }
            for (var turn = 0; turn < _nbTurns; turn++)
            {
                _maxX = 0;
                _maxY = 0;
                _maxZ = 0;
                _maxW = 0;
                _minX = 0;
                _minY = 0;
                _minZ = 0;
                _minW = 0;
                foreach (var (x, y, z, w) in map)
                {
                    _minX = Math.Min(_minX, x);
                    _minY = Math.Min(_minY, y);
                    _minZ = Math.Min(_minZ, z);
                    _minW = Math.Min(_minW, w);
                    _maxX = Math.Max(_maxX, x);
                    _maxY = Math.Max(_maxY, y);
                    _maxZ = Math.Max(_maxZ, z);
                    _maxW = Math.Max(_maxW, w);
                }

                var nextMap = new HashSet<(int x, int y, int z, int a)>(map.Count);
                for(var w = _minW-1; w<=_maxW+1; w++)
                {
                    for (var z = _minZ - 1; z <= _maxZ + 1; z++)
                    {
                        for (var y = _minY - 1; y <= _maxY + 1; y++)
                        {
                            for (var x = _minX - 1; x <= _maxX + 1; x++)
                            {
                                var countNeighbors = 0;
                                for (var wt = w -1; wt <= w+1; wt++)
                                {
                                    for (var zt = z - 1; zt <= z + 1; zt++)
                                    {
                                        for (var yt = y - 1; yt <= y + 1; yt++)
                                        {
                                            for (var xt = x - 1; xt <= x + 1; xt++)
                                            {
                                                if (xt == x && yt == y && zt ==z && wt == w)
                                                {
                                                    continue;
                                                }
                                        
                                                if (map.Contains((xt, yt, zt, wt)))
                                                {
                                                    countNeighbors++;
                                                    if (countNeighbors > 3)
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (countNeighbors > 3)
                                                {
                                                    break;
                                                }
                                            }
                                            if (countNeighbors > 3)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    
                                }

                                if (map.Contains((x, y, z, w)))
                                {
                                    if (countNeighbors == 2 || countNeighbors ==3)
                                    {
                                        nextMap.Add((x, y, z, w));
                                    }
                                }
                                else if (countNeighbors == 3)
                                {
                                    nextMap.Add((x, y, z, w));
                                }
                            }
                        }
                    }
                }

                map = nextMap;
            }

            return map.Count;
        }

        protected override void SetupTestData()
        {
            TestData = @".#.
..#
###";
            ExpectedResult1 = 112;
            ExpectedResult2 = 848;
            _nbTurns = 6;
        }

        protected override void CleanUp()
        {
            _activeCells.Clear();
        }

        private int _maxX;
        private int _maxY;
        private int _maxZ;
        private int _minX;
        private int _minY;
        private int _minZ;
        private readonly HashSet<(int x, int y, int z)> _activeCells = new();
        private int _nbTurns;
        private int _maxW;
        private int _minW;

        protected override string Input => @"##......
.##...#.
.#######
..###.##
.#.###..
..#.####
##.####.
##..#.##";
        public override int Day => 17;
    }
}
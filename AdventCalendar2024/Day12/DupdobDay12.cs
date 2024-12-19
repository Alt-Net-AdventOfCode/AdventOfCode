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

namespace AdventCalendar2024;

public class DupdobDay12 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 12;
        automatonBase.RegisterTestDataAndResult("""
                                                AAAA
                                                BBCD
                                                BBCC
                                                EEEC
                                                """, 140, 1);
        automatonBase.RegisterTestResult(80, 2);
        
        automatonBase.RegisterTestDataAndResult("""
                                                OOOOO
                                                OXOXO
                                                OOOOO
                                                OXOXO
                                                OOOOO
                                                """, 772, 1);
        automatonBase.RegisterTestResult(436, 2);
        automatonBase.RegisterTestDataAndResult("""
                                                EEEEE
                                                EXXXX
                                                EEEEE
                                                EXXXX
                                                EEEEE
                                                """, 236, 2);

        automatonBase.RegisterTestDataAndResult("""
                                                RRRRIICCFF
                                                RRRRIICCCF
                                                VVRRRCCFFF
                                                VVRCCCJFFF
                                                VVVVCJJCFE
                                                VVIVCCJJEE
                                                VVIIICJJEE
                                                MIIIIIJJEE
                                                MIIISIJEEE
                                                MMMISSJEEE
                                                """, 1206, 2);
        
    }

    public override object GetAnswer1()
    {
        _areaCharacteristics = new Dictionary<int, (int area, int perimeter, char type)>();
        var areaId = 1;
        _areas = new int[_map.Count, _map[0].Length];
        for (var y = 0; y < _map.Count; y++)
        {
            for (var x = 0; x < _map[y].Length; x++)
            {
                var type = _map[y][x];
                var (perimeter, id) = (0, areaId);
                if (y == 0 || _map[y - 1][x] != type)
                {
                    perimeter++;
                }
                else
                {
                    // we assume we continue areaId from the line above
                    id = _areas[y - 1, x];
                }

                if (x == 0 || _map[y][x-1] != type)
                {
                    perimeter++;
                }
                else
                {
                    // we inherit the areaId
                    // but we may merge two regions
                    if (id != areaId && _areas[y, x - 1] != id)
                    {
                        var newId = _areas[y, x - 1];
                        if (newId < id)
                        {
                            (newId, id) = (id, newId);
                        }
                        _areaCharacteristics[id] = (_areaCharacteristics[id].area+_areaCharacteristics[newId].area, 
                            _areaCharacteristics[id].perimeter+_areaCharacteristics[newId].perimeter, type);
                        for (var i = 0; i <= y; i++)
                        {
                            for (var j = 0; j < _areas.GetLength(1); j++)
                            {
                                if (_areas[i, j] == newId)
                                {
                                    _areas[i, j] = id;
                                }
                            }
                        }
                        // we loose the old area
                        _areaCharacteristics.Remove(newId);
                    }
                    id = _areas[y, x-1];
                }

                if (y == _map.Count-1 || _map[y+1][x] != type)
                {
                    perimeter++;
                }
                if (x == _map[y].Length-1 || _map[y][x+1] != type)
                {
                    perimeter++;
                }
                _areas[y, x] = id;
                if (_areaCharacteristics.TryGetValue(id, out var areaCharacteristic))
                {
                    _areaCharacteristics[id] = (areaCharacteristic.area+1, 
                        _areaCharacteristics[id].perimeter+perimeter, type);
                }
                else
                {
                    _areaCharacteristics[id] = (1, perimeter, type);
                    areaId++;
                }
            }
        }

        return _areaCharacteristics.Values.Aggregate(0L, (acc, entry) => 
            acc+entry.perimeter*entry.area);
    }

    private readonly (int dy, int dx)[] _vectors = [(0, 1), (1, 0), (0,-1), (-1, 0)];
    
    // this not a general solution as it does not properly some 'inner zones'.
    // the ones that are made of more than 1 type of zone.
    public override object GetAnswer2()
    {
        var height = _map.Count;
        var width = _map[0].Length;
        var areaEdges = new Dictionary<int, int>();
        foreach (var currentZone in _areaCharacteristics.Keys)
        {
            var x =0;
            int y;
            // find the first cell
            var found = false;
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    if (_areas[y,x] != currentZone) continue;
                    found = true;
                    break;
                }

                if (found)
                {
                    break;
                }
            }
            
            // we know there is a different kind of cell on top
            var dir = 0;
            var initPos = (y, x);
            var edge = 0;
            var neighbors = new HashSet<int>();

            // we store the neighbor on top
            if (y > 0)
            {
                neighbors.Add(_areas[y-1, x]);
            }
            // we follow the edge
            do
            {
                (int y , int x) next = (y+_vectors[dir].dy, x+_vectors[dir].dx);
                if (next.y<0 || next.x<0 || next.y>=height || next.x>=width)
                {
                    // we reached a border of the map
                    // we need to turn right, we do not move
                    neighbors.Add(0);
                    edge++;
                    dir = (dir + 1) % 4;
                }
                else if (_areas[next.y, next.x] != currentZone)
                {
                    // there is an edge, we may turn right, but we will turn left for this case
                    // as the 'inner' is considered as within X region (not on edge) 
                    //?X?
                    //X?X
                    neighbors.Add(_areas[next.y, next.x]);
                    edge++;
                    dir = (dir + 1) % 4;
                    var nextDir = (dir + 2) % 4;
                    (int y, int x) corner = (next.y + _vectors[nextDir].dy, next.x+_vectors[nextDir].dx);
                    if (corner is not { y: >= 0, x: >= 0 } || corner.y >= height || corner.x >= width) continue;
                    var cornerZone = _areas[corner.y, corner.x];
                    if (cornerZone == currentZone)
                    {
                        // we turn left
                        dir = nextDir;
                        (y, x) = corner;
                    }
                    else
                    {
               //         neighbors.Add(cornerZone);
                    }
                }
                else
                {
                    // either still the same side or we may need to turn left
                    (y, x) = next;
                    var nextDir = (dir + 3) % 4;
                    next = (y + _vectors[nextDir].dy, x + _vectors[nextDir].dx);
                    if (next is not { y: >= 0, x: >= 0 } || next.y >= height || next.x >= width) continue;
                    var cornerZone = _areas[next.y,next.x];
                    if (cornerZone == currentZone)
                    {
                        // we need to turn left indeed
                        (y, x) = next;
                        dir = nextDir;
                        edge++;
                    }
                    else
                    {
//                        neighbors.Add(cornerZone);
                    }
                }
            } while (initPos!=(y, x) || dir != 0);

            areaEdges[currentZone] = areaEdges.GetValueOrDefault(currentZone)+edge;
            if (neighbors.Count != 1 || neighbors.Single() == 0) continue;
            // this area is enclosed in another one, we must declare the fences
            var enclosing = neighbors.Single();
            areaEdges[enclosing] = areaEdges.GetValueOrDefault(enclosing)+edge;
        }
        // compute

        return _areaCharacteristics.Aggregate(0L, (current, area) => current + areaEdges[area.Key] * area.Value.area);
    }

    private readonly List<string> _map = [];
    private Dictionary<int, (int area, int perimeter, char type)> _areaCharacteristics = new();
    private int[,] _areas = new int[0,0];

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line);
    }
}
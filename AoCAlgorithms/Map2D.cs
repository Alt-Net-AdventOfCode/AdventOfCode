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

namespace AoCAlgorithms;

public static class Map2D
{
    public static Map2D<char> FromBlock(IList<string> block, OutBoundHandling mode = OutBoundHandling.Throw, char defaultValue = default)
    {
        var map = new char[block[0].Length, block.Count];
        for (var y = 0; y < block.Count; y++)
        {
            var x = 0;
            foreach (var entry in block[y])
            {
                map[x++, y] = entry;
            }
        }

        return new Map2D<char>(map, mode, defaultValue);
    }
}

/// <summary>
/// Represents a 2D map with arbitrary storage type
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class Map2D<T> : Map2DBase<T>
{
    private readonly T[,] _map;

    internal Map2D(T[,] map, OutBoundHandling mode, T defaultValue): base(mode, defaultValue) => _map = map;

    public Map2D(IList<IEnumerable<T>> map, OutBoundHandling mode = OutBoundHandling.Throw, T defaultValue = default) : base(mode, defaultValue)
    {
        _map = new T[map[0].Count(), map.Count];
        for (var y = 0; y < map.Count; y++)
        {
            var x = 0;
            foreach (var entry in map[y])
            {
                _map[x++, y] = entry;
            }
        }
    }
    
    protected override bool IsInMap((int x, int y) coordinates) =>
        coordinates.x >= GetLowerBound(0)
        && coordinates.x <= GetUpperBound(0)
        && coordinates.y >= GetLowerBound(1)
        && coordinates.y <= GetUpperBound(1);

    protected override T GetValue((int x, int y) coords) => _map[coords.x, coords.y];

    protected override void SetValue((int x, int y) coords, T value) => _map[coords.x, coords.y] = value;

    public override int GetSize(int dimension) => _map.GetLength(dimension);

    public override int GetLowerBound(int dimension) => _map.GetLowerBound(dimension);

    public override int GetUpperBound(int dimension) => _map.GetUpperBound(dimension);
}
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

using System.Reflection.PortableExecutable;

namespace AoCAlgorithms;
/// <summary>
/// Represents a 2D map with arbitrary storage type
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class Map2D<T>
{
    private readonly T[,] _map;
    private readonly OutBoundHandling _mode;
    private readonly T _default;

    public Map2D(IList<IEnumerable<T>> map, OutBoundHandling mode = OutBoundHandling.Throw, T defaultValue = default)
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
        
        _mode = mode;
        _default = defaultValue;
    }
    
    public T this[int x, int y] => this[(x,y)];

    public T this[(int x, int y) coordinates]
    {
        get
        {
            // if we can't adjust location, we return the (user provided) default value
            var coordinates1 = coordinates;
            return !ValidateAndConvertCoordinates(ref coordinates1 ) ? _default : _map[coordinates1.x, coordinates1.y];
        }
        set
        {
            // if we can't adjust location, we can't set the value
            if (ValidateAndConvertCoordinates(ref coordinates))
            {
                _map[coordinates.x, coordinates.y] = value;
            }
        }
    }

    /// <summary>
    /// Enumerate direct neighbors (4 directions)
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public IEnumerable<(int x, int y)> NeighborsOf((int x, int y) coordinates)
    {
        var next = (coordinates.x - 1, coordinates.y);
        if (WrapIfNeeded(ref next))
        {
            yield return next;
        }
        next = (coordinates.x, coordinates.y-1);
        if (WrapIfNeeded(ref next))
        {
            yield return next;
        }
        next = (coordinates.x+1, coordinates.y);
        if (WrapIfNeeded(ref next))
        {
            yield return next;
        }
        next = (coordinates.x, coordinates.y+1);
        if (WrapIfNeeded(ref next))
        {
            yield return next;
        }
    }

    private bool WrapIfNeeded(ref (int x, int y) coordinates)
    {
        if (IsInMap(coordinates))
        {
            return true;
        }

        switch (_mode)
        {
            case OutBoundHandling.Throw:
                return false;
            case OutBoundHandling.WrapAround:
                coordinates.x = WrapIndexAround(coordinates.x, 0);
                coordinates.y = WrapIndexAround(coordinates.y, 1);
                return true;
            case OutBoundHandling.Repeat:
            case OutBoundHandling.DefaultValue:
            default:
                return true;
        }
    }

    /// <summary>
    /// Convert external coordinates to internal ones.
    /// Returns false if it can't do it
    /// </summary>
    /// <param name="coordinates">coordinates to check and adjust</param>
    /// <returns>true if the returned coordinates fall into the map.</returns>
    private bool ConvertToInternalCoordinates(ref (int x, int y) coordinates)
    {
        if (IsInMap(coordinates))
        {
            return true;
        }

        if (_mode != OutBoundHandling.WrapAround && _mode != OutBoundHandling.Repeat)
        {
            return false;
        }

        coordinates.x = WrapIndexAround(coordinates.x, 0);
        coordinates.y = WrapIndexAround(coordinates.y, 1);
        return true;
    }

    private bool IsInMap((int x, int y) coordinates)
    {
        return coordinates.x >= _map.GetLowerBound(0)
               && coordinates.x <= _map.GetUpperBound(0)
               && coordinates.y >= _map.GetLowerBound(1)
               && coordinates.y <= _map.GetUpperBound(1);
    }

    /// <summary>
    /// Validate coordinates and return valid internal coordinates
    /// </summary>
    /// <param name="coordinates">Coordinates to validate and correct,</param>
    /// <returns>true if coordinates fall into the map.</returns>
    /// <exception cref="IndexOutOfRangeException">when coordinates are outside the map and mode is <see cref="OutBoundHandling.Throw"/></exception>
    private bool ValidateAndConvertCoordinates(ref (int x, int y) coordinates)
    {
        var status = ConvertToInternalCoordinates(ref coordinates);
        if (status || _mode != OutBoundHandling.Throw)
        {
            return _mode == OutBoundHandling.WrapAround || status;
        }
        var xOutOfBound = coordinates.x < _map.GetLowerBound(0) || coordinates.x > _map.GetUpperBound(0);
        var yOutOfBound = coordinates.y < _map.GetLowerBound(1) || coordinates.y > _map.GetUpperBound(1);
        throw new IndexOutOfRangeException((xOutOfBound ? $"x {coordinates.x} is out of bound " : "") +
                                           (yOutOfBound ? $"y {coordinates.y} is out of bound." : ""));
    }

    private int WrapIndexAround(int x, int dimension)
    {
        while (x < _map.GetLowerBound(dimension))
        {
            x += _map.GetLength(dimension);
        }

        while (x > _map.GetUpperBound(dimension))
        {
            x -= _map.GetLength(dimension);
        }

        return x;
    }

}
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

/// <summary>
/// Represents a 2D map with arbitrary storage strategy. To be specified via inheritance
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public abstract class Map2DBase<T>
{
    protected readonly OutBoundHandling Mode;
    protected readonly T Default;

#pragma warning disable CS8601
    protected Map2DBase(OutBoundHandling mode = OutBoundHandling.Throw, T defaultValue = default)
#pragma warning restore CS8601
    {
        Mode = mode;
        Default = defaultValue;
    }
    
    public T this[int x, int y]
    {
        get => this[(x, y)];
        set => this[(x, y)] = value;
    }

    public T this[(int x, int y) coordinates]
    {
        get =>
            // if we can't adjust location, we return the (user provided) default value
            !ValidateAndConvertCoordinates(ref coordinates ) ? Default : GetValue(coordinates);
        set
        {
            // if we can't adjust location, we can't set the value
            if (ValidateAndConvertCoordinates(ref coordinates))
            {
                SetValue(coordinates, value);
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
        foreach (var (dx, dy) in Vectors)
        {
            var next = (coordinates.x + dx, coordinates.y + dy);
            if (WrapIfNeeded(ref next))
            {
                yield return next;
            } 
        }
    }
    
    private static readonly (int dx, int dy)[] Vectors = {(-1, 0), (0, -1), (1, 0), (0, 1) };
    
    /// <summary>
    /// Enumerate direct neighbors (4 directions)
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
   public IEnumerable<(int x, int y)> Neighbors8Of((int x, int y) coordinates)
    {
        foreach (var (dx, dy) in Vectors8)
        {
            var next = (coordinates.x + dx, coordinates.y + dy);
            if (WrapIfNeeded(ref next))
            {
                yield return next;
            } 
        }
    }
    

    private static readonly (int dx, int dy)[] Vectors8 = {(-1, 0), (-1, -1), (0, -1), (1, -1), (1, 0), ( 1, 1), (0, 1), (-1, 1) };
    
    /// <summary>
    /// Enumerate direct neighbors (8 directions)
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns>an enumeration of the coordinates around the given cell (if they exists)</returns>
    /// <remarks>Order is : left, top left, top, top right, right, bottom right, bottom, bottom left</remarks>
    public IEnumerable<(int x, int y)> Around((int x, int y) coordinates)
    {
        var result = new List<(int x, int y)>(8);
        var next = (coordinates.x - 1, coordinates.y);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x - 1, coordinates.y-1);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x, coordinates.y-1);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x+1, coordinates.y-1);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x+1, coordinates.y);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x+1, coordinates.y+1);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x, coordinates.y+1);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }
        next = (coordinates.x-1, coordinates.y+1);
        if (WrapIfNeeded(ref next))
        {
            result.Add(next);
        }

        return result;
    }

    private bool WrapIfNeeded(ref (int x, int y) coordinates)
    {
        if (IsInMap(coordinates))
        {
            return true;
        }

        switch (Mode)
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
    protected virtual bool ConvertToInternalCoordinates(ref (int x, int y) coordinates)
    {
        if (IsInMap(coordinates))
        {
            return true;
        }

        if (Mode != OutBoundHandling.WrapAround && Mode != OutBoundHandling.Repeat)
        {
            return false;
        }

        coordinates.x = WrapIndexAround(coordinates.x, 0);
        coordinates.y = WrapIndexAround(coordinates.y, 1);
        return true;
    }

    protected abstract bool IsInMap((int x, int y) coordinates);

    /// <summary>
    /// Validate coordinates and return valid internal coordinates
    /// </summary>
    /// <param name="coordinates">Coordinates to validate and correct,</param>
    /// <returns>true if coordinates fall into the map.</returns>
    /// <exception cref="IndexOutOfRangeException">when coordinates are outside the map and mode is <see cref="OutBoundHandling.Throw"/></exception>
    protected bool ValidateAndConvertCoordinates(ref (int x, int y) coordinates)
    {
        var status = ConvertToInternalCoordinates(ref coordinates);
        if (status || Mode != OutBoundHandling.Throw)
        {
            return Mode == OutBoundHandling.WrapAround || status;
        }
        throw new IndexOutOfRangeException($"Coordinates ({coordinates.x},{coordinates.y}) are out of bounds.");
    }
    
    private int WrapIndexAround(int x, int dimension)
    {
        while (x < GetLowerBound(dimension))
        {
            x += GetSize(dimension);
        }

        while (x > GetUpperBound(dimension))
        {
            x -= GetSize(dimension);
        }

        return x;
    }
    
    protected abstract T GetValue((int x, int y) coords);
    protected abstract void SetValue((int x, int y) coords, T value);
    
    public abstract int GetSize(int dimension);

    public virtual int GetLowerBound(int dimension) => 0;

    public virtual int GetUpperBound(int dimension) => GetSize(dimension)-1;
}
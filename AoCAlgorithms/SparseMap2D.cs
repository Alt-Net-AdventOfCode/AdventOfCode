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

using System.Collections;

namespace AoCAlgorithms;

public class SparseMap2D<T>: Map2DBase<T>, IEnumerable<(int x, int y)>
{
    private readonly Dictionary<(int x, int y), T> _map = new();
    private readonly int _width;
    private readonly int _height;

    private int _minX = int.MaxValue;
    private int _maxX = int.MinValue;
    private int _minY = int.MaxValue;
    private int _maxY = int.MinValue;
    private bool _limitsAreDirty;

    public SparseMap2D(T defaultValue, OutBoundHandling mode = OutBoundHandling.DefaultValue,  int width = int.MaxValue, int height = int.MaxValue): base(mode, defaultValue)
    {   
        _width = width;
        _height = height;
    }

    public SparseMap2D(SparseMap2D<T> other) : base(other.Mode, other.Default)
    {
        _width = other._width;
        _height = other._height;
        _minX = other._minX;
        _maxX = other._maxX;
        _minY = other._minY;
        _maxY = other._maxY;
        _map = new Dictionary<(int x, int y), T>(other._map);
    }

    protected override bool IsInMap((int x, int y) coordinates) => _map.ContainsKey(coordinates);
    protected override bool ConvertToInternalCoordinates(ref (int x, int y) coordinates)
    {
        return true;
    }

    public bool Exists((int x, int y) coordinates) => IsInMap(coordinates);
    
    protected override T GetValue((int x, int y) coords) => !_map.ContainsKey(coords) ? Default : _map[coords];

    protected override void SetValue((int x, int y) coords, T value)
    {
        _map[coords] = value;
        if (_limitsAreDirty)
        {
            return;
        }
        _minX = Math.Min(_minX, coords.x);
        _maxX = Math.Max(_maxX, coords.x);
        _minY = Math.Min(_minY, coords.y);
        _maxY = Math.Max(_maxY, coords.y);
    }

    public override int GetSize(int dimension) => dimension == 0 ? _width : _height;

    public override int GetLowerBound(int dimension)
    {
        CheckLimits();
        return dimension == 0 ? _minX : _minY;
    }

    public override int GetUpperBound(int dimension)
    {
        CheckLimits();
        return dimension == 0 ? _maxX : _maxY;
    }

    public long GetBoundedSurface()
    {
        CheckLimits();
        return ((long)(_maxX - _minX+1) * (_maxY - _minY + 1));
    }

    public long GetEntryCount() => _map.Count;

    private void CheckLimits()
    {
        if (!_limitsAreDirty)
        {
            return;
        }
        _minX = int.MaxValue;
        _maxX = int.MinValue;
        _minY = int.MaxValue;
        _maxY = int.MinValue;
        foreach (var coords in _map.Keys)
        {
            _minX = Math.Min(_minX, coords.x);
            _maxX = Math.Max(_maxX, coords.x);
            _minY = Math.Min(_minY, coords.y);
            _maxY = Math.Max(_maxY, coords.y);
        }

        _limitsAreDirty = false;
    }
    
    public IEnumerator<(int x, int y)> GetEnumerator() => _map.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool RemoveAt((int x, int y) from)
    {
        if (_map.Remove(from))
        {
            if (from.x == _minX || from.x == _maxX || from.y == _minY || from.y == _maxY)
            {
                _limitsAreDirty = true;
            }

            return true;
        }

        return false;
    }
}
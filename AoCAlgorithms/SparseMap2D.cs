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

public class SparseMap2D<T>: Map2DBase<T>
{
    private readonly Dictionary<(int x, int y), T> _map = new();
    private readonly int _width;
    private readonly int _height;

    private int _minX = int.MaxValue;
    private int _maxX = int.MinValue;
    private int _minY = int.MaxValue;
    private int _maxY = int.MinValue;

    public SparseMap2D(T defaultValue, OutBoundHandling mode = OutBoundHandling.DefaultValue,  int width = int.MaxValue, int height = int.MaxValue): base(mode, defaultValue)
    {   
        _width = width;
        _height = height;
    }
    
    protected override bool IsInMap((int x, int y) coordinates) => _map.ContainsKey(coordinates);

    protected override T GetValue((int x, int y) coords) => !_map.ContainsKey(coords) ? Default : _map[coords];

    protected override void SetValue((int x, int y) coords, T value)
    {
        _minX = Math.Min(_minX, coords.x);
        _maxX = Math.Max(_maxX, coords.x);
        _minY = Math.Min(_minY, coords.y);
        _maxY = Math.Min(_maxY, coords.y);
        _map[coords] = value;
    }

    public override int GetSize(int dimension)
    {
        return dimension == 0 ? _width : _height;
    }

    public override int GetLowerBound(int dimension) => dimension == 0 ? _minX : _minY;

    public override int GetUpperBound(int dimension) => dimension == 0 ? _maxX : _maxY;
}
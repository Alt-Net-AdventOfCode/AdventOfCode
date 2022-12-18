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

using AoCAlgorithms;
using NFluent;

namespace AocAlgorithmsTests;

[TestFixture]
public class SparseMap2DShould
{
    [Test]
    [TestCase(0, 0, 0)]
    [TestCase(5, 0, 5)]
    [TestCase(0, 1, 10)]
    [TestCase(9, 1, 19)]
    public void SupportIndexedAccess(int testX, int testY, int expected)
    {
        var map = new SparseMap2D<int>(42);
        var counter = 0;
        for (var y = 0; y < 2; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                map[x, y] = counter++;
            }
        }
        Check.That(map[testX, testY]).IsEqualTo(expected);
    }
}
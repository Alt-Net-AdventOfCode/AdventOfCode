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

public static class MathHelper
{
    
    /// <summary>
    /// Returns the result of the integer division, rounded up
    /// </summary>
    /// <param name="x">number</param>
    /// <param name="y">divisor</param>
    /// <returns>Returns the result of the integer division, rounded up</returns>
    public static int RoundedUpDivision(int x, int y) => x/y + (x%y!=0 ? 1 : 0);
    /// <summary>
    /// Returns the result of the integer division, rounded up
    /// </summary>
    /// <param name="x">number</param>
    /// <param name="y">divisor</param>
    /// <returns>Returns the result of the integer division, rounded up</returns>
    public static long RoundedUpDivision(long x, long y) => x/y + (x%y!=0 ? 1 : 0);

    /// <summary>
    /// Compute the greatest common divisor of two numbers
    /// </summary>
    /// <param name="x">number</param>
    /// <param name="y">number</param>
    /// <returns>Returns the GCD</returns>
    public static long Gcd(long x, long y)
    {
        if (x * y == 0)
            return 1;
        while (true)
        {
            if (x < y)
            {
                (x, y) = (y, x);
            }

            var remainder = x % y;
            if (remainder == 0)
            {
                return y;
            }
            x = y;
            y = remainder;
        }
    }

    /// <summary>
    /// Compute the lowest common multiple of two numbers
    /// </summary>
    /// <param name="x">number</param>
    /// <param name="y">number</param>
    /// <returns>Returns the LCM</returns>
    public static long Lcm(long x, long y) => x * y / Gcd(x, y);
}
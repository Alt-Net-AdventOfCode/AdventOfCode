// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
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

using System;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(20)]
public class DupdobDay20: SolverWithParser
{
    private int _target;
    protected override void Parse(string data)
    {
        _target = int.Parse(data);
    }

    [Example("150", 8)]
    public override object GetAnswer1()
    {
        var operatingTarget = _target / 10;
        // each house receives the total of its divisors (times ten) as gifts
        var fistHouseOfInterest = (int)Math.Sqrt(operatingTarget);
        var lastHouseOfInterest = operatingTarget-1;
        var houses = new int[lastHouseOfInterest - fistHouseOfInterest +1];
        for(var elf=1; elf < lastHouseOfInterest; elf++)
        {
            for(var house = (fistHouseOfInterest + elf - 1) / elf * elf; house <= lastHouseOfInterest; house += elf)
            {
                houses[house - fistHouseOfInterest] += elf;
            }
        }

        for (int house = fistHouseOfInterest; house < lastHouseOfInterest; house++)
        {
            if (houses[house - fistHouseOfInterest] >= operatingTarget)
            {
                return house;
            }
        }
        return -1;
    }

    public override object GetAnswer2()
    {
        var operatingTarget = _target / 11;
        // each house receives the total of its divisors (times ten) as gifts
        var fistHouseOfInterest = (int)Math.Ceiling(Math.Sqrt(operatingTarget));
        var lastHouseOfInterest = operatingTarget-1;
        var houses = new int[lastHouseOfInterest - fistHouseOfInterest +1];
        for(var elf=1; elf < lastHouseOfInterest; elf++)
        {
            var firstHouse = (fistHouseOfInterest + elf - 1) / elf * elf;
            var housesToVisit = Math.Min(50, (lastHouseOfInterest - firstHouse) / elf+1);
            for(var i = 0; i < housesToVisit; i++)
            {
                houses[firstHouse-fistHouseOfInterest+i*elf] += elf*11;
            }
        }

        for (int house = fistHouseOfInterest; house < lastHouseOfInterest; house++)
        {
            if (houses[house - fistHouseOfInterest] >= _target)
            {
                return house;
            }
        }
        return -1;
    }
}
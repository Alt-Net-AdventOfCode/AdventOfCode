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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay17 : SolverWithParser
{
    private int _seed;

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 17;
        automatonBase.AddExample("3").Answer1(638);
    }

    protected override void Parse(string data)
    {
        _seed = int.Parse(data);
    }

    public override object GetAnswer1()
    {
        var buffer = new List<short>(2018);
        var position = -1;
        for (short i = 0; i <= 2017; i++, position = (position+1+_seed) % buffer.Count)
        {
            buffer.Insert(position+1, i);
        }

        return buffer[buffer.IndexOf(2017)+1];
    }

    public override object GetAnswer2()
    {
        // the approach can be very different: we just need to track the number after 0
        // i.e the one at pos 1
        var target = 50000000;
        var position = -1;
        var pos1 = 0;
        for (var i = 0; i < target; i++, position = (position+1+_seed) % i)
        {
            if (position == 0)
            {
                pos1 = i;
            }
        }

        return pos1;
    }
}
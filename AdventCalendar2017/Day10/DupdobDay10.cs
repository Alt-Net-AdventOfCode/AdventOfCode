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

using System.Text;
using AoC;

namespace AdventCalendar2017;

public class DupdobDay10 : SolverWithDataAsLines
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 10;
        dayAutomatonBase.RegisterTestDataAndResult("3, 4, 1, 5", 12, 1);
        dayAutomatonBase.RegisterTestDataAndResult("", "a2582a3a0e66e6e86e3812dcb672a272", 2);
        dayAutomatonBase.RegisterTestDataAndResult("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd", 2);
    }

    public override object GetAnswer1()
    {
        var skip = 0;
        var current = 0;
        var count = 256;
        if (_lengths.Count == 4)
        {
            count = 5;
        }
        var list = Enumerable.Range(0, count).ToList();
        list = HashRound(_lengths, list, ref current, ref skip);

        return list[0] * list[1];
    }

    private static List<int> HashRound(List<int> lengths, List<int> list, ref int current, ref int skip)
    {
        foreach (var length in lengths)
        {
            var next = list.ToList();
            var end = current + length - 1;
            for (var i = 0; i < length; i++)
            {
                next[current++ % list.Count] = list[end-- % list.Count];
            }

            current += skip;
            skip++;
            list = next;
        }

        return list;
    }

    public override object GetAnswer2()
    {
        var skip = 0;
        var current = 0;
        var list = Enumerable.Range(0, 256).ToList();
        var lengths = _raw.ToList();
        lengths.AddRange([17, 31, 73, 47, 23]);
        for (var i = 0; i < 64; i++)
        {
            list = HashRound(lengths, list, ref current, ref skip);
        }
        // now compute the last result
        var hash = new StringBuilder(16);
        for (var i = 0; i < 16; i++)
        {
            var local = 0;
            for (var j = i * 16; j < (i + 1) * 16; j++)
            {
                local ^= list[j];
            }

            hash.AppendFormat("{0:X2}", local);
        }

        return hash.ToString().ToLower();
    }

    private List<int> _lengths = null!;
    private List<int> _raw = null!;

    protected override void ParseLines(string[] lines)
    {
        try
        {
            _lengths = lines[0].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _lengths = [];
        }

        if (lines.Length==0)
        {
            _raw = [];
        }
        else
        {
            _raw = lines[0].Select(c => (int) c).ToList();
        }
    }
}
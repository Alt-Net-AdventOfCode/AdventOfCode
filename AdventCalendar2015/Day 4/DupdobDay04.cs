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

using System.Security.Cryptography;
using System.Text;
using AoC;

namespace AdventCalendar2015;

[Day(4)]
public class DupdobDay04 : SolverWithParser
{
    private string _prefix;
    private int _fiveZeros;

    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    protected override void Parse(string data)
    {
        _prefix = data;
    }

    [Example("abcdef", 609043)]
    [Example("pqrstuv", 1048970)]
    public override object GetAnswer1()
    {
        for (var i = _fiveZeros; i < int.MaxValue; i++)
        {
            var hash = MD5.HashData(Encoding.ASCII.GetBytes($"{_prefix}{i}"));
            if (hash[0] == 0 && hash[1] == 0 && hash[2] < 16)
            {
                _fiveZeros = i;
                return i;
            }
        }

        return null;
    }

    public override object GetAnswer2()
    {
        for (var i = 1; i < int.MaxValue; i++)
        {
            var hash = MD5.HashData(Encoding.ASCII.GetBytes($"{_prefix}{i}"));
            if (hash[0] == 0 && hash[1] == 0 && hash[2] == 0)
            {
                return i;
            }
        }

        return null;
    }
}
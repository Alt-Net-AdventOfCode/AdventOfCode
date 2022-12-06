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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay6 : SolverWithLineParser
{
    private string _message = string.Empty;

    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 6;
        automaton.RegisterTestDataAndResult("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7,1);
        automaton.RegisterTestDataAndResult("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19,2);
    }

    public override object GetAnswer1()
    {
        return FindPacketSize(4);
    }

    private object FindPacketSize(int len)
    {
        var lastHits = new Dictionary<char, int>();
        var begin = 0;
        for (var i = 0; i < _message.Length; i++)
        {
            if (i - begin > len)
            {
                return i ;
            }

            var seen = lastHits.TryGetValue(_message[i], out var lastHit);
            lastHits[_message[i]] = i;
            if (seen && i - lastHit < len && begin < lastHit)
            {
                // we have a duplicate, we can skip it
                begin = lastHit ;
            }
        }

        return -1;
    }

    public override object GetAnswer2()
    {
        return FindPacketSize(14);
    }

    protected override void ParseLine(string? line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        _message = line;
    }
}
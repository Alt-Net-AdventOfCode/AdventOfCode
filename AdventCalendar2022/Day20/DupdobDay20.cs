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

using AoC;

namespace AdventCalendar2022;

public class DupdobDay20: SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 20;
        automaton.RegisterTestData(@"1
2
-3
3
-2
0
4");        automaton.RegisterTestData(@"1
2
-3
3
-2
0
4",2);
        automaton.RegisterTestResult(3L);
        automaton.RegisterTestResult(1623178306L,2);
    }

    public override object GetAnswer1()
    {
        var indexes = Encrypt(_numbers);
        var offset = indexes.IndexOf(_numbers.IndexOf(0));
        return LoopUp(indexes, offset + 1000)+LoopUp(indexes, offset+2000)+LoopUp(indexes,offset+3000);
    }

    private List<int> Encrypt(List<long> numbers, int repeat=1)
    {
        var indexes = Enumerable.Range(0, _numbers.Count).ToList();
        for(var j= 0;j<repeat; j++)
        {
            for (var i = 0; i < numbers.Count; i++)
            {
                var originalPos = indexes.IndexOf(i);
                // we move it
                var index = originalPos + numbers[i];
                if (index <= 0)
                {
                    index %= numbers.Count - 1;
                    if (index <= 0)
                    {
                        index+=numbers.Count - 1;
                    }
                }

                if (index >= numbers.Count)
                {
                    index %= numbers.Count - 1;
                }

                indexes.RemoveAt((int)originalPos);
                indexes.Insert((int)index, i);
            }
        }
        return indexes;
    }

    private long LoopUp(IReadOnlyList<int> indexes, int i)
    {
        i = i % _numbers.Count;
        return _numbers[indexes[i]];
    }

    private void DumpList(IReadOnlyList<int> indexes)
    {
        foreach (var index in indexes)
        {
            Console.Write(_numbers[index]);
            Console.Write(",");
        }
        Console.WriteLine();
    }
    public override object GetAnswer2()
    {
        const int key = 811589153;
        var number = _numbers.Select(t => t * key).ToList();
        var indexes = Encrypt(number, 10);
        var offset = indexes.IndexOf(_numbers.IndexOf(0));
        return (LoopUp(indexes, offset + 1000)+LoopUp(indexes, offset+2000)+LoopUp(indexes,offset+3000))*key;
    }

    private readonly List<long> _numbers = new();
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            _numbers.Add(int.Parse(line));
        }
    }
}
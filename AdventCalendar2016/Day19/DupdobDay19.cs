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

 using System.Collections.Generic;
 using System.Linq;
using AoC;

namespace AdventCalendar2016;

public class DupdobDay19 : SolverWithLineParser
{
    private int _elvesCount;

    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 19;
        automaton.AddExample("5");
        automaton.RegisterTestResult(3);
        automaton.RegisterTestResult(2,2);
    }

    public override object GetAnswer1()
    {
        var currentCountOfElves = _elvesCount;
        var winningPos = 0;
        var rankShift = 2;
        while (currentCountOfElves>1)
        {
            if ((currentCountOfElves & 1) == 1)
            {
                winningPos += rankShift;
            }
            currentCountOfElves /= 2;
            rankShift *= 2;
        }

        return winningPos+1;
    }

    public override object GetAnswer2()
    {
        var elves = new LinkedList<int>();
        var currentCount = _elvesCount;
        for (var i = 0; i < currentCount; i++)
        {
            elves.AddLast(i + 1);
        }

        var targetElf = elves.First;
        for (var i = 0; i < currentCount/2; i++)
        {
            targetElf = targetElf!.Next;
        }
        // now the game can start
        while (currentCount>1)
        {
            var elfToRemove = targetElf;
            targetElf = Next(elfToRemove);
            elves.Remove(elfToRemove);
            if ((currentCount & 1) == 1)
            {
                targetElf = Next(targetElf);
            }
            currentCount--;
        }
        return elves.First!.Value;
    }

    private static LinkedListNode<int> Next(LinkedListNode<int> current) => current.Next ?? current.List!.First;

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        _elvesCount = int.Parse(line);
    }
}
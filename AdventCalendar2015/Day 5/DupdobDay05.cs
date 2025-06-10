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

using System.Collections.Generic;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(5)]
public class DupdobDay05 : SolverWithDataAsLines
{
    private List<string> _lines;

    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    [Example("ugknbfddgicrmopn", 1)]
    [Example("aaa", 1)]
    [Example("jchzalrnumimnmhp", 0)]
    [Example("haegwjzuvuyypxyu", 0)]
    [Example("dvszwmarrgswjxmb", 0)]
    public override object GetAnswer1()
    {
        return _lines.Count(IsNice);
    }

    private static bool IsNice(string toCheck)
    {
        var vowels = 0;
        var doubledLetter = ' ';
        var lastLetter = ' ';
        foreach (var letter in toCheck)
        {
            if (letter == lastLetter)
            {
                doubledLetter = lastLetter;
            }
            else
            {
                lastLetter = letter;
            }

            if ("aeiou".Contains(letter))
            {
                vowels++;
            }
        }

        return vowels >= 3 && doubledLetter != ' '
                           && !toCheck.Contains("ab")
                           && !toCheck.Contains("cd")
                           && !toCheck.Contains("pq")
                           && !toCheck.Contains("xy");
    }

    [Example("qjhvhtzxzqqjkmpb", 1)]
    [Example("xxyxx", 1)]
    [Example("uurcxstgmygtbstg", 0)]
    [Example("ieodomkazucvgmuy", 0)]
    public override object GetAnswer2()
    {
        return _lines.Count(IsRealNice);
    }

    private static bool IsRealNice(string toCheck)
    {
        var doublePair = false;
        var mirroredLetters = false;
        for (var i = 2; i < toCheck.Length; i++)
        {
            if (!doublePair && toCheck[i..].Contains(toCheck.Substring(i - 2, 2)))
            {
                doublePair = true;
            }
            if (toCheck[i - 2] == toCheck[i])
            {
                mirroredLetters = true;
            }
        }
        return doublePair && mirroredLetters;
    }

    protected override void ParseLines(string[] lines)
    {
        _lines = lines.ToList();
    }
}
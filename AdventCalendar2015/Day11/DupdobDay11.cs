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
using AoC;

namespace AdventCalendar2015;

[Day(11)]
public class DupdobDay11: SolverWithParser
{
    private string _password;

    public override void SetupRun(Automaton automaton)
    {
    }

    protected override void Parse(string data)
    {
        _password = data;
    }

    [UnitTest("ya", "xz")]
    [UnitTest("byaa", "bxzz")]
    private static string IncrementPassword(string password)
    {
        var index = password.Length - 1;
        
        while (index >= 0 && password[index] == 'z')
        {
            index--;
        }

        if (index == -1)
        {
            return new string('a', password.Length);
        }

        var c = (char)(password[index]+1);
        if (index == 0)
        {
            return c+new string('a', password.Length-1);
        }

        return password[..index]+c+new string('a', password.Length-index-1);
    }

    [UnitTest(false, "abcdefgh")]
    [UnitTest(true, "abcdffaa")]
    private static bool IsValidPassword(string password)
    {
        var hasStraight = false;
        var currentStraight = 0;
        var pairs = 0;
        for (var i = 1; i < password.Length; i++)
        {
            var current = password[i];
            if (current is 'i' or 'i' or 'l')
            {
                return false;
            }
            var lastChar = password[i - 1];
            if (lastChar == current)
            {
                if (i > 1 && current == password[i - 2])
                {
                    // do not count overlapping pairs
                    continue;
                }

                pairs++;
                currentStraight = 0;
            }
            else if (current == lastChar+1)
            {
                currentStraight++;
                if (currentStraight == 2)
                {
                    hasStraight = true;
                }
            }
            else
            {
                currentStraight = 0;
            }
        }

        return pairs == 2 && hasStraight;
    }
    
    [Example("abcdefgh", "abcdffaa")]
    [Example("ghijklmn", "ghjaabcc")]
    public override object GetAnswer1()
    {
        var password = _password;
        do
        {
            password = IncrementPassword(password);
        } while (!IsValidPassword(password));

        _password = password;
        return password;
    }

    public override object GetAnswer2() => GetAnswer1();
}
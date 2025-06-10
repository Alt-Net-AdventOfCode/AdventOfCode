// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
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

using System.Runtime.CompilerServices;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay25 : SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 25;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                #####
                                                .####
                                                .####
                                                .####
                                                .#.#.
                                                .#...
                                                .....

                                                #####
                                                ##.##
                                                .#.##
                                                ...##
                                                ...#.
                                                ...#.
                                                .....

                                                .....
                                                #....
                                                #....
                                                #...#
                                                #.#.#
                                                #.###
                                                #####

                                                .....
                                                .....
                                                #.#..
                                                ###..
                                                ###.#
                                                ###.#
                                                #####

                                                .....
                                                .....
                                                .....
                                                #....
                                                #.#..
                                                #.#.#
                                                #####
                                                """, 3, 1);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        foreach (var lck in _locks)
        {
            foreach (var pins in _keys)
            {
                var isMatch = true;
                for (var i = 0; i < 5; i++)
                {
                    if (lck[i] + pins[i] <= 5) continue;
                    isMatch = false;
                    break;
                }

                if (isMatch)
                {
                    result++;
                }
            }
        }

        return result;
    }

    public override object GetAnswer2()
    {
        throw new NotImplementedException();
    }

    protected override void Parse(string data)
    {
        var lines = data.Split('\n');
        // we discard the last line if it is empty (trailing newline), but we keep any intermediate newlines
        if (lines[^1].Length == 0) lines = lines[..^1];
        var block = new List<string>();
        var index = 0;
        foreach (var line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                block.Add(line);
            }
            else
            {
                ParseBlock(block, index++);
                block = [];
            }
        }
        ParseBlock(block, index);
    }

    private readonly List<int[]> _locks = [];
    private readonly List<int[]> _keys = [];

    private void ParseBlock(List<string> block, int index)
    {
        if (block[0] == "#####")
        {
            // this is a lock
            var pins = new int[5];
            for (var x = 0; x < 5; x++)
            {
                for (var y = 1; y < 7; y++)
                {
                    if (block[y][x] == '#')
                    {
                        pins[x]++;
                    }
                }
            }
            _locks.Add(pins);
        }
        else
        {
            // this is a key
            var pins = new int[5];
            for (var x = 0; x < 5; x++)
            {
                for (var y = 5; y >= 0; y--)
                {
                    if (block[y][x] == '#')
                    {
                        pins[x]++;
                    }
                }
            }
            _keys.Add(pins);
        }
    }
}
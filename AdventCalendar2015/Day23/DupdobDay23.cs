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
using System.Collections.Generic;
using AoC;

namespace AdventCalendar2015;

[Day(23)]
public class DupdobDay23 : SolverWithParser
{
    private record CpuState(int a, int b, int Ip);
    
    private readonly List<Func<CpuState, CpuState>> _program = new();
    protected override void Parse(string data)
    {
        foreach (var line in data.SplitLines())
        {
            var block = line.Split(' ', StringSplitOptions.TrimEntries| StringSplitOptions.TrimEntries);
            switch (block[0])
            {
                case "hlf":
                    if (block[1] == "a")
                    {
                        _program.Add( state => state with{ a = state.a / 2, Ip = state.Ip + 1 });
                    }
                    else
                    {
                        _program.Add( state => state with{ b = state.b / 2, Ip = state.Ip + 1 });
                    }
                    break;
                case "tpl":
                    if (block[1] == "a")
                    {
                        _program.Add( state => state with{ a = state.a * 3, Ip = state.Ip + 1 });
                    }
                    else
                    {
                        _program.Add( state => state with{ b = state.b * 3, Ip = state.Ip + 1 });
                    }
                    break;
                case "inc":
                    if (block[1] == "a")
                    {
                        _program.Add( state => state with{ a = state.a+1, Ip = state.Ip + 1 });
                    }
                    else
                    {
                        _program.Add( state => state with{ b = state.b+1, Ip = state.Ip + 1 });
                    }
                    break;
                case "jmp":
                    _program.Add(state => state with { Ip = state.Ip + int.Parse(block[1]) });
                    break;
                case "jie":
                    if (block[1] == "a,")
                    {
                        _program.Add(state => state with { Ip = (state.a % 2 == 0) ? state.Ip + int.Parse(block[2]) : state.Ip + 1 });
                    }
                    else
                    {
                        _program.Add(state => state with { Ip = (state.b % 2 == 0) ? state.Ip + int.Parse(block[2]) : state.Ip + 1 });
                    }
                    break;
                case "jio":
                    if (block[1] == "a,")
                    {
                        _program.Add(state => state with
                        {
                            Ip = (state.a == 1) ? state.Ip + int.Parse(block[2]) : state.Ip + 1
                        });
                    }
                    else
                    {
                        _program.Add(state => state with
                        {
                            Ip = (state.b == 1) ? state.Ip + int.Parse(block[2]) : state.Ip + 1
                        });
                    }

                    break;
                default:
                    throw new ArgumentException($"Unknown instruction: {line}");
            }
        }
    }

    private CpuState RunProgram(CpuState state)
    {
        while (state.Ip < _program.Count)
        {
            state = _program[state.Ip](state);
        }

        return state;
    }
    public override object GetAnswer1() => RunProgram(new CpuState(0, 0,0)).b;
    
    public override object GetAnswer2() => RunProgram(new CpuState(1, 0,0)).b;


    
}
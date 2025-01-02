// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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

namespace AdventCalendar2016;

public class DupdobDay23 : SolverWithLineParser
{
    private readonly int[] _registers = new int[4];
    private int _pc;

    private readonly List<Action> _program = [];
    private readonly Dictionary<int, bool> _toggled = [];

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 23;
        automatonBase.RegisterTestDataAndResult("""
                                                cpy 2 a
                                                tgl a
                                                tgl a
                                                tgl a
                                                cpy 1 a
                                                dec a
                                                dec a
                                                """, 3, 1);
    }

    public override object GetAnswer1()
    {
        _registers[0] = 7;
        for (_pc = 0; _pc < _program.Count; _pc++)
        {
            _program[_pc]();
        }
        return _registers[NameToIndex("a")];
    }

    public override object GetAnswer2()
    {
        _registers[0] = 12;
        _registers[1] = 0;
        _toggled.Clear();
        for (_pc = 0; _pc < _program.Count; _pc++)
        {
            _program[_pc]();
        }
        return _registers[NameToIndex("a")];
    }

    private bool IsToggled() => _toggled.GetValueOrDefault(_pc);
     
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var tokens = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        switch (tokens[0])
        {
            case "cpy":
                _program.Add(() =>
                {
                    if (IsToggled()) 
                        JumpIfNotZero(tokens);
                    else 
                        Copy(tokens);
                });
                break;
            case "inc":
                _program.Add(() =>
                {
                    if (IsToggled()) 
                        Dec(tokens);
                    else
                        Inc(tokens);
                });
                break;
            case "dec":
                _program.Add(() =>
                {
                    if (IsToggled())
                        Inc(tokens);
                    else
                        Dec(tokens);
                });
                break;
            case "tgl":
                _program.Add(() =>
                {
                    if (IsToggled())
                        Inc(tokens);
                    else
                        Toggle(tokens);
                });
                break;
            case "jnz":
                _program.Add(() =>
                {
                    if (IsToggled())
                        Copy(tokens);
                    else
                        JumpIfNotZero(tokens);
                });
                break;
        }
    }

    private void JumpIfNotZero(string[] tokens)
    {
        if (TokenToValue(tokens[1]) != 0)
        {
            _pc += TokenToValue(tokens[2]) - 1;
        }
    }

    private void Toggle(string[] tokens) => _toggled[TokenToValue(tokens[1])+_pc] = true;

    private void Dec(string[] tokens) => _registers[NameToIndex(tokens[1])]--;

    private void Inc(string[] tokens) => _registers[NameToIndex(tokens[1])]++;

    private void Copy(string[] tokens) => _registers[NameToIndex(tokens[2])] = TokenToValue(tokens[1]);

    private static int NameToIndex(string register) => register[0] - 'a';

    private int TokenToValue(string token)
    {
        return int.TryParse(token, out var value) ? value : _registers[NameToIndex(token)];
    }
    
}
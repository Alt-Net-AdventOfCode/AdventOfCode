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

namespace AdventCalendar2016;

public class DupdobDay25 : SolverWithLineParser
{
    private readonly int[] _registers = new int[4];
    private int _pc;

    private readonly List<Action> _program = [];
    private readonly List<int> _output = [];

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 25;
    }

    public override object GetAnswer1()
    {
        // = 158
        int a;
        for (a = 0; a < 100000; a++)
        {
            _registers[0] = a;
            for (_pc = 0; _pc < _program.Count && _output.Count<16; _pc++)
            {
                _program[_pc]();
            }

            if (ValidateSignal())
            {
                break;
            }
            _output.Clear();
        }

        return a;
    }

    private bool ValidateSignal()
    {
        var isValid = true;
        for (var i = 1; i < _output.Count; i++)
        {
            if (_output[i - 1] * _output[i] == 0 && _output[i - 1] + _output[i] == 1)
            {
                continue;
            }
            isValid = false;
            break;
        }
        return isValid;
    }

    public override object GetAnswer2()
    {
        return null;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var tokens = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        switch (tokens[0])
        {
            case "cpy":
                _program.Add(() => Copy(tokens));
                break;
            case "inc":
                _program.Add(() => Inc(tokens));
                break;
            case "dec":
                _program.Add(() => Dec(tokens));
                break;
            case "jnz":
                _program.Add(() => JumpIfNotZero(tokens));
                break;
            case "out":
                _program.Add(()=>Output(tokens));
                break;
        }
    }

    private void Output(string[] tokens) => _output.Add(TokenToValue(tokens[1]));

    private void JumpIfNotZero(string[] tokens)
    {
        if (TokenToValue(tokens[1]) != 0)
        {
            _pc += TokenToValue(tokens[2]) - 1;
        }
    }

    private void Dec(string[] tokens) => _registers[NameToIndex(tokens[1])]--;

    private void Inc(string[] tokens) => _registers[NameToIndex(tokens[1])]++;

    private void Copy(string[] tokens) => _registers[NameToIndex(tokens[2])] = TokenToValue(tokens[1]);

    private static int NameToIndex(string register) => register[0] - 'a';

    private int TokenToValue(string token) => int.TryParse(token, out var value) ? value : _registers[NameToIndex(token)];
}

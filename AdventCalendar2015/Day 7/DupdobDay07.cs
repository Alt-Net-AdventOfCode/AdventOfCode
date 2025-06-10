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

[Day(7)]
public class DupdobDay07: SolverWithLineParser
{
    public override void SetupRun(DayAutomaton _)
    {
    }

    [Example("""
             123 -> x
             456 -> y
             x AND y -> d
             x OR y -> e
             x LSHIFT 2 -> f
             y RSHIFT 2 -> g
             NOT x -> h
             NOT y -> i
             g -> a
             """, 114)]
    public override object GetAnswer1()
    {
        return Evaluate("a");
    }

    public override object GetAnswer2()
    {
        var lastA = _cache["a"];
        _cache.Clear();
        _cache["b"] = lastA;

        return Evaluate("a");
    }

    private readonly Dictionary<string, Func<ushort>> _wires = [];
    private readonly Dictionary<string, ushort> _cache = [];

    private ushort Evaluate(string registerOrValue)
    {
        if (_cache.TryGetValue(registerOrValue, out var value))
        {
            return value;
        }

        _cache[registerOrValue] = value = _wires.TryGetValue(registerOrValue, out var compute) ? compute() : ushort.Parse(registerOrValue);
        return value;
    }
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        var blocks = line.Split("->", StringSplitOptions.TrimEntries);
        var target = blocks[1];
        Func<ushort> wireLogic;
        var expression = blocks[0];
        if (!expression.Contains(' '))
        {
            wireLogic = () => Evaluate(expression);
        }
        else if (expression.StartsWith("NOT"))
        {
            var operand = expression.Split(' ')[1];
            wireLogic = () => (ushort)~Evaluate(operand);
        }
        else
        {
            blocks = expression.Split(' ', StringSplitOptions.TrimEntries);
            var a = blocks[0];
            var b = blocks[2];
            wireLogic = blocks[1] switch
            {
                "AND" => () => (ushort)(Evaluate(a) & Evaluate(b)),
                "OR" => () => (ushort)(Evaluate(a) | Evaluate(b)),
                "LSHIFT" => () => (ushort)(Evaluate(a) << Evaluate(b)),
                "RSHIFT" => () => (ushort)(Evaluate(a) >> Evaluate(b)),
                _ => null
            };
        }

        _wires[target] = wireLogic;
    }
}
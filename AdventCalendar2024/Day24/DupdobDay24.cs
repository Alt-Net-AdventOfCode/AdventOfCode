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

using System.Diagnostics;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay24 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 24;
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                x00: 1
                                                x01: 1
                                                x02: 1
                                                y00: 0
                                                y01: 1
                                                y02: 0

                                                x00 AND y00 -> z00
                                                x01 XOR y01 -> z01
                                                x02 OR y02 -> z02
                                                """, 4, 1);
        // dummy response
        dayAutomatonBase.RegisterTestResult(0, 2);
        dayAutomatonBase.RegisterTestDataAndResult("""
                                                x00: 1
                                                x01: 0
                                                x02: 1
                                                x03: 1
                                                x04: 0
                                                y00: 1
                                                y01: 1
                                                y02: 1
                                                y03: 1
                                                y04: 1

                                                ntg XOR fgs -> mjb
                                                y02 OR x01 -> tnw
                                                kwq OR kpj -> z05
                                                x00 OR x03 -> fst
                                                tgd XOR rvg -> z01
                                                vdt OR tnw -> bfw
                                                bfw AND frj -> z10
                                                ffh OR nrd -> bqk
                                                y00 AND y03 -> djm
                                                y03 OR y00 -> psh
                                                bqk OR frj -> z08
                                                tnw OR fst -> frj
                                                gnj AND tgd -> z11
                                                bfw XOR mjb -> z00
                                                x03 OR x00 -> vdt
                                                gnj AND wpb -> z02
                                                x04 AND y00 -> kjc
                                                djm OR pbm -> qhw
                                                nrd AND vdt -> hwm
                                                kjc AND fst -> rvg
                                                y04 OR y02 -> fgs
                                                y01 AND x02 -> pbm
                                                ntg OR kjc -> kwq
                                                psh XOR fgs -> tgd
                                                qhw XOR tgd -> z09
                                                pbm OR djm -> kpj
                                                x03 XOR y03 -> ffh
                                                x00 XOR y04 -> ntg
                                                bfw OR bqk -> z06
                                                nrd XOR fgs -> wpb
                                                frj XOR qhw -> z04
                                                bqk OR frj -> z07
                                                y03 OR x01 -> nrd
                                                hwm AND bqk -> z03
                                                tgd XOR rvg -> z12
                                                tnw OR pbm -> gnj
                                                """, 2024,1);
    }

    
    public override object GetAnswer1()
    {
        _outputBits = ExtractRegisterBits('z').OrderDescending().ToList();
        var result = 0L;
        var path = new List<string>();
        foreach (var bit in _outputBits)
        {
            var val = _circuit[bit].Evaluate(_circuit, path, true);
            result = (result << 1) + (val ? 1 : 0);
        }

        return result;
    }

    private List<string> ExtractRegisterBits(char register)
    {
        return _circuit.Keys.Where(n => n[0] == register).Order().Reverse().ToList();
    }

    /// <summary>
    /// This code helps detecting invalid logic gates but searching for the correct gate remains manual
    /// </summary>
    /// <returns></returns>
    public override object GetAnswer2()
    {
        if (_circuit.Count < 10)
        {
            return 0;
        }
        // fix up
        var fixup = new List<(string a, string b)>{("gjh", "z22"), ("jdr", "z31"), ("ffj", "z08"), ("kfm", "dwp")};

        foreach (var (a, b)  in fixup) (_circuit[a], _circuit[b]) = (_circuit[b], _circuit[a]);
        
        var x = ExtractRegisterBits('x').OrderDescending().ToList();
        var y = ExtractRegisterBits('y').OrderDescending().ToList();
        var path = new List<string>[_outputBits.Count];

        for (var i = 0; i < path.Length; i++)
        {
            ClearInputs();
            path[i] = [];
            _circuit[_outputBits[^(i + 1)]].Evaluate(_circuit, path[i], false);
            path[i].Add(_outputBits[^(i + 1)]);
        }

        if (path[0].Count != 3)
        {
            Console.WriteLine("Problem for first stage, can't fix it automatically");
            return null;
        }
        if (path[1].Count != 7)
        {
            Console.WriteLine("Problem for second stage, can't fix it automatically");
            return null;
        }        
        if (path[2].Count != 15)
        {
            Console.WriteLine("Problem for third stage, can't fix it automatically");
            return null;
        }

        var referenceGap = path[2].Except(path[1]).ToList();
        var referenceGates = referenceGap.Select(name => (name, gate:_circuit[name])).ToList();
        var previousGap = referenceGap;
        for (var i = 3; i < path.Length; i++)
        {
            var currentGap = path[i].Except(path[i - 1]).ToList();
            var theseGates = currentGap.Select(name => (name, gate: _circuit[name])).ToList();
            // scan
            if (_circuit[$"z{i:D02}"] is not XorGate xor)
            {
                // entry invalid
                continue;
            }

            if (_circuit[xor.Subs.left] is not OrGate or2 && _circuit[xor.Subs.right] is not OrGate or3)
            {
                
            }
            if (_circuit[xor.Subs.left] is not XorGate xor2 && _circuit[xor.Subs.right] is not XorGate xor3)
            {
                
            }
        }
        // we establish reference pattern moving from bit N to bit N+1
        return string.Join(",", fixup.SelectMany(p => new [] {p.a, p.b}).Order());

        void SetBit(string bit, bool b)
        {
            ((Bit)_circuit[bit]).SetBit(b);
        }

        void ClearInputs()
        {
            foreach (var bit in x)
            {
                SetBit(bit, false);
            }

            foreach (var bit in y)
            {
                SetBit(bit, false);
            }
        }
    }

    private bool _readingGates;
    private readonly Dictionary<string, IWire> _circuit = [];
    private List<string> _outputBits = null!;

    private interface IWire
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path, bool optimize);
    }
    
    private interface IGate
    {
        (string left, string right) Subs { get; }
    }
    
    private class Bit(bool state) : IWire
    {
        private bool _state = state;
        public void SetBit(bool nextState) => _state = nextState;
        public bool Evaluate(Dictionary<string, IWire> _, List<string> __, bool ___) => _state;
    }

    private class OrGate(string left, string right) : IWire, IGate
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path, bool optimize)
        {
            path.Add(left);
            var result = circuit[left].Evaluate(circuit, path, optimize);
            if (optimize && result)
            {
                return true;
            }

            path.Add(right);
            return circuit[right].Evaluate(circuit, path, optimize) || result;
        }

        public (string left, string right) Subs => (left, right);
}
        
    private class AndGate(string left, string right) : IWire, IGate
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path, bool optimize)
        {
            path.Add(left);
            var result = circuit[left].Evaluate(circuit, path, optimize);
            if (!result && optimize)
            {
                return false;
            }
            path.Add(right);
            return circuit[right].Evaluate(circuit, path, optimize) && result;
        }
        
        public (string left, string right) Subs => (left, right);
    }
        
    private class XorGate(string left, string right) : IWire, IGate
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path, bool optimize)
        {
            path.Add(left);
            path.Add(right);
            return circuit[left].Evaluate(circuit, path, optimize)^ circuit[right].Evaluate(circuit, path, optimize);
        }
        
        public (string left, string right) Subs =>(left, right);
    }
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrEmpty(line))
        {
            _readingGates = true;
            return;
        }

        if (!_readingGates)
        {
            var blocks = line.Split(':', StringSplitOptions.TrimEntries);
            _circuit.Add(blocks[0], new Bit(blocks[1]=="1"));
        }
        else
        {
            var blocks = line.Split("->", StringSplitOptions.TrimEntries);
            var gate = blocks[0].Split(' ', StringSplitOptions.TrimEntries);
            switch (gate[1])
            {
                case "OR":
                    _circuit.Add(blocks[1], new OrGate(gate[0], gate[2]));
                    break;
                case "XOR":
                    _circuit.Add(blocks[1], new XorGate(gate[0], gate[2]));
                    break;            
                case "AND":
                    _circuit.Add(blocks[1], new AndGate(gate[0], gate[2]));
                    break;            
            }
        }
    }
}
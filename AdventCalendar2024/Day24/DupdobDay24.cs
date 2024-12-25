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

using AoC;

namespace AdventCalendar2024;

public class DupdobDay24 : SolverWithLineParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 24;
        automatonBase.RegisterTestDataAndResult("""
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
        automatonBase.RegisterTestDataAndResult("""
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
        var outputBits = _circuit.Keys.Where(n => n[0] == 'z').Order().Reverse().ToList();
        var result = 0L;
        var path = new List<string>();
        foreach (var bit in outputBits)
        {
            var val = _circuit[bit].Evaluate(_circuit, path);
            result = (result << 1) + (val ? 1 : 0);
        }

        return result;
    }

    public override object GetAnswer2()
    {
        throw new NotImplementedException();
    }

    private bool _readingGates = false;
    private readonly Dictionary<string, IWire> _circuit = [];
    
    private interface IWire
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path);
    }
    
    private class Bit(bool state) : IWire
    {
        public bool Evaluate(Dictionary<string, IWire> _, List<string> __) => state;
    }

    private class OrGate(string left, string right) : IWire
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path)
        {
            path.Add(left);
            if (circuit[left].Evaluate(circuit, path))
            {
                return true;
            }
            path.Add(right);
            return circuit[right].Evaluate(circuit, path);
        }
    }
        
    private class AndGate(string left, string right) : IWire
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path)
        {
            path.Add(left);
            if (!circuit[left].Evaluate(circuit, path))
            {
                return false;
            }
            path.Add(right);
            return circuit[right].Evaluate(circuit, path);
        }
    }
        
    private class XorGate(string left, string right) : IWire
    {
        public bool Evaluate(Dictionary<string, IWire> circuit, List<string> path)
        {
            path.Add(left);
            path.Add(right);
            return circuit[left].Evaluate(circuit, path)^ circuit[right].Evaluate(circuit, path);
        }
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
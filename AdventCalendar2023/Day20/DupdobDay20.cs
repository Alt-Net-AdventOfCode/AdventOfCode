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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay20 : SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 20;
        dayAutomatonBase.ResetBetweenQuestions = true;
        dayAutomatonBase.RegisterTestDataAndResult(@"broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output", 11687500, 1);
        
        dayAutomatonBase.RegisterTestDataAndResult(@"broadcaster -> a, b, c
%a -> b
%b -> c
%c -> inv
&inv -> a", 32000000, 1);
    }

    public override object GetAnswer1()
    {
        var low =0;
        var high = 0;
        for (var i = 0; i < 1000; i++)
        {
            PushButton((_, _, signal) =>
            {
                if (signal) 
                    high++;
                else
                    low++;
            });
        }

        return low*high;
    }

    public override object GetAnswer2()
    {
        if (!_modules.ContainsKey("rx"))
        {
            return 0;
        }
        
        // the "rx" module is fed from a huge conjunction module
        // we cannot brute force the situation where all inputs will be high
        // we assume these present independent cycles. We just have to compute the lcm afterward

        var conjunctor = _modules["rx"].Inputs.Keys.First();
        var cycles = _modules[conjunctor].Inputs.Keys.ToDictionary(p => p, _ => 0);
        var pulses = 0;
        while (true)
        {
            pulses++;
            PushButton((source, dest, signal) =>
            {
                if (signal && dest == conjunctor && cycles[source] == 0)
                {
                    cycles[source] = pulses ;
                    Console.WriteLine($"Found one cycle: {cycles[source]} for {source}.");
                }
            });
            if (cycles.Values.All(c => c != 0))
            {
                break;
            }
        }

        return cycles.Values.Aggregate(1L, (current, cycle) => current * cycle);
    }


    private void PushButton(Action<string, string, bool> hook)
    {
        var queue = new Queue<(string source, string target, bool pulse)>();

        Pulse("button", "broadcaster", false);
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            var resultPulses = _modules[next.target].ReceiveSignal(next.source, next.pulse);
            if (resultPulses.Item1 == null){continue;}
            foreach (var target in resultPulses.Item1)
            {
                Pulse(next.target, target, resultPulses.Item2);
            }
        }

        return;

        void Pulse(string source, string dest, bool signal)
        {
            queue.Enqueue((source, dest, signal));
            hook(source, dest, signal);
        }
    }
    
    private readonly Dictionary<string, Module> _modules= new ();

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var module = new Module();
        module.Parse(line, _modules);
    }

    private class Module
    {
        private const string State = "state";
        private char _operation;
        private string _name;
        private readonly Dictionary<string, bool> _inputs = new();
        private List<string> _children = new();
        private static readonly Regex Parser = new(@"([\%\&]?)(.+) ->(.+)", RegexOptions.Compiled);

        public Module(): this(string.Empty){}

        private Module(string name)
        {
            _name = name;
        }

        public Dictionary<string, bool> Inputs => _inputs;
        
        public void Parse(string definition, Dictionary<string, Module> modules)
        {
            var match = Parser.Match(definition);
            if (!match.Success)
            {
                throw new FormatException($"Failed to parse definition: {definition}");
            }

            _name = match.Groups[2].Value;

            if (modules.TryGetValue(_name, out var module) && !ReferenceEquals(module, this))
            {
                // the module has been declared as a listener
                module.Parse(definition, modules);
                return;
            }
            
            _operation = string.IsNullOrWhiteSpace(match.Groups[1].Value) ? ' ' : match.Groups[1].Value[0];
            _children = match.Groups[3].Value.Split(',').Select(p => p.Trim()).ToList();
            if (_operation == '%')
            {
                _inputs[State] = false;
            }

            foreach (var child in _children)
            {
                if (!modules.TryGetValue(child, out var childModule))
                {
                    childModule = new Module(child);
                    modules[child] = childModule;
                }

                childModule.RegisterSource(_name);
            }

            modules[_name] = this;
        }

        private void RegisterSource(string name)
        {
            _inputs[name] = false;
        }

        public (List<string>?, bool) ReceiveSignal(string input, bool signal)
        {
            _inputs[input] = signal;
            switch (_operation)
            {
                default:
                    // broadcaster
                    return (_children, signal);
                case '%':
                    // flip flop
                    if (signal)
                    {
                        return (null, false);
                    }

                    _inputs[State] = !_inputs[State];
                    return (_children, _inputs[State]);
                case '&':
                    return (_children, _inputs.Values.Any(p => !p));
            }
        }
    }
}
// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay11 : SolverWithParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 11;
        automaton.RegisterTestData(@"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1
");
        automaton.RegisterTestResult(10605);
        automaton.RegisterTestResult(2713310158L,2);
    }

    public override object GetAnswer1()
    {
        var hits = new Dictionary<int,int>(_monkeys.Count);
        foreach (var key in _monkeys.Keys)
        {
            hits[key] = 0;
        }
        for (var i = 0; i < 20; i++)
        {
            foreach (var monkey in _monkeys.Values)
            {
                hits[monkey.Id] += monkey.Items.Count;
                monkey.Examine(_monkeys);
            }
        }

        var orderedHits = hits.Values.OrderBy(x => -x).ToList();
        return orderedHits[0] * orderedHits[1];
    }

    public override object GetAnswer2()
    {
        var hits = new Dictionary<int,long>(_monkeys.Count);
        var modulo = 1L;
        foreach (var key in _monkeys.Keys)
        {
            hits[key] = 0L;
            _monkeys[key].Reset();
            modulo *= _monkeys[key].Divisor;
        }

        foreach (var monkey in _monkeys.Values)
        {
            monkey.LimiterModulo = modulo;
        }
        for (var i = 0; i < 10000; i++)
        {
            foreach (var monkey in _monkeys.Values)
            {
                hits[monkey.Id] += monkey.Items.Count;
                monkey.Examine(_monkeys, false);
            }
        }

        var orderedHits = hits.Values.OrderBy(x => -x).ToList();
        return orderedHits[0] * orderedHits[1];
    }

    private Dictionary<int, Monkey> _monkeys =new();

    protected override void Parse(string data)
    {
        var lines = data.Split('\n');
        // we discard the last line if it is empty (trailing newline), but we keep any internal newlines
        if (lines[^1].Length == 0) lines = lines[..^1];
        for (var i = 0; i < lines.Length; i += 7)
        {
            var id = int.Parse(lines[i][7..^1]);
            var items = lines[i + 1][17..].Split(',').Select(t => long.Parse(t.Trim())).ToList();
            Func<long, long> operation;
            var operandText = lines[i + 2][24..].Trim();
            if (lines[i + 2].StartsWith("  Operation: new = old *"))
            {
                if (operandText == "old")
                {
                    operation = x => x * x;
                }
                else
                {
                    var operand = int.Parse(operandText);
                    operation = x => x * operand;
                }
            }
            else
            {
                if (operandText == "old")
                {
                    operation = x => x + x;
                }
                else
                {
                    var operand = int.Parse(operandText);
                    operation = x => x + operand;
                }
            }

            var test = int.Parse(lines[i + 3][21..]);
            var ifTrue = int.Parse(lines[i + 4][29..]);
            var ifFalse = int.Parse(lines[i + 5][29..]);
            _monkeys[id] = new Monkey(id, operation, test, ifTrue, ifFalse, items);
        }
    } 
    
    private class Monkey
    {
        private int _id;
        private Func<long, long> _operation;
        private int _test;
        private int _monkeyIfTrue;
        private int _monkeyIfFalse;
        private List<long> _items;
        private readonly List<long> _initialItems;

        public int Id => _id;

        public List<long> Items => _items;

        public int Divisor => _test;
        public long LimiterModulo { get; set; }

        public Monkey(int id, Func<long, long> operation, int test, int monkeyIfTrue, int monkeyIfFalse, List<long> items)
        {
            _id = id;
            _operation = operation;
            _test = test;
            _monkeyIfTrue = monkeyIfTrue;
            _monkeyIfFalse = monkeyIfFalse;
            _initialItems = items.ToList();
            _items = _initialItems.ToList();
        }

        public void Reset()
        {
            _items = _initialItems.ToList();
        }
        
        public void Examine(IDictionary<int, Monkey> monkeys, bool lowerWorry = true)
        {
            foreach (var item in _items)
            {
                var worry = _operation(item);
                if (lowerWorry)
                {
                    worry /= 3;
                }
                else
                {
                    worry %= LimiterModulo;
                }
                var nextMonkey = (worry % _test == 0) ? _monkeyIfTrue : _monkeyIfFalse;
                monkeys[nextMonkey].Items.Add(worry);
            }
            _items.Clear();
        }
    }
}
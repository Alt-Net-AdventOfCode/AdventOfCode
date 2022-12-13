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
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay13 : SolverWithLineParser
{
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 13;
        automaton.RegisterTestData(@"[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]");
        automaton.RegisterTestResult(13);
        automaton.RegisterTestResult(140,2);
    }

    public override object GetAnswer1()
    {
        AddPackets();
        var score = 0;
        for (var index = 0; index < _packets.Count; index++)
        {
            var (left, right) = _packets[index];
            if (left.IsBefore(right) < 0)
            {
                score+=index+1;
            }
        }

        return score;
    }

    public override object GetAnswer2()
    {
        var packets = new List<Item>();
        foreach ((Item left, Item right)  in _packets)
        {
            packets.Add(left);
            packets.Add(right);
        }

        var first = new Item
            { SubItems = new List<Item> { new Item { SubItems = new List<Item> { new() { Value = 2 } } } } };
        var second = new Item
            { SubItems = new List<Item> { new Item { SubItems = new List<Item> { new() { Value = 6 } } } } };
        packets.Add(first);
        packets.Add(second);
        packets.Sort((item, item1) => item.IsBefore(item1));
        return (packets.IndexOf(first)+1) * (packets.IndexOf(second)+1);
    }

    private class Item
    {
        public List<Item>? SubItems;
        public int? Value;

        public int IsBefore(Item right)
        {
            if (Value.HasValue && right.Value.HasValue)
            {
                return Value.Value.CompareTo(right.Value.Value);
            }
            else if (right.Value.HasValue)
            {
                var tempItem = new Item
                {
                    SubItems = new List<Item> { right }
                };
                return IsBefore(tempItem);
            }
            else
            {
                if (Value.HasValue)
                {
                    var tempItem = new Item
                    {
                        SubItems = new List<Item> { this }
                    };
                    return tempItem.IsBefore(right);
                }
            }

            if (SubItems == null)
            {
                if (right.SubItems == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }

            if (right.SubItems == null)
            {
                return 1;
            }
            for (var i = 0; i < right.SubItems!.Count; i++)
            {
                if (i == SubItems!.Count)
                {
                    return -1;
                }

                var sub = SubItems[i].IsBefore(right.SubItems[i]);
                if (sub != 0)
                {
                    return sub;
                }
            }

            return SubItems.Count>right.SubItems.Count ? 1 : 0;
        }

        public override string ToString()
        {
            if (Value.HasValue)
            {
                return Value.ToString()!;
            }
            else if (SubItems == null)
            {
                return "[]";
            }

            return "[" + string.Join(',', SubItems) + "]";
        }
    }

    private readonly List<(Item left, Item right)> _packets = new();
    private readonly List<Item> _store = new();
    
    protected override void ParseLine(string line, int index, int lineCount)
    {

        if (string.IsNullOrWhiteSpace(line))
        {
            AddPackets();
            return;
        }

        Item ?item = null;
        var stack = new Stack<Item?>();
        for (var i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                case '[':
                    stack.Push(item);
                    item = new Item();
                    break;
                case ']':
                {
                    var tempItem = item;
                    item = stack.Pop();
                    if (item != null)
                    {
                        item.SubItems ??= new List<Item>();
                        item.SubItems.Add(tempItem!);
                    }
                    else
                    {
                        _store.Add(tempItem!);
                    }
                    break;
                }
                case ',':
                    break;
                default:
                {
                    var start = i;
                    while(line[i]>='0' && line[i]<='9')
                    {
                        i++;
                    }

                    var value = int.Parse(line.Substring(start, i-start));
                    i--;
                    var subItem = new Item
                    {
                        Value = value
                    };
                    item!.SubItems ??= new List<Item>();
                    item.SubItems.Add(subItem);
                    break;
                }
            }
        }
    }

    private void AddPackets()
    {
        if (_store.Count != 2)
        {
            return;
        }

        _packets.Add((_store[0], _store[1]));
        _store.Clear();
    }
}
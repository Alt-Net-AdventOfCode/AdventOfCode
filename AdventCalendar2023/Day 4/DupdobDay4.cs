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

using System.Threading.Tasks.Sources;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay4 : SolverWithLineParser
{
    private readonly Dictionary<int, (List<int> winning, List<int> numbers)> _cards = new();
    private readonly Dictionary<int, int> _copies = new();
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 4;
        dayAutomatonBase.RegisterTestDataAndResult(@"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11", 13, 1);
        dayAutomatonBase.RegisterTestResult(30, 2);
    }

    public override object GetAnswer1()
    {
        foreach (var (key, card) in _cards)
        {
            _copies[key] = card.winning.Count(number => card.numbers.Contains(number));
        }

        return _copies.Values.Sum( x=> (int)Math.Pow(2, x-1));
    }

    public override object GetAnswer2() => _copies.Keys.Sum(Score);

    private int Score(int value)
    {
        var card = 1;
        for (var x = value + 1; x < value + 1 + _copies[value]; x++)
        {
            card += Score(x);
        }

        return card;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;
        var majorParts = line.Split(':');
        var cardId = int.Parse(majorParts[0].Substring(5));
        var subParts = majorParts[1].Split('|');
        var winning = subParts[0].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToList();
        var cards = subParts[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToList();
        _cards[cardId] = (winning, cards);
    }
}
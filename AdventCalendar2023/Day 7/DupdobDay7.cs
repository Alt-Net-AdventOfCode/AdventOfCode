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

using System.Collections;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay7 : SolverWithLineParser
{
    private List<(string hand, int score)> _hands = new();
    private const string _cards = "23456789TJQKA";
    private const string _altCards = "J23456789TQKA";

    private enum HandType
    {
        High,
        OnePair,
        TwoPairs,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind,
    }

    private static int CardRank(char card, bool withJoker = false)
    {
        return (withJoker ? _altCards : _cards).IndexOf(card);
    }
    
    private static (HandType type, List<int> cards) Scan(string hand, bool withJoker = false)
    {
        var maps = new Dictionary<char, int>();
        foreach (var card in hand)
        {
            if (!maps.TryGetValue(card, out var count))
            {
                count = 0;
            }

            maps[card] = ++count;
        }

        var ordered = hand.Select(p=> CardRank(p, withJoker)).ToList();
        if (maps.Count == 1)
        {
            return (HandType.FiveOfAKind, ordered);
        }

        if (maps.Values.Contains(4))
        {
            return ( (withJoker && maps.ContainsKey('J')) ? HandType.FiveOfAKind : HandType.FourOfAKind, ordered);
        }

        if (maps.Values.Contains(3))
        {
            if (withJoker && maps.ContainsKey('J'))
                return (maps.Values.Contains(2) ? HandType.FiveOfAKind : HandType.FourOfAKind, ordered);
            return (maps.Values.Contains(2) ? HandType.FullHouse : HandType.ThreeOfAKind, ordered);
        }

        if (maps.Values.Contains(2))
        {
            if (withJoker && maps.TryGetValue('J', out var map))
            {
                if (map == 1)
                {
                    // if have two pairs, we now have a full house, otherwise we have a three of a kind
                    return (maps.Values.Count == 3 ? HandType.FullHouse : HandType.ThreeOfAKind, ordered) ;
                }
                // we have two jokers, so we have a four of a kind if we have another pair, otherwise three of a kind
                return (maps.Values.Count == 3 ? HandType.FourOfAKind : HandType.ThreeOfAKind, ordered) ;
            }
            
            return (maps.Values.Count == 3 ? HandType.TwoPairs : HandType.OnePair, ordered) ;
        }

        return (withJoker && maps.ContainsKey('J') ? HandType.OnePair : HandType.High, ordered);
    }
    
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 7;
        automatonBase.RegisterTestDataAndResult(@"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483", 6440, 1);
        automatonBase.RegisterTestResult(5905, 2);
    }

    public override object GetAnswer1()
    {
        var orderedHands = _hands.OrderByDescending((tuple =>
        {
            var scan = Scan(tuple.hand);
            var rank = (int)scan.type * 10_000_000;
            var cardRank = 15 * 15 * 15 * 15;
            for (var i = 0; i < scan.cards.Count; i++)
            {
                rank += scan.cards[i] * cardRank;
                cardRank /= 15;
            }
            return rank;
        })).ToList();
        var rank = orderedHands.Count();
        var score = 0L;
        foreach (var hand in orderedHands)
        {
            score += rank * hand.score;
            rank--;
        }

        return score;
    }

    public override object GetAnswer2()
    {
        var orderedHands = _hands.OrderByDescending((tuple =>
        {
            var scan = Scan(tuple.hand, true);
            var rank = (int)scan.type * 10_000_000;
            var cardRank = 15 * 15 * 15 * 15;
            for (var i = 0; i < scan.cards.Count; i++)
            {
                rank += scan.cards[i] * cardRank;
                cardRank /= 15;
            }
            return rank;
        })).ToList();
        var rank = orderedHands.Count();
        var score = 0L;
        foreach (var hand in orderedHands)
        {
            score += rank * hand.score;
            rank--;
        }

        return score;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var block = line.Split(' ');
        _hands.Add((block[0], int.Parse(block[1])));
    }
}
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

public class DupdobDay22 : SolverWithLineParser
{
    private const int CountOfChange = 2000;

    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 22;
        automatonBase.RegisterTestDataAndResult("""
                                                1
                                                10
                                                100
                                                2024
                                                """, 37327623, 1);
        automatonBase.RegisterTestDataAndResult("""
                                                1
                                                2
                                                3
                                                2024
                                                """, 23, 2);
    }

    private long NextSecret(long currentSecret)
    {
        currentSecret = Mix(currentSecret, currentSecret << 6);
        currentSecret = Prune(currentSecret);
        currentSecret = Mix(currentSecret, currentSecret >> 5);
        currentSecret = Prune(currentSecret);
        currentSecret = Mix(currentSecret, currentSecret << 11);
        return Prune(currentSecret);
    }

    private static long Mix(long secret, long value) => secret ^ value;

    private static long Prune(long secret) => secret % 16777216;

    public override object GetAnswer1()
    {
        var result = 0L;
        foreach (var seed in _seeds)
        {
            var secret = seed;
            for (var i = 0; i < CountOfChange; i++)
            {
                secret = NextSecret(secret);
            }
            result += secret;
        }

        return result;
    }

    public override object GetAnswer2()
    {
        var gains = new Dictionary<(int, int, int, int), int>(10000);
        foreach (var seed in _seeds)
        {
            var patternsOfVariation = new Dictionary<(int p1, int p2, int p3, int p4), int>(CountOfChange);
            var secret = seed;
            var variation = new List<(int, int)>(CountOfChange);
            for (var i = 0; i < CountOfChange; i++)
            {
                var nextSecret = NextSecret(secret);
                var nextPrice = (int) nextSecret%10;
                variation.Add((nextPrice, (int)(nextPrice-secret%10)));
                secret = nextSecret;
                if (i <= 2) continue;
                var pattern = (variation[i - 3].Item2, variation[i - 2].Item2, variation[i - 1].Item2, variation[i].Item2);
                if (patternsOfVariation.TryAdd(pattern, nextPrice))
                {
                    gains[pattern] = gains.GetValueOrDefault(pattern)+nextPrice;
                }
            }
        }

        return gains.Values.Max();
    }

    private readonly List<long> _seeds = [];
    protected override void ParseLine(string line, int index, int lineCount)
    {
        _seeds.Add(long.Parse(line));
    }

}
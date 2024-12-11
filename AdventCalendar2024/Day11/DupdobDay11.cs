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

using System.Security.Cryptography;
using AoC;

namespace AdventCalendar2024;

public class DupdobDay11: SolverWithParser
{
    private List<long> _stones;
        
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 11;
        automatonBase.RegisterTestDataAndResult("""
                                                125 17
                                                """, 55312, 1);
    }

    private Dictionary<long, Dictionary<long,long>> _Blicked25 = new();
    
    public override object GetAnswer1()
    {
        var stones = new List<long>(_stones);
        foreach (var b in stones)
        {
            _Blicked25[b] = FastBlink(b);
        }

        return _Blicked25.Values.Sum(p => p.Values.Sum());
    }

    private static Dictionary<long, long> FastBlink(long stone)
    {
        var dico = new Dictionary<long, long>{[stone] = 1};
        for (var i = 0; i < 25; i++)
        {
            var nextDico = new Dictionary<long, long>();
            foreach (var (newStone, quantity) in dico)
            {
                if (newStone == 0)
                {
                    StoreAdd(nextDico, 1, quantity);
                    continue;
                }
                var text = newStone.ToString();
                if (text.Length % 2 == 0)
                {
                    StoreAdd(nextDico, long.Parse(text[..(text.Length/2)]), quantity);
                    StoreAdd(nextDico, long.Parse(text[(text.Length/2)..]), quantity);
                }
                else
                {
                    StoreAdd(nextDico, newStone*2024, quantity);
                }
            }
            dico = nextDico;
        }
        return dico;

        void StoreAdd(Dictionary<long, long> dico, long stone, long quantity)
        {
            var current = dico.GetValueOrDefault(stone, 0);
            dico[stone] = current + quantity;
        }
    }

    public override object GetAnswer2()
    {
        // we do three passes
        var result = 0L;
        foreach (var stone in _stones)
        {
            if (!_Blicked25.TryGetValue(stone, out var dico1))
            {
                dico1 = FastBlink(stone);
                _Blicked25[stone] = dico1;
            }
            foreach (var pair in dico1)
            {
                if (!_Blicked25.TryGetValue(pair.Key, out var dico2))
                {
                    dico2 = FastBlink(pair.Key);
                    _Blicked25[pair.Key] = dico2;
                }
                foreach (var pair2 in dico2)
                {
                    if (!_Blicked25.TryGetValue(pair2.Key, out var dico3))
                    {
                        dico3 = FastBlink(pair2.Key);
                        _Blicked25[pair2.Key] = dico3;
                    }
                    result += pair.Value * pair2.Value* dico3.Values.Sum();
                }
            }
        }
        return result;
    }
    
    protected override void Parse(string data)
    {
        _stones = data.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
    }
}
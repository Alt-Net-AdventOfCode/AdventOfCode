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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay16: SolverWithParser
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 16;
        automatonBase.RegisterTestData("s1,x3/4,pe/b").RegisterTestResult("baedc", 1);
    }

    private record DanceMove(char Action, string Param1, string Param2);

    private readonly List<DanceMove> _danceMoves = [];
    
    protected override void Parse(string data)
    {
        var moves = data.Split(',', StringSplitOptions.TrimEntries);
        foreach (var move in moves)
        {
            var pars = move[1..].Split('/');
            _danceMoves.Add(new DanceMove(move[0], pars[0], pars.Length > 1 ? pars[1]: string.Empty));
        }
    }

    public override object GetAnswer1()
    {
        var count = 16;
        if (_danceMoves.Count < 10)
        {
            // this is a test
            count = 5;
        }
        var positions = new char[count];
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] =(char) ('a' + i);
        }

        Positions(ref positions);

        return new string(positions);
    }

    private void Positions(ref char[] positions)
    {
        foreach (var danceMove in _danceMoves)
        {
            switch (danceMove.Action)   
            {
                case 's':
                {
                    var nextPosition = new char[positions.Length];
                    var len = int.Parse(danceMove.Param1);
                    var cursor = 0;
                    for (var i = positions.Length - len; i < positions.Length; i++)
                    {
                        nextPosition[cursor++] = positions[i];
                    }

                    for (var i = 0; i < positions.Length - len; i++)
                    {
                        nextPosition[cursor++] = positions[i];
                    }

                    positions = nextPosition;
                    break;
                }
                case 'p':
                {
                    var posA = positions.Index().First(p => p.Item == danceMove.Param1[0]).Index;
                    var posB = positions.Index().First(p => p.Item == danceMove.Param2[0]).Index;
                    (positions[posA], positions[posB]) = (positions[posB], positions[posA]);
                    break;
                }
                case 'x':
                {
                    var posA = int.Parse(danceMove.Param1);
                    var posB = int.Parse(danceMove.Param2);
                    (positions[posA], positions[posB]) = (positions[posB], positions[posA]);
                    break;
                }
            }
        }
    }

    public override object GetAnswer2()
    {
        var count = 16;
        if (_danceMoves.Count < 10)
        {
            // this is a test
            count = 5;
        }
        var positions = new char[count];
        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] =(char) ('a' + i);
        }

        var cache = new Dictionary<string, int>();
        for (var i = 0; i < 1000000000; i++)
        {
            var key = new string(positions);
            if (cache.TryGetValue(key, out var value))
            {
                // we have a cycle
                var cycle = i - value;
                // we just some computations
                var phase = (1000000000 - value) % cycle;
                // now we need to find the result matching this id
                return cache.First(p => p.Value == phase).Key;
            }
            Positions(ref positions);
            cache[key] = i;
        }


        return new string(positions);
    }
}
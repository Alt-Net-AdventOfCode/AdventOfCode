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

using AoC;

namespace AdventCalendar2023;

public class DupdobDay2 : SolverWithLineParser
{

    private Dictionary<int, List<(int r, int g, int b)>> _games = new();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 2;
        automatonBase.RegisterTestDataAndResult(@"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
", 8, 1);
        automatonBase.RegisterTestResult(2286, 2);
    }

    public override object GetAnswer1()
    {
        var result = 0;
        foreach (var (key, grabs) in _games)
        {
            if (grabs.All(entry => entry is { r: <= 12, g: <= 13, b: <= 14 }))
            {
                result += key;
            }
        }

        return result;
    }

    public override object GetAnswer2()
    {
        var result = 0;
        foreach (var grabs in _games.Values)
        {
            (int r, int g, int b) acc = (0,0,0);
            acc = grabs.Aggregate(acc, (current1, current) => (Math.Max(current1.r, current.r), Math.Max(current1.g, current.g), Math.Max(current1.b, current.b)));

            result += acc.r * acc.g * acc.b;
        }

        return result;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        var first = line.Split(':');
        // we get the game id
        int gameId = int.Parse(first[0][5..]);
        _games[gameId] = new List<(int r, int g, int b)>();
        // now we parse if set
        foreach (var subset in first[1].Split(';'))
        {
            var r = 0;
            var g = 0;
            var b = 0;
            foreach (var block in subset.Split(','))
            {
                var grab = block.Trim().Split(' ');
                var number = int.Parse(grab[0]);
                switch (grab[1])
                {
                    case "red":
                        r = number;
                        break;
                    case "blue":
                        b = number;
                        break;
                    case "green":
                        g = number;
                        break;
                }
            }
            _games[gameId].Add((r,g,b));
        }
    }
}
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

public class DupdobDay14 : SolverWithLineParser
{
    private readonly List<char[]> _map = new ();
    
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 14;
        dayAutomatonBase.RegisterTestDataAndResult(@"O....#....
O.OO#....#
.....##...
OO.#O....O
.O.....O#.
O.#..O.#.#
..O..#O..O
.......O..
#....###..
#OO..#....", 136, 1);
        dayAutomatonBase.RegisterTestResult(64, 2);
    }

    public override object GetAnswer1()
    {
        var map = _map.Select(line => (char[])line.Clone()).ToList();
        TiltNorth(map);
        return ScoreMap(map);
    }

    private static void TiltNorth(IReadOnlyList<char[]> result)
    {
        for (var y = 1; y < result.Count; y++)
        {
            for (var x = 0; x < result[y].Length; x++)
            {
                if (result[y][x] != 'O' || result[y-1][x]!='.') continue;
                var lastLine = y;
                while (lastLine > 0 && result[lastLine - 1][x] == '.')
                {
                    lastLine--;
                }

                result[lastLine][x] = 'O';
                result[y][x] = '.';
            }
        }
    }   
    
    private static void TiltSouth(IReadOnlyList<char[]> result)
    {
        for (var y = result.Count-2; y >=0; y--)
        {
            for (var x = 0; x < result[y].Length; x++)
            {
                if (result[y][x] != 'O' || result[y+1][x] != '.') continue;
                var lastLine = y;
                while (lastLine <result.Count-1 && result[lastLine + 1][x] == '.')
                {
                    lastLine++;
                }

                result[lastLine][x] = 'O';
                result[y][x] = '.';
            }
        }
    }

    private static void TiltWest(IReadOnlyList<char[]> result)
    {
        for (var x = 1; x < result[0].Length; x++)
        {
            foreach (var t in result)
            {
                if (t[x] != 'O' || t[x-1]!='.') continue;
                var lastCol = x;
                while (lastCol > 0 && t[lastCol-1] == '.')
                {
                    lastCol--;
                }

                t[lastCol] = 'O';
                t[x] = '.';
            }
        }
    }

    private static void TiltEast(IReadOnlyList<char[]> result)
    {
        for (var x = result[0].Length-2; x >= 0; x--)
        {
            foreach (var t in result)   
            {
                if (t[x] != 'O' || t[x+1]!='.') continue;
                var lastCol = x;
                while (lastCol <result[0].Length-1 && t[lastCol+1] == '.')
                {
                    lastCol++;
                }

                t[lastCol] = 'O';
                t[x] = '.';
            }
        }
    }

    private static long ScoreMap(List<char[]> result)
    {
        var score = 0L;
        for (var y = 0; y < result.Count; y++)
        {
            for (var x = 0; x < result[y].Length; x++)
            {
                if (result[y][x] != 'O') continue;
                score += result.Count - y;
            }
        }

        return score;
    }

    private static void DumpMap(List<char[]> result)
    {
        foreach (var line in result)
        {
            Console.WriteLine(string.Join(null, line));
        }
        Console.WriteLine("------");
    }

    public override object GetAnswer2()
    {
        Dictionary<long, List<(List<char[]> map, int index)>> myHash = new ();
        var target = 1_000_000_000;
        var map = CloneMap(_map);
        int cycleStart =0, cycleLength = 0;
        var scores = new List<long>();
        for (var i = 0; i < target; i++)
        {
            Cycle(map);
            var score = ScoreMap(map);
            scores.Add(score);
            if (myHash.TryGetValue(score, out var list))
            {
                var foundMatch = false;
                foreach (var entry in list)
                {
                    if (!AreEqual(entry.map, map)) continue;
                    cycleStart = entry.index;
                    cycleLength = i - cycleStart;
                    foundMatch = true;
                    break;
                }

                if (foundMatch)
                {
                    break;
                }
                list.Add((CloneMap(map), i));
            }
            else
            {
                myHash[score] = new List<(List<char[]>, int)> { (CloneMap(map),i) };
            }
        }
        if (scores.Count == target)
        {
            return scores.Last();
        }
        Console.WriteLine($"Cycle start {cycleStart}, Cycle Length {cycleLength}.");
        target -= cycleStart+1;
        return scores[cycleStart + target % cycleLength];
    }

    private bool AreEqual(IReadOnlyList<char[]> entry, IReadOnlyList<char[]> map)
    {
        for (var y = 0; y < entry.Count; y++)
        {
            for (var x = 0; x < entry[y].Length; x++)
            {
                if (entry[y][x] != map[y][x])
                    return false;
            }
        }

        return true;
    }

    private static List<char[]> CloneMap(List<char[]> map)
    {
        return map.Select(line => line.ToArray()).ToList();
    }

    private static void Cycle(List<char[]> map)
    {
        TiltNorth(map);
        TiltWest(map);
        TiltSouth(map);
        TiltEast(map);
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _map.Add(line.ToArray());
    }
}
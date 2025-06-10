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
using AoCAlgorithms;

namespace AdventCalendar2022;

public class DupdobDay23: SolverWithLineParser
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 23;
        dayAutomaton.AddExample(@"....#..
..###.#
#...#.#
.#...##
#.###..
##.#.##
.#..#..
");
        dayAutomaton.RegisterTestResult(110);
        dayAutomaton.RegisterTestResult(20,2);
    }

    public override object GetAnswer1()
    {
        var maps = new SparseMap2D<char>(_maps);
        var firstDir = 0;
        for (var round = 0; round < 10; round++)
        {
            OneRound(maps, ref firstDir);
        }

        return maps.GetBoundedSurface()-maps.GetEntryCount();
    }

    private static int OneRound(SparseMap2D<char> maps, ref int firstDir)
    {
        var destinationsMap = new List<((int x, int y) to, (int x, int y) from)>();
        var selectedDestinations = new HashSet<(int, int)>();
        var discardedDestinations = new HashSet<(int, int)>();
        var testZones = new[] { new[] { 1, 2, 3 }, new[] { 5, 6, 7 }, new[] { 7, 0, 1 }, new[] { 3, 4, 5 } };
        foreach (var (x, y) in maps)
        {
            var scan = new List<((int x, int y) coord, bool ?elf)>(8);
            var atLeastOne = false;
            foreach (var coordinates in maps.Around((x,y)))
            {
                if (!atLeastOne)
                {
                    var exist = maps.Exists(coordinates);
                    scan.Add((coordinates, exist));
                    atLeastOne |= exist;
                }
                else
                {
                    scan.Add((coordinates, null));
                }
            }
            if (!atLeastOne)
            {
                // 0 neighbours
                continue;
            }

            var testDir = firstDir;
            (int x, int y)? dest = null;
            do
            {
                var check = testZones[testDir];
                if (check.All(p =>
                    {
                        if (scan[p].elf == null)
                        {
                            scan[p] = (scan[p].coord, maps.Exists(scan[p].coord));
                        }
                        return scan[p].elf == false;
                    }))
                {
                    dest = scan[check[1]].coord;
                    break;
                }

                testDir = (testDir + 1) % 4;
            } while (testDir != firstDir);

            if (!dest.HasValue) continue;
            destinationsMap.Add(((x,y), dest.Value));
            if (!selectedDestinations.Add(dest.Value))
            {
                // more than elf wants to go there
                discardedDestinations.Add(dest.Value);
            }
        }

        var moves = 0;
        // now we move the elves
        foreach (var (from, to) in destinationsMap)
        {
            if (discardedDestinations.Contains(to))
            {
                // don't move, conflict
                continue;
            }

            maps.RemoveAt(from);
            maps[to] = '#';
            moves++;
        }


        firstDir = (firstDir + 1) % 4;
        return moves;
    }

    public override object GetAnswer2()
    {
        var maps = new SparseMap2D<char>(_maps);
        var firstDir = 0;
        int moves;
        var round = 0;
        do
        {
            moves = OneRound(maps, ref firstDir);
            round++;
        } while (moves > 0);

        return round;
    }

    private readonly SparseMap2D<char> _maps = new('.');
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        for (var i = 0; i < line.Length; i++)
        {
            if (line[i] == '#')
            {
                _maps[i, index] = line[i];
            }
        }
    }
}
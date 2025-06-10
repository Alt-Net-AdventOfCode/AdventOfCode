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

using System.Text;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay13 : SolverWithLineParser
{
    private readonly List<List<string>> _patterns = new();
    private List<string>? _currentPattern;
    
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 13;
        dayAutomatonBase.RegisterTestDataAndResult(@"#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#", 405, 1);
    }

    public override object GetAnswer1()
    {
        var sum = 0L;
        foreach (var currentPattern in _patterns)
        {
            var axis = FindSymmetryAxis(currentPattern, new BasicComparer());
            if (axis != -1)
            {
                sum += axis * 100;
                continue;
            }

            axis = FindSymmetryAxis(Pivot(currentPattern), new BasicComparer());
            if (axis == -1)
            {
                continue;
            }
            sum += axis;
        }

        return sum;
    }

    private static List<string> Pivot(IReadOnlyList<string> currentPattern)
    {
        var pivoted = new List<string>();
        for (var x = 0; x < currentPattern[0].Length; x++)
        {
            var column = new StringBuilder();
            for (var y = currentPattern.Count-1; y >= 0; y--)
            {
                column.Append(currentPattern[y][x]);
            }
            pivoted.Add(column.ToString());
        }

        return pivoted;
    }

    private long FindSymmetryAxis(List<string> currentPattern, IPatternComparer comparer)
    {
        // find potential horizontal axis
        for (var i = 0; i < currentPattern.Count - 1; i++)
        {
            if (comparer.Compare(currentPattern[i], currentPattern[i + 1]))
            {
                comparer.Reset();
                if (CheckAxis(currentPattern, i, comparer))
                {
                    return i+1;
                }
            }
            comparer.Reset();
        }

        return -1;
    }

    private bool CheckAxis(List<string> currentPattern, int axis, IPatternComparer patternComparer)
    {
        var size = Math.Min(axis+1, currentPattern.Count - (axis +1));
        for (var i = 0; i < size; i++)
        {
            if (!patternComparer.Compare(currentPattern[axis-i], currentPattern[axis + 1+i]))
            {
                return false;
            }
        }

        return patternComparer.IsSuccess;
    }

    public override object GetAnswer2()
    {
        var sum = 0L;
        foreach (var currentPattern in _patterns)
        {
            var axis = FindSymmetryAxis(currentPattern, new PatternComparer());
            if (axis != -1)
            {
                sum += axis * 100;
            }
            else
            {
                axis = FindSymmetryAxis(Pivot(currentPattern), new PatternComparer());
                if (axis == -1)
                {
                    Console.WriteLine("failed");
                    continue;
                }
                sum += axis;
            }
        }

        return sum;
    }
    
    private interface IPatternComparer
    {
        bool IsSuccess { get; }
        bool Compare(string left, string right);
        void Reset();
    }

    private class BasicComparer : IPatternComparer
    {
        public bool IsSuccess => true;

        public bool Compare(string left, string right)
        {
            return left == right;
        }

        public void Reset()
        {}
    }

    private class PatternComparer : IPatternComparer
    {
        private bool _diffFound;

        public bool IsSuccess => _diffFound;

        public bool Compare(string left, string right)
        {
            if (_diffFound)
            {
                return left == right;
            }
            else
            {
                for (var i = 0; i < left.Length; i++)
                {
                    if (left[i] == right[i])
                    {
                        continue;
                    }

                    if (_diffFound)
                    {
                        return false;
                    }

                    _diffFound = true;
                }
            }

            return true;
        }

        public void Reset()
        {
            _diffFound = false;
        }
    }
    
    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            _currentPattern = null;
            return;
        }

        if (_currentPattern == null)
        {
            _currentPattern = new List<string>();
            _patterns.Add(_currentPattern);
        }
        _currentPattern.Add(line);
    }
}
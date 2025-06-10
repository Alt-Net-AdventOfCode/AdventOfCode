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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay8 : SolverWithLineParser
{
    private readonly List<List<int>> _trees = new();
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
        dayAutomaton.Day = 8;
        dayAutomaton.RegisterTestDataAndResult(@"30373
25512
65332
33549
35390", 21, 1);
        dayAutomaton.RegisterTestDataAndResult(@"30373
25512
65332
33549
35390", 8, 2);
        }

    private bool TreeIsVisible(int x, int y)
    {
        var height = _trees[y][x];
        var visible = true;
        for (var i = 0; i < x; i++)
        {
            if (height > _trees[y][i])
            {
                continue;
            }
            visible = false;
            break;
        }
        if (visible)
        {
            return true;
        }

        visible = true;
        for (var i = x+1; i < _trees[y].Count; i++)
        {
            if (height > _trees[y][i])
            {
                continue;
            }
            visible = false;
            break;
        }
        if (visible)
        {
            return true;
        }
        visible = true;
        for (var i = 0; i < y; i++)
        {
            if (height > _trees[i][x])
            {
                continue;
            }
            visible = false;
            break;
        }
        if (visible)
        {
            return true;
        }
        visible = true;
        for (var i = y+1; i < _trees.Count; i++)
        {
            if (height > _trees[i][x])
            {
                continue;
            }
            visible = false;
            break;
        }
        return visible;
    }
    private int ViewingScore(int x, int y)
    {
        var height = _trees[y][x];
        var totalScore = 1;
        var tempScore = 0;
        for (var i = x-1; i >=0; i--)
        {
            tempScore++;
            if (height <= _trees[y][i])
            {
                break;
            }
        }
        if (tempScore == 0)
        {
            return 0;
        }

        totalScore *= tempScore;
        tempScore = 0;
        for (var i = x+1; i <_trees[y].Count; i++)
        {
            tempScore++;
            if (height <= _trees[y][i])
            {
                break;
            }
        }
        if (tempScore == 0)
        {
            return 0;
        }

        totalScore *= tempScore;
        tempScore = 0;
        for (var i = y-1; i >=0; i--)
        {
            tempScore++;
            if (height <= _trees[i][x])
            {
                break;
            }
        }
        if (tempScore == 0)
        {
            return 0;
        }

        totalScore *= tempScore;
        tempScore = 0;
        for (var i = y+1; i <_trees.Count; i++)
        {
            tempScore++;
            if (height <= _trees[i][x])
            {
                break;
            }
        }
        totalScore *= tempScore;

        return totalScore;
    }

    public override object GetAnswer1()
    {
        var visibleTrees = 0;
        for (var y = 0; y < _trees.Count; y++)
        {
            for (var x = 0; x < _trees[y].Count; x++)
            {
                if (TreeIsVisible(x, y))
                {
                    visibleTrees++;
                }
            }
        }

        return visibleTrees;
    }

    public override object GetAnswer2()
    {
        var maxScore = 0;
        for (var y = 0; y < _trees.Count; y++)
        {
            for (var x = 0; x < _trees[y].Count; x++)
            {
                var score = ViewingScore(x, y);
                maxScore = Math.Max(score, maxScore);
            }
        }

        return maxScore;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        var trees = line.Select(car => car - '0').ToList();
        _trees.Add(trees);
    }
}
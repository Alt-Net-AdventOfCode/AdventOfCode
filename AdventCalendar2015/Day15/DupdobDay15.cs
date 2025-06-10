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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2015;

[Day(15)]
public class DupdobDay15: SolverWithParser
{
    private readonly List<int[]> _ingredientsProperties = [];

    public override void SetupRun(DayAutomaton dayAutomaton)
    { }

    protected override void Parse(string data)
    {
        var reg = new Regex(@"(\w+): capacity (-?\d+), durability (-?\d+), flavor (-?\d+), texture (-?\d+), calories (-?\d+)", RegexOptions.Compiled);
        foreach (var line in data.SplitLines())
        {
            var match = reg.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Unable to parse {0}.", line);
                continue;
            }

            _ingredientsProperties.Add([
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value),
                int.Parse(match.Groups[4].Value),
                int.Parse(match.Groups[5].Value), 
                int.Parse(match.Groups[6].Value)
            ]);
        }
    }

    [Example(1, @"utterscotch: capacity -1, durability -2, flavor 6, texture 3, calories 8
Cinnamon: capacity 2, durability 3, flavor -2, texture -1, calories 3", 62842880L)]
    public override object GetAnswer1() => ComputeMaxScore(0, new int[4], 100);

    private long ComputeMaxScore(int index, int[] score, int remainder)
    {
        var withCalories = score.Length > 4;
        if (index == _ingredientsProperties.Count-1)
        {
            if (withCalories && score[4]+remainder*_ingredientsProperties[index][4] != 500)
            {   
                return 0;
            }

            var total = 1;
            for (var i = 0; i < 4; i++)
            {
                total *=score[i]+_ingredientsProperties[index][i]*remainder;
                if (total <= 0)
                {
                    return 0;
                }
            }

            return total;
        }

        var maxScore = 0L;
        for (var i = 0; i <= remainder; i++)
        {
            var tempScore = score.Clone() as int[];
            for(var j=0; j < tempScore.Length; j++)
            {
                tempScore[j] += _ingredientsProperties[index][j] * i;
            }
            if (withCalories && tempScore[4]>500)
            {
                continue;
            }
            maxScore = Math.Max(maxScore, ComputeMaxScore(index + 1, tempScore, remainder-i));
        }

        return maxScore;
    }

    [ReuseExample(1, 57600000L)]
    public override object GetAnswer2() => ComputeMaxScore(0, new int[5], 100);
}
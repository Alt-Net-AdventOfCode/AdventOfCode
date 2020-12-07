using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventCalendar2015
{
    internal class DupdobDay15
    {
        public void Parse(string input = Input)
        {
            var reg = new Regex("(\\w+): capacity (-?\\d+), durability (-?\\d+), flavor (-?\\d+), texture (-?\\d+), calories (-?\\d+)");
            foreach (var line in input.Split('\n'))
            {
                var match = reg.Match(line);
                if (!match.Success)
                {
                    Console.WriteLine("Unable to parse {0}.", line);
                    continue;
                }

                ingredientsProperties.Add(new[]
                {
                    int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value),
                    int.Parse(match.Groups[4].Value),
                    int.Parse(match.Groups[5].Value),
                    int.Parse(match.Groups[6].Value)
                });
            }
        }

        public object Compute1()
        {
            var score = 0;
            for (var i1 = 0; i1 < 100; i1++)
            {
                for (var i2 = 0; i2 < 100 - i1; i2++)
                {
                    for (var i3 = 0; i3 < 100 -i1 - i2; i3++)
                    {
                        score = Math.Max(score, ComputeScore(i1, i2, i3, 100 - i1 - i2 -i3));
                    }
                }
            }

            return score;
        }

        private int ComputeScore(params int[] teaSpoons)
        {
            var totals = new[] {0, 0, 0, 0};
            for (var idx = 0; idx < teaSpoons.Length; idx++)
            {
                for (var i = 0; i < totals.Length; i++)
                {
                    totals[i]+= ingredientsProperties[idx][i] * teaSpoons[idx];
                }
            }

            var score = 1;
            foreach (var total in totals)
            {
                score *= Math.Max(0, total);
            }

            return score;
        }

        private int ComputeCalories(params int[] teaSpoons)
        {
            var calories = 0;
            for (var idx = 0; idx < teaSpoons.Length; idx++)
            {
                calories += teaSpoons[idx] * ingredientsProperties[idx][4];
            }

            return calories;
        }
        
        public object Compute2()
        {
            var score = 0;
            for (var i1 = 0; i1 < 100; i1++)
            {
                for (var i2 = 0; i2 < 100 - i1; i2++)
                {
                    for (var i3 = 0; i3 < 100 -i1 - i2; i3++)
                    {
                        var i4 = 100 - i1 - i2 - i3;
                        if (ComputeCalories(i1, i2, i3, i4) != 500)
                        {
                            continue;
                        }
                        score = Math.Max(score, ComputeScore(i1, i2, i3, i4));
                    }
                }
            }

            return score;
        }

        private List<int[]> 
            ingredientsProperties = new List<int[]>();
        
        private const string Input = @"Sprinkles: capacity 2, durability 0, flavor -2, texture 0, calories 3
Butterscotch: capacity 0, durability 5, flavor -3, texture 0, calories 3
Chocolate: capacity 0, durability 0, flavor 5, texture -1, calories 8
Candy: capacity 0, durability -1, flavor 0, texture 5, calories 8";
    }
}
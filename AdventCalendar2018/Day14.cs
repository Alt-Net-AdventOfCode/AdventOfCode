using System;
using System.Collections.Generic;
using System.Text;

namespace AdventCalendar2018
{
    public static class Day14
    {
        private static void MainDay14()
        {
            var recipes = new List<short> {3, 7};
            var elf1 = 0L;
            var elf2 = 1L;
            const int target = 503761;

            var nbDigits = (int) Math.Ceiling(Math.Log10(target));
            for (var i = 0; i < int.MaxValue; i++)
            {
                var i1 = recipes[(int)elf1];
                var i2 = recipes[(int)elf2];
                var newRecipes = i1 + i2;
                var tTarget = target;
                if (newRecipes > 9)
                {
                    recipes.Add(1);
                    newRecipes -= 10;
                    for (var j = recipes.Count - 1; j >= recipes.Count-nbDigits; j--)
                    {
                        if (recipes[j] != tTarget % 10)
                        {
                            break;
                        }

                        tTarget /= 10;
                    }
                    if (tTarget == 0)
                    {
                        Console.WriteLine($"Result 2 : {recipes.Count-nbDigits}");
                        break;
                    }
                }
                recipes.Add((short) newRecipes);
                elf1 = (elf1 + i1 + 1) % recipes.Count;
                elf2 = (elf2 + i2 + 1) % recipes.Count;
                // check if we have match

            }
/*
            var result = new StringBuilder();
            for (var i = target; i < 10+target; i++)
            {
                result.Append((char)('0' + recipes[i]));
            }
            Console.WriteLine($"Result :{result.ToString()}");
            */
        }
    }
}
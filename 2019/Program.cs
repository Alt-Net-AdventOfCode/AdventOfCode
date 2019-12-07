using System;
using AdventCalendar2019.Day7;

namespace AdventCalendar2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var puzzle = new Dupdob_Day7();
            puzzle.ParseInput();
            Console.WriteLine($"Answer 1: {puzzle.FindMaxPower()}");
            Console.WriteLine($"Answer 2: {puzzle.FindMaxPowerWithAlt()}");
        }
    }
}
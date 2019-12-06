using System;
using AdventCalendar2019.Day_6;

namespace AdventCalendar2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var puzzle = new DupdobDay6();
            puzzle.ParseInput();
            Console.WriteLine($"Answer 1: {puzzle.CountOrbits()}");
            Console.WriteLine($"Answer 2: {puzzle.FindMinimumNumberOfOrbitalTransfers()}");
        }
    }
}
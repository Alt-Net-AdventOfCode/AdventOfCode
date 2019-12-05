using System;
using AdventCalendar2019.Day_2;

namespace AdventCalendar2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var puzzle = new Dupdob_Day2();
            puzzle.ParseInput();
            Console.WriteLine($"Answer 1: {puzzle.ComputeAnswer()}");
            Console.WriteLine($"Answer 1: {puzzle.ComputeAnswer2()}");
        }
    }
}
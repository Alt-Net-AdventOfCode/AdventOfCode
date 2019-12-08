using System;
using AdventCalendar2019.Day7;
using AdventCalendar2019.Day8;

namespace AdventCalendar2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var puzzle = new DupdobDay8();
            puzzle.ParseInput();
            Console.WriteLine($"Answer 1: {puzzle.ElvishChecksum()}");
            Console.WriteLine($"Answer 2:\n{puzzle.GenerateSingleMessage()}");
        }
    }
}
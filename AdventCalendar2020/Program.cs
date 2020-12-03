using System;
using AdventCalendar2020.Day_1;
using AdventCalendar2020.Day_2;
using AdventCalendar2020.Day_3;

namespace AdventCalendar2020
{
    class Program
    {
        static void Main(string[] args)
        {
            var day3 = new DupdobDay3();
            day3.Parse();
            Console.WriteLine($"Day 1: {day3.GiveAnswer1()} & {day3.GiveAnswer2()}");
        }

        private static void FullCalendar()
        {
            var day1 = new DupdobDay1();
            day1.Parse();
            Console.WriteLine($"Day 1: {day1.GiveAnswer1()} & {day1.GiveAnswer2()}");
            var day2 = new DupdobDay2();
            day2.Parse();
            Console.WriteLine($"Day 1: {day2.GiveAnswer1()} & {day2.GiveAnswer2()}");
        }
    }
}
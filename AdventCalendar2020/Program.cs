using System;
using AdventCalendar2020.Day_1;
using AdventCalendar2020.Day_2;
using AdventCalendar2020.Day_3;
using AdventCalendar2020.Day_4;
using AdventCalendar2020.Day_5;
using AdventCalendar2020.Day_6;
using AdventCalendar2020.Day_7;
using AdventCalendar2020.Day_8;

namespace AdventCalendar2020
{
    class Program
    {
        static void Main(string[] args)
        {
            var day = new DupdobDay8();
            day.SetData();
            Console.WriteLine($"Day {day.Day}: {day.GiveAnswer1()} & {day.GiveAnswer2()}");
        }

        private static void FullCalendar()
        {
            var day1 = new DupdobDay1();
            day1.Parse();
            Console.WriteLine($"Day 1: {day1.GiveAnswer1()} & {day1.GiveAnswer2()}");
            var day2 = new DupdobDay2();
            day2.Parse();
            Console.WriteLine($"Day 2: {day2.GiveAnswer1()} & {day2.GiveAnswer2()}");
            var day3 = new DupdobDay3();
            day3.Parse();
            Console.WriteLine($"Day 3: {day3.GiveAnswer1()} & {day3.GiveAnswer2()}");
            var day4 = new DupdobDay4();
            day4.Parse();
            Console.WriteLine($"Day 4: {day4.GiveAnswer1()} & {day4.GiveAnswer2()}");
            var day5 = new DupdobDay5();
            day5.Parse();
            Console.WriteLine($"Day 5: {day5.GiveAnswer1()} & {day5.GiveAnswer2()}");
            var day6 = new DupdobDay6();
            day6.Parse();
            Console.WriteLine($"Day 6: {day6.GiveAnswer1()} & {day6.GiveAnswer2()}");
            var day7 = new DupdobDay7();
            day7.Parse();
            Console.WriteLine($"Day 7: {day7.GiveAnswer1()} & {day7.GiveAnswer2()}");
            var day = new DupdobDay8();
            day.SetData();
            Console.WriteLine($"Day {day.Day}: {day.GiveAnswer1()} & {day.GiveAnswer2()}");
        }
    }
}
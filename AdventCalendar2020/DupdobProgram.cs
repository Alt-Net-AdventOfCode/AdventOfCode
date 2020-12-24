using System;
using AdventCalendar2020.Day_1;
using AdventCalendar2020.Day_2;
using AdventCalendar2020.Day_3;
using AdventCalendar2020.Day_4;
using AdventCalendar2020.Day_5;
using AdventCalendar2020.Day_6;
using AdventCalendar2020.Day_7;
using AdventCalendar2020.Day_8;
using AdventCalendar2020.Day_9;
using AdventCalendar2020.Day10;
using AdventCalendar2020.Day11;
using AdventCalendar2020.Day12;
using AdventCalendar2020.Day13;
using AdventCalendar2020.Day14;
using AdventCalendar2020.Day15;
using AdventCalendar2020.Day16;
using AdventCalendar2020.Day17;
using AdventCalendar2020.Day18;
using AdventCalendar2020.Day19;
using AdventCalendar2020.Day20;
using AdventCalendar2020.Day21;
using AdventCalendar2020.Day22;
using AdventCalendar2020.Day23;
using AdventCalendar2020.Day24;
using AOCHelpers;

namespace AdventCalendar2020
{
    static class DupdobProgram
    {
        private static void Main()
        {    
            new DupdobDay24().OutputAnswers();
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
            DupdobDayBase day = new DupdobDay8();
            day.SetData();
            Console.WriteLine($"Day {day.Day}: {day.GiveAnswer1()} & {day.GiveAnswer2()}");
            day = new DupdobDay9();
            day.SetData();
            Console.WriteLine($"Day {day.Day}: {day.GiveAnswer1()} & {day.GiveAnswer2()}");

            new DupdobDay10().OutputAnswers();
            new DupdobDay11().OutputAnswers();
            new DupdobDay12().OutputAnswers();
            new DupdobDay13().OutputAnswers();
            new DupdobDay14().OutputAnswers();
            new DupdobDay15().OutputAnswers();
            new DupdobDay16().OutputAnswers();
            new DupdobDay17().OutputAnswers();
            new DupdobDay18().OutputAnswers();
            new DupdobDay19().OutputAnswers();
            new DupdobDay20().OutputAnswers();
            new DupdobDay21().OutputAnswers();
            new DupdobDay22().OutputAnswers();
            new DupdobDay23().OutputAnswers();
        }
    }
}
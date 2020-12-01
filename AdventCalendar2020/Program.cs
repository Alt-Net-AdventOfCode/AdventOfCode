using System;
using AdventCalendar2020.Day_1;

namespace AdventCalendar2020
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new DupdobDay1();
            test.Parse();
            Console.WriteLine($"Day 1: {test.GiveAnswers()} & {test.GiveAnswer2()}");
        }
    }
}
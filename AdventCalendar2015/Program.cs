using System;

namespace AdventCalendar2015
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            //GiveAllAnswers();
        }

        private static void GiveAllAnswers()
        {
            var day1 = new DupdobDay1();
            Console.WriteLine("Day 1: (1) = {0} , (2)= {1}.", day1.ComputeDay1(), day1.ComputeDay2());
            var day10 = new DupdobDay10();
            day10.Parse();
            Console.WriteLine("Day 10: (1) = {0} , (2) = {1}", day10.RepeatedLookAndSay(40), day10.RepeatedLookAndSay(10));
            var day11 = new DupdobDay11();
            day11.Parse();
            Console.WriteLine("Day 10: (1) = {0} , (2) = {1}", day11.Compute1(), day11.Compute2());
        }
    }
}
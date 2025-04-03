    using System;
    using AoC;
    using AOCHelpers;

namespace AdventCalendar2015
{
    internal static class DupdobProgram
    {
        private static void Main(string[] args)
        {
            var automaton = Automaton.WebsiteAutomaton(2015);
            automaton.SetRootPath("../../../");
            automaton.SetDataPath("Day{0,2}");
            automaton.RunDay<DupdobDay12>();
        }

        private static void GiveAllAnswers()
        {
            var day1 = new DupdobDay1();
            Console.WriteLine("Day 1: (1) = {0} , (2)= {1}.", day1.ComputeDay1(), day1.ComputeDay2());
            new DupdobDay2().OutputAnswers();
            new DupdobDay3().OutputAnswers();
            new DupdobDay4().OutputAnswers();
            new DupdobDay5().OutputAnswers();
            new DupdobDay6().OutputAnswers();
            new DupdobDay7().OutputAnswers();
            new DupdobDay8().OutputAnswers();
            new DupdobDay9().OutputAnswers();
            var day10 = new OldSolvers.DupdobDay10();
            day10.Parse();
            Console.WriteLine("Day 10: (1) = {0} , (2) = {1}", day10.RepeatedLookAndSay(40),
                day10.RepeatedLookAndSay(10));
            var day11 = new OldDupdobDay11();
            day11.Parse();
            Console.WriteLine("Day 11: (1) = {0} , (2) = {1}", day11.Compute1(), day11.Compute2());
            var day12 = new OldSolvers.DupdobDay12();
            day12.Parse();
            Console.WriteLine("Day 12: (1) = {0} , (2) = {1}", day12.Compute1(), day12.Compute2());
            var day13 = new DupdobDay13();
            day13.Parse();
            Console.WriteLine("Day 13: (1) = {0} , (2) = {1}", day13.Compute1(), day13.Compute2());
            var day14 = new DupdobDay14();
            day14.Parse();
            Console.WriteLine("Day 14: (1) = {0} , (2) = {1}", day14.Compute1(), day14.Compute2());
            var day15 = new DupdobDay15();
            day15.Parse();
            Console.WriteLine("Day 15: (1) = {0} , (2) = {1}", day15.Compute1(), day15.Compute2());
            DupdobDayBase day= new DupdobDay17();
            day.SetData();
            Console.WriteLine("Day {2}: (1) = {0} , (2) = {1}", day.GiveAnswer1(), day.GiveAnswer2(), day.Day);
            day = new DupdobDay18();
            day.SetData();
            Console.WriteLine("Day {0}: (1) = {1} , (2) = {2}", day.Day, day.GiveAnswer1(), day.GiveAnswer2());
            new DupdobDay19().OutputAnswers();
            new DupdobDay20().OutputAnswers();
            new DupdobDay21().OutputAnswers();
            new DupdobDay22().OutputAnswers();
            new DupdobDay23().OutputAnswers();
            new DupdobDay24().OutputAnswers();
            new DupdobDay25().OutputAnswers();
        }
    }
}
using System;
using System.Linq;
using AoC;
using AOCHelpers;

namespace AdventCalendar2016
{
    internal static class DupdobProgram
    {
        private static readonly Type[] Solvers =
        {
            typeof(DupdobDay6), 
            typeof(DupdobDay5), 
            typeof(DupdobDay4), 
            typeof(DupdobDay3), 
            typeof(DupdobDay2), 
            typeof(DupdobDay1)
        };
        static void Main(string[] args)
        {
            var engine = new Engine(2016);
            engine.RunDay<DupdobDay11>();
        }

        static void GiveAllDays()
        {
            var orderedDays = Solvers.Select(DupdobDayBase.BuildFromType).OrderBy(d => d.Day);
            foreach (var day in orderedDays)
            {
                day.OutputAnswers();
            }
        }
    }
}
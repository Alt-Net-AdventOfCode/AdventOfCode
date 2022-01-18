using System;
using System.Linq;
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
            DayEngine.Year = 2016;
            var engine = new DayEngine();
            engine.RunDay<DupdobDay10>();
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
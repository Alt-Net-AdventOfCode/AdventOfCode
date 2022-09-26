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
            typeof(DupdobDay12), 
            typeof(DupdobDay11), 
            typeof(DupdobDay10), 
            typeof(DupdobDay9), 
            typeof(DupdobDay8), 
            typeof(DupdobDay7), 
            typeof(DupdobDay6), 
            typeof(DupdobDay5), 
            typeof(DupdobDay4), 
            typeof(DupdobDay3), 
            typeof(DupdobDay2), 
            typeof(DupdobDay1)
        };

        private static void Main()
        {
            var engine = new Automaton(2016);
            engine.SetDataPath("../../../Day{0,2}");
            engine.RunDay<DupdobDay13>();
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
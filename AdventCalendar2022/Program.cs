﻿using AoC;

namespace AdventCalendar2022
{
    internal static class Program
    {
        private static void Main()
        {
            var engine = new HttpAutomaton(2022);
            engine.SetDataPath("../../../Day{0,2}");
            engine.RunDay<DupdobDay17>();
        }
    }
}
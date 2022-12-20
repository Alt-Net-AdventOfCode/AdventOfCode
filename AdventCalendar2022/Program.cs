using AoC;

namespace AdventCalendar2022
{
    internal static class Program
    {
        private static void Main()
        {
            var engine = new Automaton(2022);
            engine.SetDataPath("../../../Day{0,2}");
            engine.RunDay<DupdobDay20>();
        }
    }
}
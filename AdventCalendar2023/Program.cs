using AoC;

namespace AdventCalendar2023;

internal static class Program
{
    private static void Main()
    {
        var engine = new Automaton(2023);
        engine.SetDataPath("../../../Day{0,2}");
        engine.RunDay<DupdobDay14>();
    }
}

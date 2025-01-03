using AoC;

namespace AdventCalendar2017;

internal static class Program
{
    private static void Main(string[] args)
    {
        var engine = new Automaton(2017);
        engine.SetDataPath("../../../Day{0,2}");
        engine.RunDay<DupdobDay05>();
    }
}
using AoC;
namespace AdventCalendar2024;

class Program
{
    private static void Main()
    {
        var engine = new HttpAutomaton(2024);
        engine.SetDataPath("../../../Day{0,2}");
        engine.RunDay<DupdobDay14>();
    }
}
using AoC;

namespace AdventCalendar2019
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var automaton = Automaton.WebsiteAutomaton(2019);
            automaton.RunDay<DupdobDay1>();
        }
    }
}
// See https://aka.ms/new-console-template for more information

using AdventCalendar2025;
using AoC;

var engine = Automaton.WebsiteAutomaton(2025);
engine.SetDataPath("../../../Day{0:00}");
engine.RunDay<DupdobDay06>();
using System;
using System.Text.RegularExpressions;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay9: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            var matched = _matcher.Match(line);
            if (!matched.Success)
            {
                throw new ApplicationException($"Failed to parse {line}.");
            }

            _graph.AddEdge(matched.Groups[1].Value, matched.Groups[2].Value, int.Parse(matched.Groups[3].Value),
                true);
        }

        public override object GiveAnswer1()
        {
            return FindShortestPath();
        }

        public override object GiveAnswer2()
        {
            return FindLongestPath();
        }

        private int FindShortestPath()
        {
            return _graph.TravelingSalesman();
        }

        private int FindLongestPath()
        {
            return _graph.LongestPath();
        }

        private readonly Graph _graph = new();
        private readonly Regex _matcher = new("(\\w+) to (\\w+) = (\\d+)", RegexOptions.Compiled);
        public override int Day => 9;
        protected override string Input =>
@"Faerun to Norrath = 129
Faerun to Tristram = 58
Faerun to AlphaCentauri = 13
Faerun to Arbre = 24
Faerun to Snowdin = 60
Faerun to Tambi = 71
Faerun to Straylight = 67
Norrath to Tristram = 142
Norrath to AlphaCentauri = 15
Norrath to Arbre = 135
Norrath to Snowdin = 75
Norrath to Tambi = 82
Norrath to Straylight = 54
Tristram to AlphaCentauri = 118
Tristram to Arbre = 122
Tristram to Snowdin = 103
Tristram to Tambi = 49
Tristram to Straylight = 97
AlphaCentauri to Arbre = 116
AlphaCentauri to Snowdin = 12
AlphaCentauri to Tambi = 18
AlphaCentauri to Straylight = 91
Arbre to Snowdin = 129
Arbre to Tambi = 53
Arbre to Straylight = 40
Snowdin to Tambi = 15
Snowdin to Straylight = 99
Tambi to Straylight = 70";

        protected override void SetupTestData(int id)
        {
        }

        protected override void SetupRunData()
        {
        }
    }
}
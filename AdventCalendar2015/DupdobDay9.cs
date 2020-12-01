using System;
using System.Text.RegularExpressions;

namespace AdventCalendar2015
{
    public class DupdobDay9
    {
        public void Parse(string input = Input)
        {
            foreach (var line in input.Split('\n'))
            {
                var matched = matcher.Match(line);
                if (!matched.Success)
                {
                    throw new ApplicationException($"Failed to parse {line}.");
                }

                graph.AddEdge(matched.Groups[1].Value, matched.Groups[2].Value, int.Parse(matched.Groups[3].Value),
                    true);
            }
        }

        public int FindShortestPath()
        {
            return graph.TravelingSalesman();
        }        
        
        public int FindLongestPath()
        {
            return graph.LongestPath();
        }

        private readonly Graph graph = new Graph();
        private readonly Regex matcher = new Regex("(\\w+) to (\\w+) = (\\d+)", RegexOptions.Compiled);
        private const string Input =
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
    }
}
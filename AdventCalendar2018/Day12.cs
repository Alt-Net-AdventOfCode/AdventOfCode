using System;
using System.Collections.Generic;
using System.Text;

namespace AdventCalendar2018
{
    public static class Day12
    {
        private static void MainDay12()
        {
            const string initialStateMarker = "initial state: ";
            var lines = Input.Split(Environment.NewLine);
            var state = string.Empty;
            var nbGenerations = 50000000000L;
            var prefixSize = 0L;
            var rules = new Dictionary<string, string>();
            var fastMode = true;
            foreach (var line in lines)
            {
                if (line.StartsWith(initialStateMarker))
                {
                    var builder = new StringBuilder();
                    builder.Append(line.Substring(initialStateMarker.Length));
                    state = builder.ToString();
                }
                else if (line.Contains("=>"))
                {
                    rules[line.Substring(0, 5)] = line.Substring(9);
                }
            }

            for (var i = 0; i < nbGenerations; i++)
            {
                var stateBuilder = new StringBuilder(state.Length);
                var initState = state;
                state = ".." + state + "..";
                for (var pot = 0; pot < state.Length - 4; pot++)
                {
                    stateBuilder.Append(rules[state.Substring(pot, 5)]);
                }
                state = stateBuilder.ToString();
                // ensure there is at least two empty pots in front of the list
               if (state[0] != '.')
                {
                    state = ".." + state;
                    prefixSize += 2;
                }
                else if (state[1] != '.')
                {
                    state = "." + state;
                    prefixSize++;
                }
               else if (state[2] == '.')
               {
                   state = state.Substring(1);
                   prefixSize--;
               }

                if (!state.EndsWith("."))
                {
                    state += "..";
                }
                else if (!state.EndsWith(".."))
                {
                    state += ".";
                }
                Console.WriteLine(state);
                if (state == initState && fastMode)
                {
                    Console.WriteLine($"Is the same starting at generation {i} with a shift of {prefixSize}");
                    // autoshift
                    prefixSize -= nbGenerations - i -1;
                    break;
                }
            }
            
            Console.WriteLine($"Starts at : {-prefixSize}");
            // compute score
            var score = 0L;
            for (var i = 0; i < state.Length; i++)
            {
                if (state[i] == '#')
                {
                    score += i - prefixSize;
                }
            }
            Console.WriteLine($"Starts at : {-prefixSize}, score is: {score}");
        }

        private const string @Input = @"
initial state: .#..##..#.....######.....#....####.##.#.#...#...##.#...###..####.##.##.####..######......#..##.##.##

#.... => .
.##.# => #
#..## => .
....# => .
###.# => #
...#. => #
#...# => #
#.### => .
.#... => #
...## => .
..### => .
####. => .
##.## => .
..##. => .
.#.## => #
#..#. => #
..... => .
#.#.. => .
##.#. => #
.#### => #
##### => .
#.##. => #
.#..# => #
##... => .
..#.# => #
##..# => #
.###. => .
.#.#. => #
#.#.# => #
###.. => .
.##.. => .
..#.. => .";
    }
    
}
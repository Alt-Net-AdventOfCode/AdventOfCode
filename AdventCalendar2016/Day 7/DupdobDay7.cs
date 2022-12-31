using System.Collections.Generic;
using System.Linq;
using AoC;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay7 : SolverWithLineParser
    {
        private readonly List<string> _lines = new();

        protected override void ParseLine(string line, int index, int lineCount)
        {
           _lines.Add(line);
        }

        public override object GetAnswer1()
        {
            var score = 0;
            foreach (var line in _lines)
            {
                var match = false;
                var inBrackets = false;
                for (var i = 0; i < line.Length-3; i++)
                {
                    if (line[i] == '[')
                    {
                        inBrackets = true;
                    }
                    else if (line[i] == ']')
                    {
                        inBrackets = false;
                    }
                    else if (line[i] != line[i + 1] && line[i] == line[i+3] && line[i + 1] == line[i + 2])
                    {
                        // we have a match
                        match = true;
                        if (inBrackets)
                        {
                            break;
                        }
                    }
                }

                if (match && !inBrackets)
                {
                    score++;
                }
            }

            return score;
        }
        public override object GetAnswer2()
        {
            var score = 0;
            foreach (var line in _lines)
            {
                var superNetAbas = new List<string>();
                var interNetBaBs = new List<string>();
                var inBrackets = false;
                for (var i = 0; i < line.Length-2; i++)
                {
                    switch (line[i])
                    {
                        case '[':
                            inBrackets = true;
                            break;
                        case ']':
                            inBrackets = false;
                            break;
                        default:
                        {
                            if (line[i+1] == '[' || line[i+1] == ']')
                            {
                            }
                            else if (line[i] != line[i + 1] && line[i] == line[i+2])
                            {
                                // we have a match
                                if (inBrackets)
                                {
                                    interNetBaBs.Add(line[i..(i+2)]);
                                }
                                else
                                {
                                    superNetAbas.Add(line[(i+1)..(i+3)]);
                                }
                            }

                            break;
                        }
                    }
                }

                if (interNetBaBs.Intersect(superNetAbas).Any())
                {
                    score++;
                }
            }

            return score;
        }

        public override void SetupRun(Automaton automaton)
        {
            automaton.Day = 7;
            automaton.RegisterTestDataAndResult(@"abba[mnop]qrst
abcd[bddb]xyyx
aaaa[qwer]tyui
ioxxoj[asdfgh]zxcvbn", 2, 1);
            automaton.RegisterTestDataAndResult(@"aba[bab]xyz
xyx[xyx]xyx
aaa[kek]eke
zazbz[bzb]cdb",3, 2);
        }
    }
}
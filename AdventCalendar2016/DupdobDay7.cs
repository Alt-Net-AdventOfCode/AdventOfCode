using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay7 : Algorithm
    {
        private readonly List<string> _lines = new();
  
        public override void ParseLine(string line, int index, int lineCount)
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
                var end = line.IndexOf('[');
                if (end == -1)
                {
                    end = line.Length;
                }

                var match = false;
                var inBrackets = false;
                for (var i = 0; i < line.Length-2; i++)
                {
                    if (line[i] == '[')
                    {
                        inBrackets = true;
                    }
                    else if (line[i] == ']')
                    {
                        inBrackets = false;
                    }
                    else if (line[i+1] == '[' || line[i+1] == ']')
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
                }

                if (interNetBaBs.Intersect(superNetAbas).Any())
                {
                    score++;
                }
            }

            return score;
        }

        public override void SetupRun(DayEngine dayEngine)
        {
            dayEngine.Day = 7;
            dayEngine.RegisterTestData(1, @"abba[mnop]qrst
abcd[bddb]xyyx
aaaa[qwer]tyui
ioxxoj[asdfgh]zxcvbn", 2);
            dayEngine.RegisterTestData(2, @"aba[bab]xyz
xyx[xyx]xyx
aaa[kek]eke
zazbz[bzb]cdb",3);
        }
    }
}
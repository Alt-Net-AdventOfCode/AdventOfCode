using System;
using System.Text.RegularExpressions;

namespace AOCHelpers
{
    public abstract class DupdobDayBase
    {
        public void SetData(string input = null)
        {
            Parse(input ?? Input);
        }

        protected virtual void Parse(string input)
        {
            var index=0;
            foreach (var line in input.Split('\n'))
            {
                ParseLine(index++, line);
            }
        }

        protected abstract void ParseLine(int index, string line);
        
        public virtual void OutputAnswers()
        {
            SetData();
            Console.Write($"Day {Day}: {GiveAnswer1()}");
            Console.WriteLine($" & {GiveAnswer2()}");
        }
        
        public virtual object GiveAnswer1()
        {
            return "undef";
        }

        public virtual object GiveAnswer2()
        {
            return "undef";
        }
        
        protected static bool Extract(Regex regex, string line, out string val)
        {
            var match = regex.Match(line);
            if (!match.Success)
            {
                Console.WriteLine("Failed to parse line {0}.", line);
                val = null;
                return false;
            }

            val = match.Groups[1].Value;
            return true;
        }

        protected abstract string Input { get; }
        public abstract int Day { get; }

    }
}
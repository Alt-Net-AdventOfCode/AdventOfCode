﻿using System;
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
            foreach (var line in input.Split('\n'))
            {
                ParseLine(line);
            }
        }

        protected virtual void ParseLine(string line)
        {
            throw new NotImplementedException();
        }

        public virtual void OutputAnswers()
        {
            SetData();
            Console.WriteLine($"Day {Day}: {GiveAnswer1()} & {GiveAnswer2()}");
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
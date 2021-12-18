using System;

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
            var lines = input.Split('\n');
            // we discard the last line if it is empty (trailing newline), but we keep any internal newlines
            if (lines[^1].Length == 0)
            {
                lines = lines[0..^1];
            }
            var index=0;
            foreach (var line in lines)
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

        protected abstract string Input { get; }
        public abstract int Day { get; }
        public static DupdobDayBase BuildFromType(Type type)
        {
            return type.GetConstructor(Array.Empty<Type>())?.Invoke(Array.Empty<object>()) as DupdobDayBase;
        }
    }
}
namespace AOCHelpers
{
    public abstract class Algorithm
    {
        protected abstract void ParseLine(string line, int index, int lineCount);

        public virtual object GetAnswer1()
        {
            return null;
        }

        public virtual object GetAnswer2()
        {
            return null;
        }

        public abstract void SetupRun(DayEngine dayEngine);

        public void Parse(string data)
        {
            var lines = data.Split('\n');
            // we discard the last line if it is empty (trailing newline), but we keep any internal newlines
            if (lines[^1].Length == 0)
            {
                lines = lines[0..^1];
            }
            var index=0;
            foreach (var line in lines)
            {
                ParseLine(line, index++, lines.Length);
            }
        }
    }
}
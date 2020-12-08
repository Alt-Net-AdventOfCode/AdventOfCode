namespace AOCHelpers
{
    public static class DupdobStringExtensions
    {
        public static (string, string) SplitAtFirst(this string text, char separator)
        {
            var pos = text.IndexOf(separator);
            return (text.Substring(0, pos), text.Substring(pos+1));
        }
    }
}
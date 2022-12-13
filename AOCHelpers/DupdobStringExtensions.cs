using System.ComponentModel.DataAnnotations;

namespace AOCHelpers
{
    public static class DupdobStringExtensions
    {
        public static (string, string) SplitAtFirst(this string text, char separator)
        {
            var pos = text.IndexOf(separator);
            return (text[..pos], text[(pos+1)..]);
        }
    }
}
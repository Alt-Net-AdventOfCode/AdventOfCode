using System;
using System.ComponentModel.DataAnnotations;

namespace AOCHelpers
{
    public static class DupdobStringExtensions
    {
        public static (string, string) SplitAtFirst(this string text, char separator)
        {
            var pos = text.IndexOf(separator);
            return (text.Substring(0, pos), text.Substring(pos+1));
        }

        public static string ReplaceAll(this string text, string toReplace, string replacement, out int count)
        {
            var pos = 0;
            count = 0;
            do
            {
                pos = text.IndexOf(toReplace, pos, StringComparison.Ordinal);
                if (pos >= 0)
                {
                    count++;
                    pos += toReplace.Length;
                }
            } while (pos >=0);

            return text.Replace(toReplace, replacement, StringComparison.Ordinal);
        }
    }
}
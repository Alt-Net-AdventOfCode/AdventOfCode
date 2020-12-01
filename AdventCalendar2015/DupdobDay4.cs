using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AdventCalendar2015
{
    public class DupdobDay4
    {
        private readonly MD5 helper = MD5.Create();

        private string ComputeMd5(string test)
        {
            var hash = helper.ComputeHash(Encoding.UTF8.GetBytes(test));
            return string.Join("", hash.Select(x => x.ToString("x2")));
        }
        
        public int Md5Suffix(string input, string leading)
        {
            for (var suffix = 0; ; suffix++)
            {
                var test = $"{input}{suffix}";
                if (ComputeMd5(test).StartsWith(leading))
                    return suffix;
            }

            return -1;
        }
    }
}
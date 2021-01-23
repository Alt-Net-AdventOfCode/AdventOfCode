using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay4: DupdobDayWithTest
    {
        private readonly MD5 _helper = MD5.Create();

        public override object GiveAnswer1()
        {
            return Md5Suffix(Input, new string('0', 5));
        }

        public override object GiveAnswer2()
        {
            return Md5Suffix(Input, new string('0', 6));
        }

        private string ComputeMd5(string test)
        {
            var hash = _helper.ComputeHash(Encoding.UTF8.GetBytes(test));
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
        }

        protected override void ParseLine(int index, string line)
        {
        }

        protected override string Input => "bgvyzdsv";
        public override int Day => 4;
        protected override void SetupTestData(int id)
        {
        }

        protected override void SetupRunData()
        {
        }
    }
}
using System.Linq;
using System.Security.Cryptography;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay5:DupdobDayWithTest
    {
        private string _password;
        public override object GiveAnswer1()
        {
            var result = "";
            for (var index = 0; result.Length<8 ;index++)
            {
                var attempt = _password + index;
                var hash = MD5.HashData(System.Text.Encoding.ASCII.GetBytes(attempt));
                if (hash[0] != 0 || hash[1] != 0 || hash[2] > 15) continue;
                var hexa = hash[2];
                if (hexa < 10)
                {
                    result += (char)('0' + hexa);
                }
                else
                {
                    result += (char)('a' + hexa-10);
                }
            }

            return result;
        }

        public override object GiveAnswer2()
        {
            var result = new char[8];
            for (var index = 0; result.Any(c => c == 0); index++)
            {
                var attempt = _password + index;
                var hash = MD5.HashData(System.Text.Encoding.ASCII.GetBytes(attempt));
                if (hash[0] != 0 || hash[1] != 0 || hash[2] > 7 || result[hash[2]]!= 0) continue;
                var hexa = hash[3] >> 4;
                if (hexa < 10)
                {
                    result[hash[2]] = (char)('0' + hexa);
                }
                else
                {
                    result[hash[2]] = (char)('a' + hexa-10);
                }
            }

            return new string(result);        
        }

        protected override void ParseLine(int index, string line)
        {
            _password = line;
        }

        protected override string Input => "cxdnnyjw";
        public override int Day => 5;
        protected override void SetupTestData()
        {
            TestData = "abc";
            ExpectedResult1 = "18f47a30";
            ExpectedResult2 = "05ace8e3";
        }

        protected override void CleanUp()
        {
        }
    }
}
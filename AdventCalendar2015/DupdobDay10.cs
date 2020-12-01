using System.Text;

namespace AdventCalendar2015
{
    public class DupdobDay10
    {
        public void Parse(string input = Input)
        {
            value = input;
        }
        
        public int RepeatedLookAndSay(int repeat)
        {
            for (var i = 0; i < repeat; i++)
            {
                value = LookAndSay();
            }

            return value.Length;
        }

        private string LookAndSay()
        {
            var countRepeat = 0;
            var lastchar = ' ';
            var result = new StringBuilder();
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == lastchar)
                {
                    countRepeat++;
                }
                else
                {
                    if (countRepeat > 0)
                    {
                        result.Append($"{countRepeat}{lastchar}");
                    }
                    lastchar = value[i];
                    countRepeat = 1;
                }
            }
            result.Append($"{countRepeat}{lastchar}");
            return result.ToString();
        }

        private string value;
        private const string Input = "1113122113";
    }
}
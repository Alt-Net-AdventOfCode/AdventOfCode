namespace AdventCalendar2019.Day_4
{
    public class Dupdob_Day4
    {
        public bool IsValidPassword(int password, bool strenghten)
        {
            var test = password.ToString("0000000");
            var foundDouble = false;
            var previous = test[0];
            var countRepeating = 1;
            for (var i = 1; i < test.Length; i++)
            {
                if (test[i] < previous)
                {
                    return false;
                }

                if (test[i] == previous)
                {
                    countRepeating++;
                    if (!strenghten)
                    {
                        foundDouble = true;
                    }
                }
                else
                {
                    if (countRepeating == 2)
                    {
                        foundDouble = true;
                    }

                    countRepeating = 1;
                }

                previous = test[i];
            }
            return foundDouble || countRepeating == 2;
        }
        
        private const string Input =
@"";
    }
}
using System;

namespace AdventCalendar2019.Day_4
{
    public class DupdobDay4
    {
        private int _start;
        private int _end;
        
        public static void GiveAnswers()
        {
            var runner = new DupdobDay4();
            runner.ParseInput();
            Console.WriteLine("Answer 1: {0}.", runner.CountPasswords());
            Console.WriteLine("Answer 2: {0}.", runner.CountStrongerPasswords());
        }

        private void ParseInput(string input = Input)
        {
            var vals = input.Split('-');
            _start = int.Parse(vals[0]);
            _end = int.Parse(vals[1]);
        }

        private int CountPasswords()
        {
            var result = 0;
            for (var i = _start; i <= _end; i++)
            {
                if (IsValidPassword(i, false))
                {
                    result++;
                }
            }

            return result;
        }

        private int CountStrongerPasswords()
        {
            var result = 0;
            for (var i = _start; i <= _end; i++)
            {
                if (IsValidPassword(i, true))
                {
                    result++;
                }
            }

            return result;
        }

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

        private const string Input = "347312-805915";
    }
}
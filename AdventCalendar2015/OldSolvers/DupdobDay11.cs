using System;

namespace AdventCalendar2015
{
    internal class DupdobDay11
    {
        private string _input;
        private string _lastPassword;
        private const string Input = "vzbxkghb";

        public void Parse(string input = Input)
        {
            this._input = input;
        }

        private string NextPassword(string password)
        {
            for (var i = password.Length - 1; i >= 0; i--)
            {
                if (password[i] < 'z')
                {
                    return password.Substring(0, i) + (char)(password[i] + 1) + password.Substring(i+1);
                }
                else
                {
                    password = password.Substring(0, i) + new string('a', password.Length - i);
                }
            }
            throw new InvalidOperationException("cant find next password.");
        }

        private bool IsValid(string password)
        {
            if (password.IndexOfAny(new[] {'o', 'l', 'i'}) >= 0)
                return false;
            var lastChar = ' ';
            var nbRepeat = 0;
            var nbRepeated = 1;
            var straight = 1;
            var nbStraight = 0;
            password += ' ';
            for (var i = 0; i < password.Length; i++)
            {
                var car = password[i];
                if (car == lastChar)
                {
                    if (straight >= 3)
                    {
                        nbStraight++;
                    }
                    nbRepeated++;
                    straight = 1;
                }
                else
                {
                    if (nbRepeated > 1)
                    {
                        nbRepeat++;
                        nbRepeated = 1;
                    }

                    if (car == lastChar + 1)
                    {
                        straight++;
                    }
                    else
                    {
                        if (straight >= 3)
                        {
                            nbStraight++;
                        }
                        straight = 1;
                    }
                    lastChar = car;
                }
            }

            return nbRepeat >= 2 && nbStraight >= 1;
        }
        
        public string Compute1()
        {
            var password = NextPassword(this._input);
            while (!IsValid(password))
            {
                password = NextPassword(password);
            }

            this._lastPassword = password;
            return password;
        }

        public string Compute2()
        {
            var password = NextPassword(this._lastPassword);
            while (!IsValid(password))
            {
                password = NextPassword(password);
            }

            this._lastPassword = password;
            return password;           
        }
    }
}
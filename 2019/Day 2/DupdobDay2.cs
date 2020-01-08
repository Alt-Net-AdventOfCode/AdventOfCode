using System;
using System.Linq;

namespace AdventCalendar2019.Day_2
{
    public  class DupdobDay2
    {
        private int[] _opCodes;

        public static void GiveAnswers()
        {
            var runner = new DupdobDay2();
            runner.ParseInput();
            Console.WriteLine("Answer 1: {0}.", runner.ComputeAnswer());
            Console.WriteLine("Answer 2: {0}.", runner.ComputeAnswer2());
        }

        public int[] ParseInput(string input = Input)
        {
            _opCodes = input.Split(',').Select(int.Parse).ToArray();
            return _opCodes;
        }

        public int ComputeAnswer()
        {
            return Process(12, 2);
        }

        private int Process(int noun, int verb)
        {
            var opCodes = (int[])_opCodes.Clone();
            opCodes[1] = noun;
            opCodes[2] = verb;
            // patch
            for (var i = 0; i < opCodes.Length; i+=4)
            {
                switch (opCodes[i])
                {
                    case 1:
                        opCodes[opCodes[i + 3]] = opCodes[opCodes[i + 1]] + opCodes[opCodes[i+2]];
                        break;
                    case 2:
                        opCodes[opCodes[i + 3]] = opCodes[opCodes[i + 1]] * opCodes[opCodes[i+2]];
                        break;
                    case 99:
                        return opCodes[0];
                }
            }
            throw new ApplicationException("failed");
        }

        public long ComputeAnswer2()
        {
            for (int i = 0; i <= 99; i++)
            {
                for (int j = 0; j <= 99; j++)
                {
                    if (Process(i, j) == 19690720)
                    {
                        return 100 * i + j;
                    }
                }
            }

            throw new ApplicationException("failed");
        }

        private const string Input = 
@"1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,10,1,19,1,19,9,23,1,23,13,27,1,10,27,31,2,31,13,35,1,10,35,39,2,9,39,43,2,43,9,47,1,6,47,51,1,10,51,55,2,55,13,59,1,59,10,63,2,63,13,67,2,67,9,71,1,6,71,75,2,75,9,79,1,79,5,83,2,83,13,87,1,9,87,91,1,13,91,95,1,2,95,99,1,99,6,0,99,2,14,0,0";
    }
    
}
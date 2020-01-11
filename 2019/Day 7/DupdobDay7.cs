using System;
using System.Collections.Generic;
using AdventCalendar2019.Day_5;

namespace AdventCalendar2019.Day_7
{
    public class DupdobDay7
    {
 
        public static void GiveAnswers()
        {
            var runner = new DupdobDay7();
            runner.ParseInput();
            Console.WriteLine("Answer 1: {0}.", runner.FindMaxPower());
            Console.WriteLine("Answer 2: {0}.", runner.FindMaxPowerWithAlt());
        }
 
        public void ParseInput(string input = Input)
        {
            for (var i = 0; i < 5; i++)
            {
                _engines.Add(new DupdobDay5());
            }
            foreach (var engine in _engines)
            {
                engine.ParseInput(input);
            }
        }

        IEnumerable<IList<int>> GenerateCombinations(IList<int> collection)
        {
            if (collection.Count == 1)
            {
                yield return collection;
            }
            else for (var i = 0; i < collection.Count; i++)
            {
                var clone = new List<int>(collection);
                clone.RemoveAt(i);
                foreach (var combination in GenerateCombinations(clone))
                {
                    combination.Add(collection[i]);
                    yield return combination;
                }
            }
        }
        
        public int FindMaxPower()
        {
            var maxOutput = int.MinValue;
            var settings = new List<int>{0,1,2,3,4};
            List<int> maxSettings = null;
            foreach (var combination in GenerateCombinations(settings))
            {
                var result = GetThrustPower(combination);
                if (result <= maxOutput) continue;
                maxSettings = new List<int>(combination);
                maxOutput = result;
            }

            Console.WriteLine("Max settings: {0}", string.Join(',', maxSettings));
            return maxOutput;
        }

        public int FindMaxPowerWithAlt()
        {
            var maxOutput = int.MinValue;
            var settings = new List<int>{5,6,7,8,9};
            List<int> maxSettings = null;
            foreach (var combination in GenerateCombinations(settings))
            {
                var result = GetAltThrustPower(combination);
                if (result <= maxOutput) continue;
                maxSettings = new List<int>(combination);
                maxOutput = result;
            }

            Console.WriteLine("Max settings: {0}", string.Join(',', maxSettings));
            return maxOutput;
        }

        private int GetThrustPower(IList<int> settings)
        {
            var power = 0;
            for (var i = 0; i < _engines.Count; i++)
            {
                _engines[i].RunProgram((index) => index == 0 ? settings[i] : power, (value) =>
                {
                    power = value;
                    return true;
                });
            }
            return power;
        }

        private int GetAltThrustPower(IList<int> settings)
        {
            var power = 0;
            
            for (var i = 0; i < _engines.Count; i++)
            {
                _engines[i].RunProgram((index) => index == 0 ? settings[i] : power, (value) =>
                {
                    power = value;
                    return false;
                });
            }

            var stop = false;
            do
            {
                for (var i = 0; i < _engines.Count; i++)
                {
                    _engines[i].ContinueProgram((index) => index == 0 ? settings[i] : power, (value) =>
                    {
                        power = value;
                        return true;
                    });
                    stop |= _engines[i].Halted;
                }                
            } while (!stop);
            return power;
        }

        private readonly List<DupdobDay5> _engines = new List<DupdobDay5>();

        private const string Input =
@"3,8,1001,8,10,8,105,1,0,0,21,34,51,64,73,98,179,260,341,422,99999,3,9,102,4,9,9,1001,9,4,9,4,9,99,3,9,1001,9,4,9,1002,9,3,9,1001,9,5,9,4,9,99,3,9,101,5,9,9,102,5,9,9,4,9,99,3,9,101,5,9,9,4,9,99,3,9,1002,9,5,9,1001,9,3,9,102,2,9,9,101,5,9,9,1002,9,2,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,99,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,99";

    }
}
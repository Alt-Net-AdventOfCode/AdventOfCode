using System;
using System.Collections.Generic;

namespace AOCHelpers
{
    public abstract class DupdobDayWithTest: DupdobDayBase
    {
        protected virtual void SetupTestData()
        {
        }

        protected abstract void CleanUp();
        
        public override void OutputAnswers()
        {
            SetupTestData();
            if (!RunTests()) return;
            
            base.OutputAnswers();
        }

        protected virtual IEnumerable<(string intput, object result)> GetTestData1()
        {
            if (ExpectedResult1 == null)
                yield break;
            if (TestData == null)
            {
                throw new InvalidOperationException("Define _testData.");
            }
            yield return (TestData, ExpectedResult1);
        }
        
        protected virtual IEnumerable<(string intput, object result)> GetTestData2()
        {
            if (ExpectedResult2 == null)
                yield break;
            if (TestData == null)
            {
                throw new InvalidOperationException("Define _testData.");
            }
            yield return (TestData, ExpectedResult2);
        }
        
        private bool RunTests()
        {
            foreach (var (data, expected) in GetTestData1())
            {
                Parse(data);
                var answer = GiveAnswer1();
                if (Compare(answer, expected))
                {
                    Console.WriteLine($"Correct answer 1: {answer} (for {data}).");
                    CleanUp();
                    continue;
                }
                Console.WriteLine($"Incorrect answer 1: {answer}, expected {expected} (for {data}).");
                return false;
            }

            foreach (var (data, expected) in GetTestData2())
            {
                Parse(data);
                var answer = GiveAnswer2();
                if (Compare(answer, expected))
                {
                    Console.WriteLine($"Correct answer 2: {answer} (for {data}).");
                    CleanUp();
                    continue;
                }
                Console.WriteLine($"Incorrect answer 2: {answer}, expected {expected} (for {data}).");
                return false;
            }

            return true;
        }

        private static bool Compare(object a, object b)
        {
            return a switch
            {
                int anInt => anInt == (int) b,
                long aLong => aLong == (long) b,
                string aString => aString == (string) b,
                _ => a == b
            };
        }

        protected string TestData;
        protected object ExpectedResult1;
        protected object ExpectedResult2;
    }
}
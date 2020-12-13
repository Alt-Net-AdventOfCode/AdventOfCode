using System;

namespace AOCHelpers
{
    public abstract class DupdobDayWithTest: DupdobDayBase
    {
        protected abstract void SetupTestData();
        protected abstract void SetupRunData();
        
        public override void OutputAnswers()
        {
            SetupTestData();
            if (_expectedResult1 != null)
            {
                if (_testData == null)
                {
                    throw new InvalidOperationException("Define _testData.");
                }
                Parse(_testData);

                var answer = GiveAnswer1();
                if (!Compare(answer, _expectedResult1))
                {
                    throw new InvalidOperationException(_expectedResult1 == null ? "Define _expectedResult1." : $"Incorrect answer : {answer}, expected {_expectedResult1}.");
                } 
                if (_expectedResult2 != null)
                {
                    answer = GiveAnswer2();
                    if (!Compare(answer, _expectedResult2))
                    {
                        throw new InvalidOperationException(_expectedResult1 == null
                            ? "Define _expectedResult2."
                            : $"Incorrect answer : {answer}, expected {_expectedResult2}.");
                    }
                }
            }
            
            SetupRunData();
            base.OutputAnswers();
        }

        private bool Compare(object a, object b)
        {
            if (a is int anInt)
            {
                return anInt == (int) b;
            }
            if (a is long aLong)
            {
                return aLong == (long) b;
            }

            if (a is string aString)
            {
                return aString == (string) b;
            }

            return a == b;
        }

        protected string _testData;
        protected object _expectedResult1;
        protected object _expectedResult2;
    }
}
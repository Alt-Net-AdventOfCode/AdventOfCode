using System;

namespace AOCHelpers
{
    public abstract class DupdobDayWithTest: DupdobDayBase
    {
        protected abstract void SetupTestData(int id);
        protected abstract void SetupRunData();
        
        public override void OutputAnswers()
        {
            SetupTestData(1);
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
                    SetupTestData(1);
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
            return a switch
            {
                int anInt => anInt == (int) b,
                long aLong => aLong == (long) b,
                string aString => aString == (string) b,
                _ => a == b
            };
        }

        protected string _testData;
        protected object _expectedResult1;
        protected object _expectedResult2;
    }
}
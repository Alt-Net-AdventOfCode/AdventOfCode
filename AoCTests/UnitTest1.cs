using NFluent;
using NUnit.Framework;

namespace AoC.AoCTests
{
    public class EngineShould
    {
        [Test]
        public void SetUpDataProperly()
        {
            var fakeClient = new AoCFakeClient(2015);
            var engine = new Engine(2015, fakeClient);
            
            engine.RunDay( ()=> new Algo(10, null, null));

            Check.That(engine.Day).IsEqualTo(10);
        }
        
        class Algo : ISolver
        {
            private readonly int _day;
            private readonly object _answer1;
            private readonly object _answer2;

            public Algo(int day, object answer1, object answer2)
            {
                _day = day;
                _answer1 = answer1;
                _answer2 = answer2;
            }

            public void SetupRun(Engine engine)
            {
                engine.Day = _day;
            }

            public object GetAnswer1(string data) => _answer1;

            public object GetAnswer2(string data) => _answer2;
        }
    }
}
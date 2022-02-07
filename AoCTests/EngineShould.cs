using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text.RegularExpressions;
using NFluent;
using NFluent.Mocks;
using NUnit.Framework;

namespace AoC.AoCTests
{
    public class EngineShould
    {

        private MockFileSystem GetFileSystem()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.Directory.SetCurrentDirectory(Directory.GetCurrentDirectory());
            return mockFileSystem;
        }
        
        [Test]
        public void SetUpEverythingProperly()
        {
            var mockFileSystem = GetFileSystem();
            var fakeClient = new AoCFakeClient(2015);

            var testInputData = "Silly input data";
            fakeClient.SetInputData(testInputData);
            var engine = new Engine(2015, fakeClient, mockFileSystem);
            var algo = new FakeSolver(10, null, null);
            using var console = new CaptureConsole();
            engine.RunDay( ()=> algo);
            // verify the day is properly ste yp
            Check.That(engine.Day).IsEqualTo(10);
            // it should request the first answer
            Check.That(algo.GetAnswer1Calls).IsEqualTo(1);
            // and not the second
            Check.That(algo.GetAnswer2Calls).IsEqualTo(0);
            // it should say that no answer was provided
            Check.That(console.Output).Contains("No answer provided");
            // it should have received the provided input data
            Check.That(algo.InputData).IsEqualTo(testInputData);
            // it should have cached the input data
            Check.That(mockFileSystem.AllFiles.Any(p => Regex.IsMatch(p, "Input.*\\.txt")));
        }
        
        [Test]
        public void HandlerWrongAnswer()
        {
            var fakeClient = new AoCFakeClient(2015);
            var mockFileSystem = GetFileSystem();
            using var console = new CaptureConsole();
          
            fakeClient.SetAnswerResponseFilename("WrongAnswer.html");
            var engine = new Engine(2015, fakeClient, mockFileSystem);
            var algo = new FakeSolver(10, 58, null);
            engine.RunDay( ()=> algo);

            Check.That(algo.GetAnswer1Calls).IsEqualTo(1);
            Check.That(algo.GetAnswer2Calls).IsEqualTo(0);

            Check.That(console.Output.Contains("AoC site response"));
        }
        
        [Test]
        public void HandlerGoodAnswer()
        {
            var fakeClient = new AoCFakeClient(2015);
            using var console = new CaptureConsole();
            var mockFileSystem = GetFileSystem();
            
            fakeClient.SetAnswerResponseFilename("GoodAnswer.html");
            var engine = new Engine(2015, fakeClient, mockFileSystem);
            var algo = new FakeSolver(10, 12, null);
            engine.RunDay( ()=> algo);

            Check.That(algo.GetAnswer1Calls).IsEqualTo(1);
            Check.That(algo.GetAnswer2Calls).IsEqualTo(1);

            Check.That(console.Output.Contains("AoC site response"));
        }
        
        class FakeSolver : ISolver
        {
            private readonly int _day;
            private readonly object _answer1;
            private readonly object _answer2;
            
            public int GetAnswer1Calls { get; private set; }
            public int GetAnswer2Calls { get; private set; }
            
            public string InputData { get; private set; }

            public FakeSolver(int day, object answer1, object answer2)
            {
                _day = day;
                _answer1 = answer1;
                _answer2 = answer2;
            }

            public void SetupRun(Engine engine)
            {
                engine.Day = _day;
            }

            public object GetAnswer1(string data)
            {
                InputData = data;
                GetAnswer1Calls++;
                return _answer1;
            }

            public object GetAnswer2(string data)
            {
                GetAnswer2Calls++;
                return _answer2;
            }
        }
    }
}
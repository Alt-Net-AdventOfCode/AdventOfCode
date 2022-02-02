using System.IO;
using System.Threading.Tasks;

namespace AoC.AoCTests
{
    public class AoCFakeClient : AoCClientBase
    {
        private string _inputData;
        private string _responseFile;
        
        public AoCFakeClient(int year) : base(year)
        {
        }

        public void SetInputData(string data)
        {
            _inputData = data;
        }

        public void SetAnswerResponseFilename(string fileName)
        {
            _responseFile = fileName;
        }
        
        public override Task<string> RequestPersonalInput() => Task.FromResult(_inputData);

        public override Task<string> PostAnswer(int question, string value) => File.ReadAllTextAsync(_responseFile);
        public override void Dispose()
        {
            
        }
    }
}
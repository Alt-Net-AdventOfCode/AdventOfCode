using System.Linq;
using AoC;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay9: SolverWithLineParser
    {
        private string _compressedData;

        protected override void ParseLine(string line, int index, int lineCount)
        {
            _compressedData = line;
        }

        public override object GetAnswer1()
        {
            var index = 0;
            var answer = 0;
            while (index<_compressedData.Length)
            {
                if (_compressedData[index] == '(')
                {
                    index++;
                    var end = _compressedData.IndexOf(')', index);
                    var repeatFormula = _compressedData.Substring(index, end - index);
                    var values = repeatFormula.Split('x').Select(int.Parse).ToList();
                    index = end + 1;
                    index += values[0];
                    answer += values[0] * values[1];
                }
                else
                {
                    answer++;
                    index++;
                }
            }

            return answer;
        }

        public override object GetAnswer2()
        {
            var compressedData = _compressedData;
            return ComputeDecompressedLen(compressedData);
        }

        private static long ComputeDecompressedLen(string compressedData)
        {
            var index = 0;
            var answer = 0L;
            while (index < compressedData.Length)
            {
                if (compressedData[index] == '(')
                {
                    index++;
                    var end = compressedData.IndexOf(')', index);
                    var repeatFormula = compressedData.Substring(index, end - index);
                    var values = repeatFormula.Split('x').Select(int.Parse).ToList();
                    index = end + 1;
                    var bloc = compressedData.Substring(index, values[0]);
                    index += values[0];
                    answer += ComputeDecompressedLen(bloc) * values[1];
                }
                else
                {
                    answer++;
                    index++;
                }
            }

            return answer;
        }

        public override void SetupRun(Engine engine)
        {
            engine.Day = 9;
            engine.RegisterTestDataAndResult("ADVENT", 6, 1);
            engine.RegisterTestDataAndResult("A(1x5)BC", 7, 1);
            engine.RegisterTestDataAndResult("(3x3)XYZ", 9, 1);
            engine.RegisterTestDataAndResult("A(2x2)BCD(2x2)EFG", 11, 1);
            engine.RegisterTestDataAndResult("(6x1)(1x3)A", 6, 1);
            engine.RegisterTestDataAndResult("X(8x2)(3x3)ABCY", 18, 1);
        }
    }
}
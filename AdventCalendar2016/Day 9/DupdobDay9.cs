using System.Linq;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay9: Algorithm
    {
        private string _compressedData;
        
        public override void ParseLine(string line, int index, int lineCount)
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

        public override void SetupRun(DayEngine dayEngine)
        {
            dayEngine.Day = 9;
            dayEngine.RegisterTestData(1, "ADVENT", 6);
            dayEngine.RegisterTestData(1, "A(1x5)BC", 7);
            dayEngine.RegisterTestData(1, "(3x3)XYZ", 9);
            dayEngine.RegisterTestData(1, "A(2x2)BCD(2x2)EFG", 11);
            dayEngine.RegisterTestData(1, "(6x1)(1x3)A", 6);
            dayEngine.RegisterTestData(1, "X(8x2)(3x3)ABCY", 18);
        }
    }
}
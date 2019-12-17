using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2019.Day16
{
    public class DupdobDay16
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay16();
            runner.ParseInput();
            Console.WriteLine("Answer 1: {0}", runner.ComputeRuns(100));
            Console.WriteLine("Answer 2: {0}", runner.ComputeRunsOnMultipleInputs(100, 10000));
        }

        private void ParseInput(string input = Input)
        {
            _data = input.Select(c => int.Parse(c.ToString())).ToList();
        }

        private int GetRange(int start, int len, int maxLength)
        {
            var startIndex = start % maxLength;
            var result = 0;
            result += (startIndex == 0) ? 0 : - _cumulated[startIndex - 1];
            var endIndex = Math.Min(startIndex + len, maxLength)-1;
            result += _cumulated[endIndex];
            return result;
        }

        private string ComputeRuns(int nbRuns)
        {
            var toProcess = _data.ToArray();
            for (var i = 0; i < nbRuns; i++)
            {
                var cumul = 0;
                _cumulated = new List<int>();
                foreach (var t in toProcess)
                {
                    cumul += t;
                    _cumulated.Add(cumul);
                }
                
                var newData = new int[toProcess.Length];
                for (var j = 0; j < toProcess.Length; j++)
                {
                    var digit = 0;
                    var mode = -1;
                    for (var k = -1; k < toProcess.Length; k+=j+1)
                    {
                        mode++;
                        mode %= 4;
                        if (mode % 2 == 0)
                        {
                            continue;
                        }

                        if (mode == 1)
                        {
                            digit += GetRange(k, j + 1, toProcess.Length);
                        }
                        else
                        {
                            digit -= GetRange(k, j + 1, toProcess.Length);
                        }
                    }

                    newData[j] = Math.Abs(digit) % 10;
                }

                toProcess = newData;
            }

            return toProcess.Take(8).Select(x => x.ToString()).Aggregate((s, s1) => s+s1);
        }

        private string ComputeRunsOnMultipleInputs(int nbRuns, int repeats)
        {
            var buffer = new List<int>();
            for (var i = 0; i < repeats; i++)
            {
                buffer.AddRange(_data);
            }

            var offset = 0;
            for (var scan = 0; scan < 7; scan++)
            {
                offset *= 10;
                offset += buffer[scan];
            }
            var toProcess = buffer.ToArray();
            for (var i = 0; i < nbRuns; i++)
            {
                var newData = new int[toProcess.Length];
                var cumul = 0;
                _cumulated = new List<int>();
                foreach (var t in toProcess)
                {
                    cumul += t;
                    _cumulated.Add(cumul);
                }

                for (var j = 0; j < toProcess.Length; j++)
                {
                    var digit = 0;
                    var mode = -1;
                    for (var k = -1; k < toProcess.Length; k+=j+1)
                    {
                        mode++;
                        mode %= 4;
                        if (mode % 2 == 0)
                        {
                            continue;
                        }

                        if (mode == 1)
                        {
                            digit += GetRange(k, j + 1, toProcess.Length);
                        }
                        else
                        {
                            digit -= GetRange(k, j + 1, toProcess.Length);
                        }
                    }

                    newData[j] = Math.Abs(digit) % 10;
                }

                toProcess = newData;
            }

            return toProcess.Skip(offset).Take(8).Select(x => x.ToString()).Aggregate((s, s1) => s+s1);
        }

        private const string Input = 
            @"59782619540402316074783022180346847593683757122943307667976220344797950034514416918778776585040527955353805734321825495534399127207245390950629733658814914072657145711801385002282630494752854444244301169223921275844497892361271504096167480707096198155369207586705067956112600088460634830206233130995298022405587358756907593027694240400890003211841796487770173357003673931768403098808243977129249867076581200289745279553289300165042557391962340424462139799923966162395369050372874851854914571896058891964384077773019120993386024960845623120768409036628948085303152029722788889436708810209513982988162590896085150414396795104755977641352501522955134675";

        private List<int> _data;

        private List<int> _cumulated = new List<int>();
    }
}
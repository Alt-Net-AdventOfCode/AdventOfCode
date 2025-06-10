// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
// ---
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2023;

public class DupdobDay15 : SolverWithLineParser
{
    private string? _line;
    
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 15;
        dayAutomatonBase.RegisterTestDataAndResult("rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7", 1320, 1);
        dayAutomatonBase.RegisterTestResult(145, 2);
    }

    public override object GetAnswer1() => string.IsNullOrEmpty(_line) ? 0 : _line.Split(',', StringSplitOptions.RemoveEmptyEntries).Sum(part => ComputeHash(part));

    private static int ComputeHash(string part)
    {
        var hash = 0;
        foreach (var letter in part)
        {
            hash += letter;
            hash *= 17;
            hash &= 0xFF;
        }

        return hash;
    }

    private class Box
    {
        private readonly Dictionary<string, int> _focalLengths = new();
        private readonly List<string> _labels = new();

        public void Remove(string label)
        {
            if (!_focalLengths.ContainsKey(label)) return;
            _focalLengths.Remove(label);
            _labels.Remove(label);
        }

        public void Replace(string label, int focal)
        {
            if (_focalLengths.ContainsKey(label))
            {
                _focalLengths[label] = focal;
                return;
            }
            _labels.Add(label);
            _focalLengths.Add(label, focal);
        }

        public int FocalPower()
        {
            var result = 0;
            for(var i =0; i<_labels.Count; i++)
            {
                result += (i + 1) * _focalLengths[_labels[i]];
            }

            return result;
        }
    }
    
    public override object GetAnswer2()
    {
        var boxes = new Dictionary<int, Box>();
        foreach (var operation in _line!.Split(','))
        {
            if (operation.Last()=='-')
            {
                // this is a removal
                var label = operation.Split('-')[0];
                var hash = ComputeHash(label);
                if (boxes.TryGetValue(hash, out var box))
                {
                    box.Remove(label);
                }
            }
            else
            {
                var parts = operation.Split('=');
                var hash = ComputeHash(parts[0]);
                if (!boxes.TryGetValue(hash, out var box))
                {
                    box = new Box();
                    boxes.Add(hash, box);
                }

                box.Replace(parts[0], int.Parse(parts[1]));
            }
        }

        var result = 0L;
        foreach (var i in boxes.Keys)
        {
            result += (i + 1) * boxes[i].FocalPower();
        }

        return result;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        _line += line;
    }
}
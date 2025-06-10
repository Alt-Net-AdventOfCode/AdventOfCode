// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
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

using System.Security.AccessControl;

namespace AdventCalendar2017;
using AoC;

public class DupdobDay06: SolverWithDataAsLines
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 6;
        dayAutomatonBase.RegisterTestDataAndResult("0\t2\t7\t0", 5, 1).RegisterTestResult(4,2);
    }

    public override object GetAnswer1()
    {
        var buffer = new Banks(_banks.ToArray());
        var seen = new HashSet<Banks>();
        var step = 0;
        while (seen.Add(buffer))
        {
            buffer = buffer.Distribute();
            step++;
        }

        return step;
    }

    public override object GetAnswer2()
    {
        var buffer = new Banks(_banks.ToArray());
        var seen = new HashSet<Banks>();
        while (seen.Add(buffer))
        {
            buffer = buffer.Distribute();
        }
        var step = 1;
        var reference = new Banks(buffer);
        buffer = buffer.Distribute();
        while (buffer != reference)
        {
            buffer = buffer.Distribute();
            step++;
        }
        return step;
    }

    private List<int> _banks = null;
    
    private class Banks : IEquatable<Banks>
    {
        private readonly int[] _banks;

        public Banks(int[] banks)
        {
            _banks = banks;
        }

        public Banks(Banks other)
        {
            _banks = other._banks.ToArray();
        }
        
        public Banks Distribute()
        {
            var index = 0;
            for (var i = 1; i < _banks.Length; i++)
            {
                if (_banks[i] > _banks[index])
                {
                    index = i;
                }
            }

            var stock = _banks[index];
            var newBanks = _banks.ToArray();
            newBanks[index] = 0;
            while (stock>0)
            {
                index = (index + 1) % newBanks.Length;
                newBanks[index]++;
                stock--;
            }

            return new Banks(newBanks);
        }
        
        public bool Equals(Banks? other)
        {
            return other!= null && _banks.SequenceEqual(other._banks);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Banks)obj);
        }

        public override int GetHashCode()
        {
            var hash = _banks[0];
            for (var i = 1; i < _banks.Length; i++)
            {
                hash = hash * 17 + _banks[i];
            }

            return hash;
        }

        public static bool operator ==(Banks? left, Banks? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Banks? left, Banks? right)
        {
            return !Equals(left, right);
        }

    }
    protected override void ParseLines(string[] lines)
    {
        _banks = lines[0].Split('\t', StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
    }
}
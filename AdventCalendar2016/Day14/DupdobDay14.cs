// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AoC;

namespace AdventCalendar2016
{
    
    public class DupdobDay14 : SolverWithLineParser
    {
        private string _salt;
        public override void SetupRun(Automaton automaton)
        {
            automaton.Day = 14;
            automaton.RegisterTestDataAndResult("abc\n", 22728, 1);
            automaton.RegisterTestDataAndResult("abc\n", 22551, 2);
        }

        public override object GetAnswer1()
        {
            return ComputeOneTimePad(GetHashForIndex);
        }

        private static object ComputeOneTimePad(Func<int, string> hashFunction)
        {
            var oneTimePads = new List<int>();
            var cache = new Dictionary<int, string>();

            string CachedHash(int index)
            {
                if (!cache.ContainsKey(index))
                {
                    cache[index] = hashFunction(index);
                }

                return cache[index];
            }
            
            for (var index = 0; index < int.MaxValue; index++)
            {
                var hexaHash = CachedHash(index);
                var triplet = ' ';
                for (var i = 0; i < hexaHash.Length - 2; i++)
                {
                    if (hexaHash[i] != hexaHash[i + 1] || hexaHash[i] != hexaHash[i + 2]) continue;
                    triplet = hexaHash[i];
                    break;
                }

                if (triplet == ' ')
                {
                    continue;
                }

                var confirmation = new string(triplet, 5);
                for (var i = index + 1; i < index + 1001; i++)
                {
                    if (CachedHash(i).Contains(confirmation))
                    {
                        oneTimePads.Add(index);
                        if (oneTimePads.Count == 64)
                        {
                            return index;
                        }

                        break;
                    }
                }
            }

            return -1;
        }

        private string GetHashForIndex(int index)
        {
            var hash = MD5.HashData(Encoding.ASCII.GetBytes(_salt + index));
            return Convert.ToHexString(hash).ToLower();
        }

        private string Get2016HashForIndex(int index)
        {
            var hash = MD5.HashData(Encoding.ASCII.GetBytes(_salt + index));
            for (var i = 0; i < 2016; i++)
            {
                var hexString = Convert.ToHexString(hash).ToLower();
                hash = MD5.HashData(Encoding.ASCII.GetBytes( hexString));
            }
            return Convert.ToHexString(hash).ToLower();
        }

        public override object GetAnswer2()
        {
            return ComputeOneTimePad(Get2016HashForIndex);
        }

        protected override void ParseLine(string line, int index, int lineCount)
        {
            if (index == 0)
            {
                _salt = line;
            }
        }
    }
}
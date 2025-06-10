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

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AoC;

namespace AdventCalendar2016
{
    public class DupdobDay17 : SolverWithLineParser
    {
        public override void SetupRun(DayAutomaton dayAutomaton)
        {
            dayAutomaton.Day = 17;

            dayAutomaton.AddExample("ihgpwlah");
            dayAutomaton.RegisterTestResult("DDRRRD", 1);
            dayAutomaton.RegisterTestResult(370, 2);
            dayAutomaton.AddExample("kglvqrro");
            dayAutomaton.RegisterTestResult("DDUDRLRRUDRD", 1);
            dayAutomaton.RegisterTestResult(492,2);
            
        }
        private static readonly ((int dx, int dy) vector, char letter) [] Rules = { ((0, -1), 'U'), ((0, 1), 'D'), ((-1, 0), 'L'), ((1, 0),'R') };

        public override object GetAnswer1()
        {
            (int x, int y) start = (0, 0);
            var queue = new List<((int x, int y), string passkey, int distance)> { ((start), _passkey, 0) };
            string passkey;
            while (true)
            {
                var index = 0;
                var currentDistance = queue[index].distance;
                for (var i = 1; i < queue.Count; i++)
                {
                    if (queue[index].distance >= currentDistance)
                    {
                        continue;
                    }
                    index = i;
                    currentDistance = queue[index].distance;
                }
                (var pos, passkey, _) = queue[index];
                queue.RemoveAt(index);
                if (pos == (3, 3))
                {
                    break;
                }
                var state = PasskeyToHexa(passkey);
                for (var i = 0; i < 4; i++)
                {
                    (int x, int y) newPos = (pos.x + Rules[i].vector.dx, pos.y + Rules[i].vector.dy);
                    if (newPos.x < 0 || newPos.y < 0 || newPos.x > 3 || newPos.y > 3)
                    {
                        continue;
                    }

                    if (state[i] < 'B' || state[i] > 'F' ) continue;
                    // door is open and we found a closer path
                    queue.Add((newPos, passkey+Rules[i].letter, currentDistance+1));
                }
            }
            
            return passkey[_passkey.Length..];  
        }

        private static string PasskeyToHexa(string startingString)
        {
            var hash = MD5.HashData(Encoding.ASCII.GetBytes(startingString));
            return hash[0].ToString("X2") + hash[1].ToString("X2");
        }
        
        public override object GetAnswer2()
        {
            (int x, int y) start = (0, 0);
            var queue = new Queue<((int x, int y), string passkey, int distance)>();
            queue.Enqueue(((start), _passkey, 0));
            var longestPath = 0;
            while (queue.Count>0)
            {
                var (pos, passkey, currentDistance) = queue.Dequeue();
                var state = PasskeyToHexa(passkey);
                for (var i = 0; i < 4; i++)
                {
                    (int x, int y) newPos = (pos.x + Rules[i].vector.dx, pos.y + Rules[i].vector.dy);
                    if (newPos.x < 0 || newPos.y < 0 || newPos.x > 3 || newPos.y > 3)
                    {
                        continue;
                    }

                    if (state[i] < 'B' || state[i] > 'F' ) continue;
                    if (newPos == (3, 3))
                    {
                        if (longestPath < currentDistance + 1)
                        {
                            longestPath = currentDistance + 1;
                        }
                        continue;
                    }
                    // door is open and we found a closer path
                    queue.Enqueue((newPos, passkey+Rules[i].letter, currentDistance+1));
                }
            }
            
            return longestPath;
        }

        private string _passkey;
        protected override void ParseLine(string line, int index, int lineCount)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                _passkey = line;
            }
        }
    }
}
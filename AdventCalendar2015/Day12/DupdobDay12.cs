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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AoC;

namespace AdventCalendar2015;

[Day(12)]
public class DupdobDay12 : SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomaton)
    {
    }

    protected override void Parse(string data)
    {
        _object = JSon.ParseIt(data);
    }

    [Example("{\"a\":2,\"b\":4}", 6)]
    [Example("{\"a\":[-1,1]}", 0)]
    [Example("[-1,{\"a\":1}]", 0)]
    public override object GetAnswer1() => _object.Sum();

    public override object GetAnswer2() => _object.Sum("red");

    private JSon _object;
    
    private class JSon
    {
        // default is 'null' JSon
        protected virtual void Parse(StringReader input, char first) => input.Read();

        public virtual int Sum(string exception = "") => 0;

        public static JSon ParseIt(string input)
        {
            var stream = new StringReader(input);
            return Build(stream);
        }

        protected static JSon Build(StringReader stream)
        {
            var next = (char)stream.Read();
            var host = next switch
            {
                '"' => new JSonString(),
                ',' => new JSon(),
                '[' => new JSonArray(),
                '{' => new JSonObject(),
                _ => new JSonInt()
            };

            host.Parse(stream, next);

            return host;
        }
    }
    
    private class JSonInt: JSon
    {
        private int _value;
        
        public override int Sum(string exception = "") => _value;

        protected override void Parse(StringReader input, char first)
        {
            var sign = 1;
            if (first == '-')
            {
                sign = -1;
                first = (char) input.Read();
            }
            _value = first-'0';
            while (true)
            {
                var next = input.Peek();
                if (next is < '0' or > '9')
                {
                    _value = _value * sign;
                    return;
                }

                input.Read();
                _value = _value * 10 + (next - '0');
            }
        }
    }
    
    private class JSonString: JSon
    {
        public string Value { get; private set; }

        protected override void Parse(StringReader input, char _)
        {
            var build = new StringBuilder();
            while (true)
            {
                var next = input.Read();
                if (next == '"')
                {
                    Value = build.ToString();
                    return;
                }

                build.Append((char)next);
            }
        }
    }
    
    private class JSonArray: JSon
    {
        private readonly List<JSon> _value = [];

        public override int Sum(string exception = "") => _value.Sum(son => son.Sum(exception));

        protected override void Parse(StringReader input, char _)
        {
            var peek = input.Peek();
            // check if this is an empty object
            if (peek == ']')
            {
                input.Read();
                return;
            }
            while (true)
            {
                _value.Add(Build(input));
                var token = input.Read();
                if (token == ']')
                {
                    return;
                }
                // it should be a comma
            }
        }
    }
    private class JSonObject: JSon
    {
        private readonly Dictionary<string, JSon> _value = [];

        public override int Sum(string exception = "") => _value.Values.Any(v => v is JSonString text && text.Value == exception) ? 0 : _value.Values.Sum(son => son.Sum(exception));

        protected override void Parse(StringReader input, char _)
        {
            var peek = input.Peek();
            if (peek == '}')
            {
                return;
            }

            while (true)
            {
                if (Build(input) is not JSonString key)
                {
                    return;
                }
                if (input.Read() != ':')
                {
                    return;
                }

                var value = Build(input);
                _value[key.Value] = value;
                var token = input.Read();
                if (token == '}')
                {
                    return;
                }
                // it should be a comma
            }
        }
    }
}
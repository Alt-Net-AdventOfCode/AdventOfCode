using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace AdventCalendar2021
{
    public class DupdobDay24 : AdvancedDay
    {
        private class State
        {
            private int _x;
            private int _y;
            private int _z;
            private int _w;
            private readonly string _input;
            public bool Stop { get; private set; }

            private int Cursor { get; set; } = 0;

            public State(string input)
            {
                this._input = input;
            }

            public int GetVar(string name) => name switch { "x" => _x, "y" => _y, "z" => _z, "w" => _w, _ => int.Parse(name) };

            public void SetVar(string name, int value)
            {
                switch (name)
                {
                    case "x":
                        _x = value;
                        break;
                    case "y":
                        _y = value;
                        break;
                    case "z":
                        _z = value;
                        break;
                    case "w":
                        _w = value;
                        break;
                }
            }

            public int GetInput()
            {
                if (Cursor == _input.Length)
                {
                    Stop = true;
                    return -1;
                }
                var result = _input[Cursor++] - '0';
                return result;
            }

            public bool Ok => _z == 0;
        }

        private readonly List<Action<State>> _program = new ();
        public DupdobDay24() : base(24)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var fields = line.Split(' ');
            switch (fields[0])
            {
                case "inp":
                    _program.Add(s => s.SetVar( fields[1], s.GetInput()));
                    break;
                case "add":
                    _program.Add(s => s.SetVar(fields[1], s.GetVar(fields[1]) + s.GetVar(fields[2])));
                    break;
                case "mul":
                    _program.Add(s => s.SetVar(fields[1], s.GetVar(fields[1]) * s.GetVar(fields[2])));
                    break;
                case "div":
                    _program.Add(s => s.SetVar(fields[1], s.GetVar(fields[1]) / s.GetVar(fields[2])));
                    break;
                case "mod":
                    _program.Add(s => s.SetVar(fields[1], s.GetVar(fields[1]) % s.GetVar(fields[2])));
                    break;
                case "eql":
                    _program.Add(s => s.SetVar(fields[1], s.GetVar(fields[1]) == s.GetVar(fields[2]) ? 1 : 0));
                    break;
            }
        }

        protected override void CleanUp()
        {
          _program.Clear();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield break;
        }

        public override object GiveAnswer1()
        {
            var increment = 1;
            for (var i = 1; i < 10; i++)
            {
                var start = i;
                var text = start.ToString();
                if (text.Contains('0'))
                {
                    continue;
                }
                var state = new State(text);
                foreach (var action in _program)
                {
                    action(state);
                    if (state.Stop)
                    {
                        break;
                    }
                }

                increment *= 10;
            }

            return 0;
        }
        // 
    }
}
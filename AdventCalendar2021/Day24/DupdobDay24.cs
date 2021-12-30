using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace AdventCalendar2021
{
    public class DupdobDay24 : AdvancedDay
    {
        private class State
        {
            public int X { get; private set; }
            private int _y;
            public int Z { get; private set; }
            private int _w;
            private readonly string _input;
            public bool Stop { get; private set; }

            private int Cursor { get; set; } = 0;

            public State(string input)
            {
                this._input = input;
            }

            public int GetVar(string name) => name switch { "x" => X, "y" => _y, "z" => Z, "w" => _w, _ => int.Parse(name) };

            public void SetVar(string name, int value)
            {
                switch (name)
                {
                    case "x":
                        X = value;
                        break;
                    case "y":
                        _y = value;
                        break;
                    case "z":
                        Z = value;
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

            public bool Ok => Z == 0;
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

        // < 99964995919919
        public override object GiveAnswer1()
        {
            var foundDigit = 11197951L;
            var scan = 100;
            Dictionary<long, int> matches = new ();
            for (var i = foundDigit*scan+scan-1; i >=foundDigit*scan; i-=1)
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

                if (state.X == 0)
                {
                    matches.Add(i, state.Z);
                    if (state.Ok)
                        return i;
                }
            }

            var minZ = int.MaxValue;
            var minVal = 0L;
            foreach (var pair in matches)
            {
                if (pair.Value < minZ)
                {
                    minZ = pair.Value;
                    minVal = pair.Key;
                }
            }
            return 0;
        }
        // 
    }
}
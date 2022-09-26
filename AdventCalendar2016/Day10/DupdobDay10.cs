using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AoC;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay10 : SolverWithLineParser
    {
        private readonly Regex _input = new("value (\\d*) goes to bot (\\d*)");
        private readonly Regex _comparing = new("bot (\\d*) gives low to (output|bot) (\\d*) and high to (bot|output) (\\d*)");

        private readonly Dictionary<int, Bot> _bots = new();
        
        private readonly Dictionary<int, Bin> _bins = new();

        private interface IInput
        {
            int Value { get; }
        }

        private class ConstantInput : IInput
        {
            public ConstantInput(int value)
            {
                Value = value;
            }

            public int Value
            {
                get; private set; 
            }
        }

        private class Bin
        {
            protected readonly List<IInput> Values = new(2);

            public void AddSource(IInput value)
            {
                Values.Add(value);
            }

            public int FirstValue()
            {
                return Values[0].Value;
            }
        }

        private class Bot : Bin
        {
            public IInput Min => new Filter(this, true);
            public IInput Max => new Filter(this, false);

            private class Filter : IInput
            {
                private readonly Bot _root;
                private readonly bool _min;
                private int? _cache;

                public Filter(Bot root, bool min)
                {
                    _root = root;
                    _min = min;
                }

                public int Value
                {
                    get
                    {
                        if (_root.Values.Count != 2)
                        {
                            throw new Exception("Invalid state");
                        }

                        if (!_cache.HasValue)
                        {
                            _cache = _min ? _root.Values.Min(v => v.Value) : _root.Values.Max(v => v.Value);
                        }

                        return _cache.Value;
                    }
                }
            }
        }

        protected override void ParseLine(string line, int index, int lineCount)
        {
            var match = _input.Match(line);
            if (match.Success)
            {
                // this is a constant input
                var constant = new ConstantInput(int.Parse(match.Groups[1].Value));
                GetBot(int.Parse(match.Groups[2].Value)).AddSource(constant);
            }
            else
            {
                match = _comparing.Match(line);
                if (!match.Success)
                {
                    throw new Exception($"Failed to parse {line}");
                }

                var sourceBot = GetBot(int.Parse(match.Groups[1].Value));
                var id = int.Parse(match.Groups[3].Value);
                var bin = match.Groups[2].Value == "bot" ? GetBot(id) : GetBin(id);
                bin.AddSource(sourceBot.Min);
                id = int.Parse(match.Groups[5].Value);
                bin = match.Groups[4].Value == "bot" ? GetBot(id) : GetBin(id);
                bin.AddSource(sourceBot.Max);
            }
        }

        private Bin GetBin(int id)
        {
            if (!_bins.ContainsKey(id))
            {
                _bins[id] = new Bin();
            }
            return _bins[id];
        }

        private Bot GetBot(int id)
        {
            if (!_bots.ContainsKey(id))
            {
                _bots[id] = new Bot();
            }
            return _bots[id];
        }

        public override object GetAnswer1()
        {
            foreach (var (id, bot) in _bots)
            {
                if (bot.Min.Value == 17 && bot.Max.Value == 61)
                {
                    return id;
                }
            }

            return -1;
        }

        public override object GetAnswer2()
        {
            return _bins[0].FirstValue() * _bins[1].FirstValue() * _bins[2].FirstValue();
        }

        public override void SetupRun(Automaton automaton)
        {
            automaton.Day = 10;
            automaton.RegisterTestDataAndResult(@"value 5 goes to bot 2
bot 2 gives low to bot 1 and high to bot 0
value 3 goes to bot 1
bot 1 gives low to output 1 and high to bot 0
bot 0 gives low to output 2 and high to output 0
value 2 goes to bot 2", -1, 1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AdventCalendar2021
{
    public class DupdobDay21: AdvancedDay
    {
        private readonly Regex _parser = new ("Player (\\d) starting position: (\\d*)");

        private readonly List<(int, int)> _players = new();
        
        public DupdobDay21() : base(21)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var match = _parser.Match(line);
            if (match.Success)
            {
                _players.Add((int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
            }
        }

        protected override void CleanUp()
        {
            _players.Clear();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"Player 1 starting position: 4
Player 2 starting position: 8", 739785);
        }

        protected override IEnumerable<(string intput, object result)> GetTestData2()
        {
            yield return (@"Player 1 starting position: 4
Player 2 starting position: 8", 444356092776315L);
        }

        public override object GiveAnswer1()
        {
            var dice = 1;
            var scores = new List<int> { 0, 0 };
            var positions = new List<int> { _players[0].Item2, _players[1].Item2 };
            var currentPlayer = 1;
            do
            {
                currentPlayer = (currentPlayer + 1) % 2;
                var position = ((positions[currentPlayer] + (dice + 1) * 3) - 1) % 10 + 1;
                positions[currentPlayer] = position;
                dice += 3;
                scores[currentPlayer] += positions[currentPlayer];
            } while (scores[currentPlayer] < 1000);

            return scores[(currentPlayer + 1) % 2] * (dice-1);
        }

        public override object GiveAnswer2()
        {
            var (w1, w2) = ComputeWins(_players[0].Item2, _players[1].Item2, 0, 0);
            return Math.Max(w1, w2);
        }

        private readonly Dictionary<(int, int, int, int), (long, long)> _cached = new();

        private readonly (int, int)[] _dieThrows = { (3, 1), (4, 3), (5, 6), (6, 7), (7, 6), (8, 3), (9, 1) };
        private (long w1, long w2) ComputeWins(int p1, int p2, int s1, int s2)
        {
            var key = (p1, p2, s1, s2);

            if (s2 >= 21)
            {
                return (0L, 1L);
            }
            
            if (_cached.ContainsKey(key))
            {
                return _cached[key];
            }

            (long w1, long w2) result = (0, 0);
            foreach (var (dice, times) in _dieThrows)
            {
                var nPos = Move(p1, dice);
                var (w2, w1) = ComputeWins(p2, nPos, s2, s1 + nPos);
                result = (result.w1 + w1 * times, result.w2 + w2 * times);
            }
            
            _cached[key] = result;
            return result;
        }

        private static int Move(int pos, int step)
        {
            return (pos + step - 1) % 9 + 1;
        }
    }
}
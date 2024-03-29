using System;
using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2015
{
    public static class DupdobArray
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> array)
        {
            var rnd = new Random();
            return array.OrderBy(_ => rnd.Next());
        }

        public static string ReplaceAll(this string text, string toReplace, string replacement, out int count)
        {
            var pos = 0;
            count = 0;
            do
            {
                pos = text.IndexOf(toReplace, pos, StringComparison.Ordinal);
                if (pos < 0) continue;
                count++;
                pos += toReplace.Length;
            } while (pos >=0);

            return text.Replace(toReplace, replacement, StringComparison.Ordinal);
        }
    }

    public class DupdobDay19: DupdobDayWithTest
    {
        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"e => H
e => O
H => HO
H => OH
O => HH

HOH", 3);
        }
        protected override IEnumerable<(string intput, object result)> GetTestData2()
        {
            yield return (@"H => HO
H => OH
O => HH

HOHOHO", 7);
        }
        
        protected override void CleanUp()
        {
            _replacements.Clear();
        }

        protected override void ParseLine(int index, string line)
        {
            var fields = line.Split("=>");

            if (fields.Length == 1)
            {
                _molecule = line;
            }
            else
            {
                _replacements.Add((fields[0].Trim(), fields[1].Trim()));
            }
        }

        // 577 to high
        public override object GiveAnswer1()
        {
            var possible = new HashSet<string>();
            foreach (var (source, dest) in _replacements)
            {
                var position = _molecule.IndexOf(source, StringComparison.Ordinal);
                while (position>=0)
                {
                    possible.Add(_molecule.Substring(0, position) 
                                 + dest + _molecule.Substring(position+source.Length));
                    position = _molecule.IndexOf(source, position + source.Length, StringComparison.Ordinal);
                }
            }

            return possible.Count;
        }

        // 68, 56, 58, 47, 46, 43
        public override object GiveAnswer2()
        {
            var reversedMap = _replacements
                .Where(entry => _molecule.Contains(entry.dest) || _replacements.Any(t => entry.dest.Contains(t.source)))
                .ToDictionary(entry => entry.dest, entry => entry.source);

            var count = 0;
            var sortedEntries = reversedMap.Shuffle();
            var current = _molecule;
            while (current != "e")
            {
                var text = current;
                foreach (var keyValuePair in sortedEntries)
                {
                    current = current.ReplaceAll(keyValuePair.Key, keyValuePair.Value, out var repl);
                    count += repl;
                }

                if (text == current)
                {
                    current = _molecule;
                    sortedEntries = sortedEntries.Shuffle();
                    count = 0;
                }
            }

            return count;
        }

        private string _molecule;
        private readonly List<(string source, string dest)> _replacements = new List<(string source, string dest)>();
        protected override string Input => @"Al => ThF
Al => ThRnFAr
B => BCa
B => TiB
B => TiRnFAr
Ca => CaCa
Ca => PB
Ca => PRnFAr
Ca => SiRnFYFAr
Ca => SiRnMgAr
Ca => SiTh
F => CaF
F => PMg
F => SiAl
H => CRnAlAr
H => CRnFYFYFAr
H => CRnFYMgAr
H => CRnMgYFAr
H => HCa
H => NRnFYFAr
H => NRnMgAr
H => NTh
H => OB
H => ORnFAr
Mg => BF
Mg => TiMg
N => CRnFAr
N => HSi
O => CRnFYFAr
O => CRnMgAr
O => HP
O => NRnFAr
O => OTi
P => CaP
P => PTi
P => SiRnFAr
Si => CaSi
Th => ThCa
Ti => BP
Ti => TiTi
e => HF
e => NAl
e => OMg

ORnPBPMgArCaCaCaSiThCaCaSiThCaCaPBSiRnFArRnFArCaCaSiThCaCaSiThCaCaCaCaCaCaSiRnFYFArSiRnMgArCaSiRnPTiTiBFYPBFArSiRnCaSiRnTiRnFArSiAlArPTiBPTiRnCaSiAlArCaPTiTiBPMgYFArPTiRnFArSiRnCaCaFArRnCaFArCaSiRnSiRnMgArFYCaSiRnMgArCaCaSiThPRnFArPBCaSiRnMgArCaCaSiThCaSiRnTiMgArFArSiThSiThCaCaSiRnMgArCaCaSiRnFArTiBPTiRnCaSiAlArCaPTiRnFArPBPBCaCaSiThCaPBSiThPRnFArSiThCaSiThCaSiThCaPTiBSiRnFYFArCaCaPRnFArPBCaCaPBSiRnTiRnFArCaPRnFArSiRnCaCaCaSiThCaRnCaFArYCaSiRnFArBCaCaCaSiThFArPBFArCaSiRnFArRnCaCaCaFArSiRnFArTiRnPMgArF";
        public override int Day => 19;
    }
}
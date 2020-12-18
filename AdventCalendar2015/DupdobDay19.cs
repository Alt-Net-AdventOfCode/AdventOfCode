using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay19: DupdobDayWithTest
    {
        protected override void SetupTestData(int phase)
        {
            if (phase == 1)
            {
                _testData = @"H => HO
H => OH
O => HH

HOHOHO";
                _expectedResult1 = 7;
            }
            else
            {
                _testData = @"e => H
e => O
H => HO
H => OH
O => HH

HOH";
                _replacements.Clear();
                _expectedResult2 = 3;
            }
        }
    
        protected override void SetupRunData()
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
            foreach (var replacement in _replacements)
            {
                var position = _molecule.IndexOf(replacement.source, StringComparison.Ordinal);
                while (position>=0)
                {
                    possible.Add(_molecule.Substring(0, position) 
                                 + replacement.dest + _molecule.Substring(position+replacement.source.Length));
                    position = _molecule.IndexOf(replacement.source, position + replacement.source.Length, StringComparison.Ordinal);
                }
            }

            return possible.Count;
        }

        public override object GiveAnswer2()
        {
            var reversedMap = _replacements.Where(entry => _molecule.Contains(entry.dest) || _replacements.Any(t => entry.dest.Contains(t.source))).ToDictionary(entry => entry.dest, entry => entry.source);

            var count = 0;
            var sortedEntries = reversedMap.Keys.OrderBy(t => -t.Length).ToList();

            var queue = new List<(string current, int step)> {(_molecule, 0)};
            while (queue.Count>0)
            {
                (string current, int step) cursor = (string.Empty, 0);
                foreach (var valueTuple in queue.Where(valueTuple => valueTuple.current.Length < cursor.current.Length ||cursor.current.Length == 0))
                {
                    cursor = valueTuple;
                }

                var current = cursor.current;
                var curCount = cursor.step;

                queue.Remove((cursor));
                foreach (var precursor in EnumeratePrecursors(current, sortedEntries, reversedMap))
                {
                    if (precursor == "e")
                    {
                        queue.Clear();
                        count = curCount + 1;
                        break;
                    }

                    if (queue.All(t => t.current != precursor))
                    {
                        queue.Add((precursor, curCount+1));
                    }
                }
            }
            return count;
        }

        private static IEnumerable<string> EnumeratePrecursors(string molecule, IList<string> molecules,
            IDictionary<string, string> conversions)
        {
            foreach (var precursor in molecules)
            {
                for (var start = molecule.IndexOf(precursor, StringComparison.Ordinal);
                    start >= 0;
                    start = molecule.IndexOf(precursor, start + precursor.Length, StringComparison.Ordinal))
                {
                    yield return molecule.Substring(0, start) + conversions[precursor] +
                                 molecule.Substring(start + precursor.Length);
                }
            }
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
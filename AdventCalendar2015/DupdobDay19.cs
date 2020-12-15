using System;
using System.Collections.Generic;
using System.Linq;
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
            var usefulMap = new List<(string source, string dest)>();
            foreach (var entry in _replacements)
            {
                if (_molecule.Contains(entry.dest) || _replacements.Any(t => entry.dest.Contains(t.source)))
                {
                    usefulMap.Add((entry.source, entry.dest));
                }
            }

            var dist = new Dictionary<string, int>();
            var nextSteps = new List<string> {"e"};
            for (var step = 0;;step++)
            {
                var after = new List<string>();
                foreach (var current in nextSteps)
                {
                    dist[current] = step;
                    for (var i = 0; i < current.Length; i++)
                    {
                        foreach (var tuple in usefulMap)
                        {
                            if (current.Substring(i).StartsWith(tuple.source))
                            {
                                var mutation = current.Substring(0, i) + tuple.dest +
                                               current.Substring(i + tuple.source.Length);
                                if (!dist.ContainsKey(mutation))
                                {
                                    after.Add(mutation);
                                    if (mutation == _molecule)
                                    {
                                        return step+1;
                                    }
                                }
                            }
                        }
                    }
                }
                nextSteps = after;
            }
            
            return 3;
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
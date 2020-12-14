using System;
using System.Collections.Generic;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay19: DupdobDayWithTest
    {
        protected override void SetupTestData(int _)
        {
            _testData = @"H => HO
H => OH
O => HH

HOHOHO";
            _expectedResult1 = 7;
            _expectedResult2 = "undef";
        }
    
        protected override void SetupRunData()
        {
            _replacements.Clear();
        }

        protected override void ParseLine(string line)
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
            return base.GiveAnswer2();
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
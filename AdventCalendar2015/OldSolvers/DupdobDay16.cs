using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2015.OldSolvers
{
    public class DupdobDay16 : DupdobDayBase
    {

        private readonly Dictionary<int, Dictionary<string, int>> _Sues = new Dictionary<int, Dictionary<string, int>>();
        private Dictionary<string, int> _criteria;

        private const string CriteriaText = @"children: 3
cats: 7
samoyeds: 2
pomeranians: 3
akitas: 0
vizslas: 0
goldfish: 5
trees: 3
cars: 2
perfumes: 1";

        protected override void ParseLine(int index, string line)
        {
            var (suePart, dataSet) = line.SplitAtFirst(':');
            var data = dataSet.Split(',');
            var sueId = int.Parse(suePart.Substring(3));
            var sueData = new Dictionary<string, int>();
            foreach (var entry in data)
            {
                var (left, right) = entry.SplitAtFirst(':');
                sueData[left.Trim()] = int.Parse(right);
            }

            _Sues[sueId] = sueData;
        }

        protected override void Parse(string input)
        {
            base.Parse(input);
            _criteria = new Dictionary<string, int>();
            foreach (var crit in CriteriaText.Split('\n'))
            {
                var (left, right) = crit.SplitAtFirst(':');
                _criteria[left.Trim()] = int.Parse(right);
            }
        }

        public override object GiveAnswer1()
        {
            foreach (var sue in _Sues)
            {
                var match = _criteria.Keys.All(criteriaKey => !sue.Value.ContainsKey(criteriaKey) || sue.Value[criteriaKey] == _criteria[criteriaKey]);

                if (match)
                {
                    return sue.Key;
                }

            }
            return -1;
        }      
        
        // 3, 1 are invalid
        public override object GiveAnswer2()
        {
            foreach (var sue in _Sues)
            {
                var match = true;
                foreach (var criteriaKey in _criteria.Keys)
                {
                    if (!sue.Value.ContainsKey(criteriaKey)) continue;
                    switch (criteriaKey)
                    {
                        case "trees":
                        case "cats":
                            match = sue.Value[criteriaKey] > _criteria[criteriaKey];
                            break;
                        case "pomeranians":
                        case "goldfish":
                            match = sue.Value[criteriaKey] < _criteria[criteriaKey];
                            break;

                        default:
                            match = sue.Value[criteriaKey] == _criteria[criteriaKey];
                            break;
                    }
                    if (!match)
                        break;
                }

                if (match)
                {
                    return sue.Key;
                }
            }
            return -1;
        }

        protected override string Input => @"Sue 1: goldfish: 9, cars: 0, samoyeds: 9
Sue 2: perfumes: 5, trees: 8, goldfish: 8
Sue 3: pomeranians: 2, akitas: 1, trees: 5
Sue 4: goldfish: 10, akitas: 2, perfumes: 9
Sue 5: cars: 5, perfumes: 6, akitas: 9
Sue 6: goldfish: 10, cats: 9, cars: 8
Sue 7: trees: 2, samoyeds: 7, goldfish: 10
Sue 8: cars: 8, perfumes: 6, goldfish: 1
Sue 9: cats: 4, pomeranians: 0, trees: 0
Sue 10: trees: 2, children: 10, samoyeds: 10
Sue 11: akitas: 10, perfumes: 4, vizslas: 1
Sue 12: akitas: 1, trees: 0, goldfish: 3
Sue 13: perfumes: 6, goldfish: 10, cars: 8
Sue 14: cats: 8, akitas: 5, vizslas: 0
Sue 15: cars: 8, trees: 3, samoyeds: 5
Sue 16: vizslas: 6, cats: 6, pomeranians: 10
Sue 17: akitas: 6, cats: 2, perfumes: 9
Sue 18: children: 9, goldfish: 2, akitas: 10
Sue 19: trees: 3, perfumes: 0, goldfish: 6
Sue 20: vizslas: 3, akitas: 0, trees: 1
Sue 21: vizslas: 3, cars: 7, akitas: 3
Sue 22: perfumes: 7, children: 1, pomeranians: 7
Sue 23: trees: 10, cars: 9, akitas: 10
Sue 24: akitas: 5, goldfish: 6, vizslas: 6
Sue 25: samoyeds: 3, trees: 8, vizslas: 5
Sue 26: vizslas: 4, pomeranians: 2, trees: 1
Sue 27: cars: 9, goldfish: 2, trees: 4
Sue 28: vizslas: 6, goldfish: 10, perfumes: 7
Sue 29: vizslas: 6, pomeranians: 3, akitas: 6
Sue 30: trees: 0, samoyeds: 5, akitas: 9
Sue 31: vizslas: 1, perfumes: 0, trees: 6
Sue 32: cars: 7, vizslas: 1, children: 10
Sue 33: vizslas: 1, cars: 1, perfumes: 7
Sue 34: vizslas: 9, trees: 10, akitas: 9
Sue 35: akitas: 3, vizslas: 5, cars: 10
Sue 36: cats: 3, children: 9, samoyeds: 3
Sue 37: vizslas: 5, pomeranians: 7, cars: 6
Sue 38: cars: 10, akitas: 5, vizslas: 8
Sue 39: akitas: 5, trees: 9, children: 2
Sue 40: vizslas: 0, cats: 7, akitas: 0
Sue 41: cars: 9, trees: 10, perfumes: 8
Sue 42: akitas: 4, trees: 2, goldfish: 3
Sue 43: goldfish: 1, cats: 1, akitas: 8
Sue 44: goldfish: 8, akitas: 9, vizslas: 4
Sue 45: perfumes: 3, goldfish: 4, trees: 0
Sue 46: trees: 7, perfumes: 1, goldfish: 8
Sue 47: pomeranians: 10, cars: 7, trees: 2
Sue 48: trees: 2, akitas: 1, cars: 4
Sue 49: goldfish: 5, perfumes: 7, akitas: 8
Sue 50: akitas: 9, vizslas: 9, trees: 2
Sue 51: cars: 0, samoyeds: 0, vizslas: 8
Sue 52: trees: 0, perfumes: 6, pomeranians: 4
Sue 53: vizslas: 1, cats: 6, akitas: 3
Sue 54: samoyeds: 8, akitas: 1, vizslas: 4
Sue 55: goldfish: 10, perfumes: 2, pomeranians: 10
Sue 56: trees: 9, perfumes: 3, goldfish: 5
Sue 57: akitas: 3, perfumes: 0, cats: 2
Sue 58: perfumes: 4, vizslas: 4, cars: 8
Sue 59: goldfish: 7, children: 5, pomeranians: 8
Sue 60: cars: 1, trees: 1, perfumes: 10
Sue 61: trees: 4, samoyeds: 4, cars: 6
Sue 62: akitas: 10, trees: 2, vizslas: 6
Sue 63: goldfish: 3, perfumes: 7, vizslas: 10
Sue 64: pomeranians: 5, children: 10, cars: 0
Sue 65: vizslas: 10, cars: 8, perfumes: 3
Sue 66: children: 5, vizslas: 4, akitas: 10
Sue 67: children: 6, perfumes: 7, cars: 3
Sue 68: goldfish: 8, cars: 6, children: 1
Sue 69: vizslas: 5, perfumes: 3, cars: 9
Sue 70: goldfish: 0, cats: 6, perfumes: 0
Sue 71: trees: 2, samoyeds: 3, cars: 1
Sue 72: cats: 3, akitas: 8, vizslas: 7
Sue 73: akitas: 3, vizslas: 2, goldfish: 6
Sue 74: pomeranians: 10, samoyeds: 9, cats: 8
Sue 75: vizslas: 7, cars: 7, akitas: 10
Sue 76: children: 3, cats: 6, vizslas: 3
Sue 77: goldfish: 7, pomeranians: 10, trees: 0
Sue 78: vizslas: 9, children: 7, trees: 10
Sue 79: trees: 6, pomeranians: 8, samoyeds: 1
Sue 80: vizslas: 5, children: 6, pomeranians: 5
Sue 81: cars: 9, vizslas: 9, akitas: 9
Sue 82: vizslas: 3, cars: 8, akitas: 1
Sue 83: vizslas: 4, trees: 2, cats: 1
Sue 84: children: 3, akitas: 0, vizslas: 1
Sue 85: cats: 6, vizslas: 5, akitas: 2
Sue 86: cars: 3, akitas: 7, goldfish: 8
Sue 87: samoyeds: 8, vizslas: 3, goldfish: 8
Sue 88: vizslas: 4, children: 0, cats: 7
Sue 89: goldfish: 9, pomeranians: 10, samoyeds: 0
Sue 90: trees: 6, akitas: 3, cars: 7
Sue 91: samoyeds: 3, akitas: 7, perfumes: 10
Sue 92: cars: 7, pomeranians: 10, trees: 2
Sue 93: samoyeds: 1, children: 3, cars: 3
Sue 94: samoyeds: 8, akitas: 7, vizslas: 0
Sue 95: goldfish: 7, children: 2, cars: 6
Sue 96: cars: 3, perfumes: 9, akitas: 10
Sue 97: akitas: 9, cars: 10, vizslas: 10
Sue 98: trees: 4, goldfish: 8, pomeranians: 7
Sue 99: samoyeds: 6, pomeranians: 0, vizslas: 7
Sue 100: akitas: 7, perfumes: 8, vizslas: 3
Sue 101: cars: 5, perfumes: 1, trees: 0
Sue 102: akitas: 6, pomeranians: 10, trees: 0
Sue 103: trees: 3, perfumes: 5, cats: 9
Sue 104: goldfish: 10, perfumes: 8, akitas: 0
Sue 105: goldfish: 6, vizslas: 5, trees: 2
Sue 106: pomeranians: 9, samoyeds: 10, perfumes: 10
Sue 107: cars: 8, vizslas: 4, akitas: 2
Sue 108: cats: 0, goldfish: 7, trees: 0
Sue 109: cars: 3, pomeranians: 6, trees: 2
Sue 110: perfumes: 4, goldfish: 5, akitas: 10
Sue 111: cars: 3, perfumes: 4, pomeranians: 4
Sue 112: cats: 2, goldfish: 10, akitas: 0
Sue 113: cats: 10, children: 0, trees: 1
Sue 114: akitas: 10, vizslas: 3, goldfish: 0
Sue 115: samoyeds: 3, goldfish: 6, vizslas: 1
Sue 116: cars: 3, perfumes: 5, trees: 6
Sue 117: akitas: 9, samoyeds: 8, goldfish: 8
Sue 118: pomeranians: 5, perfumes: 10, trees: 1
Sue 119: goldfish: 6, perfumes: 3, children: 1
Sue 120: trees: 1, children: 3, pomeranians: 6
Sue 121: akitas: 7, cars: 10, vizslas: 9
Sue 122: trees: 4, akitas: 8, samoyeds: 10
Sue 123: cats: 4, cars: 8, vizslas: 9
Sue 124: cars: 10, children: 1, trees: 0
Sue 125: goldfish: 5, pomeranians: 5, trees: 2
Sue 126: goldfish: 1, vizslas: 8, akitas: 10
Sue 127: vizslas: 4, cars: 9, akitas: 1
Sue 128: goldfish: 8, perfumes: 3, cars: 9
Sue 129: goldfish: 9, pomeranians: 9, perfumes: 1
Sue 130: trees: 1, vizslas: 9, perfumes: 3
Sue 131: children: 6, trees: 8, vizslas: 8
Sue 132: cars: 1, vizslas: 3, children: 7
Sue 133: cars: 7, children: 1, perfumes: 6
Sue 134: trees: 8, vizslas: 3, samoyeds: 2
Sue 135: cats: 9, perfumes: 4, pomeranians: 7
Sue 136: perfumes: 0, akitas: 8, vizslas: 6
Sue 137: goldfish: 5, trees: 0, vizslas: 7
Sue 138: trees: 1, perfumes: 2, cars: 10
Sue 139: samoyeds: 8, goldfish: 8, trees: 0
Sue 140: vizslas: 10, perfumes: 9, goldfish: 0
Sue 141: perfumes: 7, cars: 9, cats: 5
Sue 142: trees: 2, samoyeds: 2, cars: 0
Sue 143: cars: 1, perfumes: 1, akitas: 1
Sue 144: vizslas: 9, cars: 7, pomeranians: 10
Sue 145: pomeranians: 2, samoyeds: 7, children: 7
Sue 146: vizslas: 6, cars: 9, goldfish: 7
Sue 147: trees: 2, vizslas: 1, cats: 9
Sue 148: perfumes: 9, trees: 4, pomeranians: 5
Sue 149: samoyeds: 8, children: 1, vizslas: 9
Sue 150: cats: 3, trees: 2, vizslas: 4
Sue 151: goldfish: 7, akitas: 10, trees: 3
Sue 152: perfumes: 4, vizslas: 7, cars: 4
Sue 153: pomeranians: 4, akitas: 0, vizslas: 3
Sue 154: samoyeds: 8, trees: 2, vizslas: 10
Sue 155: vizslas: 7, cats: 7, pomeranians: 5
Sue 156: goldfish: 10, pomeranians: 1, vizslas: 1
Sue 157: cars: 6, perfumes: 7, trees: 9
Sue 158: trees: 5, samoyeds: 9, goldfish: 3
Sue 159: pomeranians: 4, akitas: 6, vizslas: 8
Sue 160: goldfish: 7, children: 0, cats: 0
Sue 161: vizslas: 5, akitas: 0, samoyeds: 2
Sue 162: akitas: 4, children: 0, vizslas: 3
Sue 163: samoyeds: 2, perfumes: 0, goldfish: 9
Sue 164: cars: 9, vizslas: 8, akitas: 6
Sue 165: samoyeds: 9, vizslas: 9, perfumes: 5
Sue 166: cars: 5, pomeranians: 4, samoyeds: 8
Sue 167: cars: 10, perfumes: 3, samoyeds: 6
Sue 168: pomeranians: 8, goldfish: 9, trees: 9
Sue 169: vizslas: 7, akitas: 3, samoyeds: 4
Sue 170: cats: 2, goldfish: 0, vizslas: 4
Sue 171: perfumes: 3, goldfish: 10, cats: 3
Sue 172: goldfish: 7, akitas: 6, cars: 0
Sue 173: cars: 9, goldfish: 7, akitas: 5
Sue 174: goldfish: 6, cats: 0, vizslas: 8
Sue 175: perfumes: 7, cats: 10, cars: 10
Sue 176: samoyeds: 9, vizslas: 4, pomeranians: 10
Sue 177: perfumes: 0, trees: 0, cars: 10
Sue 178: vizslas: 6, children: 7, samoyeds: 1
Sue 179: vizslas: 8, children: 6, trees: 0
Sue 180: cars: 1, vizslas: 6, trees: 1
Sue 181: vizslas: 10, perfumes: 3, cars: 1
Sue 182: trees: 8, samoyeds: 9, cars: 7
Sue 183: cars: 6, vizslas: 2, perfumes: 7
Sue 184: trees: 5, samoyeds: 9, akitas: 0
Sue 185: cars: 8, goldfish: 8, trees: 4
Sue 186: samoyeds: 6, goldfish: 1, trees: 2
Sue 187: perfumes: 1, trees: 2, akitas: 7
Sue 188: samoyeds: 5, cars: 6, perfumes: 2
Sue 189: samoyeds: 8, goldfish: 3, perfumes: 5
Sue 190: akitas: 2, cats: 1, samoyeds: 1
Sue 191: trees: 5, akitas: 1, goldfish: 7
Sue 192: vizslas: 3, trees: 0, perfumes: 4
Sue 193: cars: 3, perfumes: 4, akitas: 3
Sue 194: perfumes: 4, vizslas: 8, children: 4
Sue 195: vizslas: 1, samoyeds: 3, cars: 6
Sue 196: cars: 5, perfumes: 6, vizslas: 2
Sue 197: vizslas: 8, akitas: 8, cats: 6
Sue 198: cars: 9, akitas: 2, pomeranians: 7
Sue 199: cats: 9, akitas: 6, cars: 10
Sue 200: vizslas: 10, pomeranians: 2, goldfish: 9
Sue 201: vizslas: 9, samoyeds: 4, akitas: 3
Sue 202: akitas: 5, cats: 2, vizslas: 0
Sue 203: perfumes: 1, children: 3, akitas: 10
Sue 204: trees: 4, vizslas: 7, akitas: 9
Sue 205: trees: 8, perfumes: 9, cars: 1
Sue 206: goldfish: 6, trees: 5, cars: 8
Sue 207: akitas: 3, vizslas: 8, trees: 8
Sue 208: vizslas: 4, perfumes: 7, akitas: 10
Sue 209: cars: 9, perfumes: 7, goldfish: 9
Sue 210: vizslas: 2, cats: 2, akitas: 10
Sue 211: akitas: 1, trees: 3, cars: 2
Sue 212: goldfish: 5, trees: 0, vizslas: 7
Sue 213: akitas: 3, perfumes: 1, vizslas: 5
Sue 214: perfumes: 3, pomeranians: 6, cars: 0
Sue 215: goldfish: 1, cats: 9, cars: 3
Sue 216: goldfish: 9, pomeranians: 6, samoyeds: 0
Sue 217: cars: 6, trees: 2, perfumes: 2
Sue 218: vizslas: 3, goldfish: 8, akitas: 5
Sue 219: cats: 9, perfumes: 7, cars: 5
Sue 220: pomeranians: 5, vizslas: 4, cats: 5
Sue 221: trees: 0, akitas: 7, goldfish: 10
Sue 222: akitas: 2, cars: 3, vizslas: 5
Sue 223: goldfish: 3, perfumes: 7, akitas: 4
Sue 224: samoyeds: 2, cars: 4, vizslas: 7
Sue 225: trees: 5, cars: 0, perfumes: 0
Sue 226: trees: 2, goldfish: 10, perfumes: 6
Sue 227: cars: 8, trees: 9, akitas: 6
Sue 228: goldfish: 10, trees: 10, perfumes: 0
Sue 229: children: 7, samoyeds: 4, goldfish: 6
Sue 230: vizslas: 9, perfumes: 1, children: 10
Sue 231: vizslas: 8, trees: 5, akitas: 9
Sue 232: akitas: 5, goldfish: 9, trees: 1
Sue 233: vizslas: 3, trees: 2, children: 9
Sue 234: samoyeds: 8, perfumes: 0, cats: 0
Sue 235: perfumes: 4, vizslas: 3, akitas: 5
Sue 236: pomeranians: 5, vizslas: 3, akitas: 9
Sue 237: cats: 1, trees: 7, vizslas: 5
Sue 238: children: 5, cats: 4, samoyeds: 5
Sue 239: trees: 3, akitas: 2, goldfish: 6
Sue 240: goldfish: 9, trees: 1, perfumes: 1
Sue 241: cars: 2, pomeranians: 1, samoyeds: 2
Sue 242: akitas: 2, trees: 3, cars: 4
Sue 243: vizslas: 6, akitas: 2, samoyeds: 7
Sue 244: trees: 0, perfumes: 5, cars: 7
Sue 245: goldfish: 10, perfumes: 5, vizslas: 8
Sue 246: akitas: 0, perfumes: 0, cars: 1
Sue 247: samoyeds: 8, goldfish: 0, cars: 6
Sue 248: perfumes: 0, children: 10, trees: 10
Sue 249: perfumes: 6, akitas: 5, cats: 5
Sue 250: vizslas: 7, akitas: 4, cats: 5
Sue 251: samoyeds: 4, akitas: 1, trees: 8
Sue 252: perfumes: 8, pomeranians: 5, cars: 1
Sue 253: akitas: 10, trees: 4, cats: 3
Sue 254: perfumes: 2, cats: 2, goldfish: 9
Sue 255: cars: 4, trees: 1, akitas: 4
Sue 256: samoyeds: 9, goldfish: 0, akitas: 9
Sue 257: vizslas: 9, perfumes: 2, goldfish: 2
Sue 258: perfumes: 1, cars: 9, samoyeds: 1
Sue 259: trees: 0, goldfish: 0, samoyeds: 3
Sue 260: perfumes: 7, cars: 1, goldfish: 0
Sue 261: cars: 0, trees: 5, goldfish: 6
Sue 262: akitas: 7, vizslas: 3, pomeranians: 5
Sue 263: trees: 1, vizslas: 3, goldfish: 3
Sue 264: akitas: 7, vizslas: 4, children: 0
Sue 265: samoyeds: 5, trees: 0, akitas: 4
Sue 266: perfumes: 9, goldfish: 9, cars: 8
Sue 267: cars: 7, perfumes: 10, pomeranians: 8
Sue 268: cars: 0, akitas: 7, perfumes: 4
Sue 269: pomeranians: 0, cars: 9, perfumes: 10
Sue 270: samoyeds: 10, perfumes: 10, cars: 9
Sue 271: akitas: 2, vizslas: 8, cats: 5
Sue 272: akitas: 3, children: 9, samoyeds: 10
Sue 273: perfumes: 2, cars: 10, goldfish: 8
Sue 274: cars: 3, children: 10, perfumes: 10
Sue 275: cats: 9, akitas: 5, trees: 0
Sue 276: akitas: 6, children: 2, vizslas: 1
Sue 277: pomeranians: 6, trees: 10, samoyeds: 3
Sue 278: cars: 7, perfumes: 10, trees: 1
Sue 279: cars: 6, pomeranians: 8, trees: 2
Sue 280: pomeranians: 9, cats: 0, perfumes: 7
Sue 281: vizslas: 10, goldfish: 9, pomeranians: 5
Sue 282: perfumes: 4, samoyeds: 7, cars: 9
Sue 283: cars: 9, vizslas: 6, trees: 5
Sue 284: cars: 7, trees: 1, vizslas: 4
Sue 285: samoyeds: 4, goldfish: 10, cats: 4
Sue 286: samoyeds: 0, akitas: 4, children: 5
Sue 287: trees: 1, perfumes: 3, goldfish: 10
Sue 288: pomeranians: 10, akitas: 3, cars: 2
Sue 289: trees: 7, pomeranians: 4, goldfish: 10
Sue 290: samoyeds: 10, perfumes: 0, cars: 9
Sue 291: akitas: 0, pomeranians: 7, vizslas: 4
Sue 292: cats: 2, vizslas: 8, goldfish: 5
Sue 293: vizslas: 6, pomeranians: 9, perfumes: 0
Sue 294: akitas: 6, cars: 7, vizslas: 5
Sue 295: goldfish: 0, akitas: 9, cats: 0
Sue 296: goldfish: 1, trees: 0, cars: 6
Sue 297: perfumes: 6, cats: 8, pomeranians: 6
Sue 298: cats: 0, goldfish: 6, perfumes: 2
Sue 299: cars: 4, akitas: 1, samoyeds: 10
Sue 300: goldfish: 9, samoyeds: 6, cats: 5
Sue 301: cars: 0, vizslas: 7, trees: 0
Sue 302: goldfish: 9, samoyeds: 1, children: 6
Sue 303: cars: 6, perfumes: 7, samoyeds: 8
Sue 304: trees: 8, goldfish: 9, children: 9
Sue 305: perfumes: 0, cars: 5, goldfish: 4
Sue 306: cats: 3, cars: 7, vizslas: 7
Sue 307: pomeranians: 4, perfumes: 6, cars: 2
Sue 308: cars: 9, akitas: 6, goldfish: 4
Sue 309: pomeranians: 2, vizslas: 10, goldfish: 10
Sue 310: children: 0, cats: 4, akitas: 7
Sue 311: children: 10, akitas: 8, vizslas: 2
Sue 312: children: 5, cars: 0, vizslas: 4
Sue 313: perfumes: 10, trees: 3, pomeranians: 9
Sue 314: samoyeds: 3, goldfish: 2, trees: 9
Sue 315: cars: 2, cats: 5, pomeranians: 10
Sue 316: cats: 6, pomeranians: 6, children: 9
Sue 317: cats: 2, vizslas: 3, perfumes: 1
Sue 318: akitas: 1, perfumes: 3, vizslas: 10
Sue 319: cars: 7, perfumes: 0, trees: 0
Sue 320: goldfish: 6, samoyeds: 6, pomeranians: 4
Sue 321: trees: 2, goldfish: 6, children: 0
Sue 322: goldfish: 0, trees: 2, akitas: 8
Sue 323: pomeranians: 2, samoyeds: 9, vizslas: 1
Sue 324: trees: 4, goldfish: 6, pomeranians: 6
Sue 325: trees: 2, pomeranians: 3, goldfish: 1
Sue 326: perfumes: 4, goldfish: 6, trees: 5
Sue 327: akitas: 3, cars: 8, cats: 2
Sue 328: cats: 6, vizslas: 0, akitas: 2
Sue 329: perfumes: 3, goldfish: 10, akitas: 3
Sue 330: goldfish: 3, vizslas: 1, akitas: 6
Sue 331: perfumes: 4, trees: 1, goldfish: 5
Sue 332: goldfish: 7, vizslas: 9, akitas: 1
Sue 333: children: 8, cars: 8, trees: 4
Sue 334: cars: 1, vizslas: 6, trees: 0
Sue 335: goldfish: 2, cars: 2, akitas: 1
Sue 336: goldfish: 5, akitas: 5, trees: 9
Sue 337: cars: 5, vizslas: 6, goldfish: 6
Sue 338: cats: 9, akitas: 3, goldfish: 9
Sue 339: akitas: 3, cats: 2, children: 7
Sue 340: goldfish: 0, pomeranians: 8, perfumes: 9
Sue 341: trees: 0, pomeranians: 1, goldfish: 5
Sue 342: goldfish: 10, trees: 3, vizslas: 4
Sue 343: cats: 3, samoyeds: 1, children: 6
Sue 344: perfumes: 3, children: 4, samoyeds: 2
Sue 345: children: 6, trees: 2, goldfish: 1
Sue 346: trees: 2, pomeranians: 3, goldfish: 5
Sue 347: akitas: 10, vizslas: 7, trees: 1
Sue 348: perfumes: 4, akitas: 2, vizslas: 7
Sue 349: perfumes: 8, goldfish: 3, vizslas: 5
Sue 350: trees: 4, pomeranians: 5, akitas: 10
Sue 351: perfumes: 5, cars: 9, trees: 0
Sue 352: akitas: 6, children: 8, trees: 10
Sue 353: samoyeds: 7, akitas: 6, vizslas: 4
Sue 354: children: 9, goldfish: 7, perfumes: 5
Sue 355: trees: 1, perfumes: 4, cars: 1
Sue 356: samoyeds: 1, perfumes: 4, pomeranians: 8
Sue 357: trees: 7, goldfish: 10, akitas: 0
Sue 358: akitas: 1, vizslas: 6, cars: 7
Sue 359: vizslas: 3, goldfish: 8, trees: 4
Sue 360: akitas: 10, vizslas: 2, trees: 3
Sue 361: samoyeds: 6, pomeranians: 1, perfumes: 0
Sue 362: samoyeds: 3, cars: 1, trees: 0
Sue 363: vizslas: 0, pomeranians: 9, akitas: 4
Sue 364: perfumes: 9, pomeranians: 8, vizslas: 9
Sue 365: vizslas: 7, cars: 4, perfumes: 10
Sue 366: cars: 0, samoyeds: 5, goldfish: 10
Sue 367: children: 4, vizslas: 5, akitas: 4
Sue 368: samoyeds: 9, perfumes: 4, vizslas: 6
Sue 369: perfumes: 5, cars: 4, samoyeds: 5
Sue 370: akitas: 3, vizslas: 2, perfumes: 1
Sue 371: cars: 8, cats: 7, children: 5
Sue 372: vizslas: 9, perfumes: 2, akitas: 10
Sue 373: trees: 10, pomeranians: 9, goldfish: 3
Sue 374: children: 4, cars: 10, perfumes: 2
Sue 375: children: 7, samoyeds: 5, cats: 0
Sue 376: akitas: 10, samoyeds: 5, vizslas: 5
Sue 377: goldfish: 8, trees: 3, perfumes: 3
Sue 378: goldfish: 10, vizslas: 0, perfumes: 2
Sue 379: trees: 1, vizslas: 7, pomeranians: 4
Sue 380: samoyeds: 8, vizslas: 3, trees: 2
Sue 381: goldfish: 2, perfumes: 5, samoyeds: 9
Sue 382: cats: 3, vizslas: 10, akitas: 5
Sue 383: cars: 7, goldfish: 5, akitas: 8
Sue 384: children: 6, goldfish: 10, trees: 1
Sue 385: cats: 2, akitas: 6, samoyeds: 7
Sue 386: cars: 10, children: 4, goldfish: 2
Sue 387: cats: 0, perfumes: 5, akitas: 9
Sue 388: pomeranians: 7, akitas: 0, samoyeds: 9
Sue 389: trees: 0, akitas: 9, vizslas: 8
Sue 390: cars: 0, trees: 10, perfumes: 9
Sue 391: cats: 9, goldfish: 10, perfumes: 10
Sue 392: cars: 3, vizslas: 6, cats: 3
Sue 393: vizslas: 10, perfumes: 4, goldfish: 5
Sue 394: perfumes: 4, akitas: 10, trees: 2
Sue 395: pomeranians: 5, cars: 4, perfumes: 3
Sue 396: pomeranians: 9, vizslas: 5, akitas: 2
Sue 397: cars: 10, goldfish: 8, trees: 2
Sue 398: perfumes: 7, children: 9, goldfish: 9
Sue 399: akitas: 6, cats: 2, goldfish: 7
Sue 400: goldfish: 9, perfumes: 0, cars: 2
Sue 401: children: 4, vizslas: 0, trees: 2
Sue 402: akitas: 4, cars: 8, pomeranians: 4
Sue 403: vizslas: 8, perfumes: 7, goldfish: 1
Sue 404: goldfish: 10, samoyeds: 7, vizslas: 3
Sue 405: akitas: 1, vizslas: 6, perfumes: 6
Sue 406: pomeranians: 8, goldfish: 6, cats: 3
Sue 407: goldfish: 2, vizslas: 4, akitas: 7
Sue 408: cars: 10, perfumes: 10, vizslas: 3
Sue 409: vizslas: 7, pomeranians: 4, perfumes: 4
Sue 410: goldfish: 4, vizslas: 7, trees: 5
Sue 411: cars: 8, trees: 0, goldfish: 4
Sue 412: cars: 8, perfumes: 5, vizslas: 4
Sue 413: vizslas: 3, akitas: 7, samoyeds: 6
Sue 414: trees: 0, perfumes: 6, cars: 10
Sue 415: pomeranians: 4, trees: 1, perfumes: 6
Sue 416: cars: 10, perfumes: 6, akitas: 2
Sue 417: perfumes: 6, samoyeds: 0, akitas: 0
Sue 418: children: 1, perfumes: 9, vizslas: 3
Sue 419: goldfish: 9, samoyeds: 3, perfumes: 8
Sue 420: goldfish: 4, cars: 10, vizslas: 7
Sue 421: samoyeds: 7, vizslas: 7, cats: 2
Sue 422: trees: 1, goldfish: 8, perfumes: 0
Sue 423: cars: 3, perfumes: 2, trees: 3
Sue 424: samoyeds: 6, vizslas: 0, akitas: 6
Sue 425: trees: 3, akitas: 7, goldfish: 1
Sue 426: cars: 9, trees: 1, perfumes: 0
Sue 427: pomeranians: 0, children: 5, perfumes: 8
Sue 428: cars: 0, perfumes: 6, children: 4
Sue 429: akitas: 7, pomeranians: 9, cats: 6
Sue 430: cats: 6, trees: 1, cars: 0
Sue 431: children: 8, akitas: 5, perfumes: 9
Sue 432: perfumes: 5, akitas: 10, trees: 9
Sue 433: akitas: 4, perfumes: 10, vizslas: 7
Sue 434: trees: 3, children: 10, samoyeds: 4
Sue 435: vizslas: 5, goldfish: 2, akitas: 2
Sue 436: samoyeds: 3, trees: 2, cars: 6
Sue 437: children: 9, akitas: 0, pomeranians: 3
Sue 438: perfumes: 10, akitas: 2, cars: 7
Sue 439: perfumes: 10, samoyeds: 6, akitas: 10
Sue 440: vizslas: 10, trees: 2, akitas: 8
Sue 441: perfumes: 8, akitas: 2, pomeranians: 7
Sue 442: cars: 8, trees: 3, goldfish: 6
Sue 443: cars: 1, goldfish: 5, vizslas: 5
Sue 444: vizslas: 2, akitas: 10, samoyeds: 4
Sue 445: vizslas: 2, akitas: 10, perfumes: 9
Sue 446: akitas: 3, vizslas: 8, goldfish: 1
Sue 447: vizslas: 7, pomeranians: 5, trees: 10
Sue 448: cats: 6, perfumes: 10, children: 6
Sue 449: trees: 2, cars: 5, goldfish: 8
Sue 450: trees: 0, goldfish: 6, samoyeds: 3
Sue 451: perfumes: 0, cars: 8, trees: 1
Sue 452: akitas: 4, trees: 8, perfumes: 9
Sue 453: goldfish: 1, perfumes: 7, akitas: 6
Sue 454: vizslas: 3, cars: 1, perfumes: 6
Sue 455: trees: 1, akitas: 7, goldfish: 10
Sue 456: samoyeds: 4, vizslas: 2, cars: 9
Sue 457: perfumes: 10, children: 1, trees: 8
Sue 458: perfumes: 0, vizslas: 9, cars: 8
Sue 459: cats: 0, children: 7, trees: 3
Sue 460: vizslas: 4, cats: 6, perfumes: 2
Sue 461: trees: 3, children: 5, cars: 8
Sue 462: goldfish: 7, vizslas: 7, children: 5
Sue 463: cars: 5, akitas: 3, goldfish: 5
Sue 464: vizslas: 0, pomeranians: 5, cars: 0
Sue 465: goldfish: 4, akitas: 0, cats: 5
Sue 466: cars: 5, trees: 1, goldfish: 6
Sue 467: perfumes: 10, trees: 8, cars: 1
Sue 468: perfumes: 4, akitas: 3, cars: 0
Sue 469: vizslas: 3, cars: 7, pomeranians: 1
Sue 470: perfumes: 1, vizslas: 7, akitas: 8
Sue 471: goldfish: 10, samoyeds: 10, pomeranians: 5
Sue 472: goldfish: 6, trees: 0, perfumes: 0
Sue 473: goldfish: 5, vizslas: 0, children: 5
Sue 474: cars: 3, vizslas: 7, perfumes: 10
Sue 475: vizslas: 5, trees: 9, goldfish: 8
Sue 476: akitas: 2, goldfish: 6, children: 7
Sue 477: samoyeds: 0, perfumes: 1, pomeranians: 5
Sue 478: trees: 2, goldfish: 9, vizslas: 0
Sue 479: perfumes: 1, cars: 6, goldfish: 9
Sue 480: pomeranians: 3, perfumes: 5, trees: 9
Sue 481: cats: 3, akitas: 0, vizslas: 8
Sue 482: pomeranians: 10, akitas: 8, trees: 5
Sue 483: goldfish: 6, akitas: 10, perfumes: 2
Sue 484: cats: 0, goldfish: 0, children: 9
Sue 485: children: 4, akitas: 10, vizslas: 8
Sue 486: vizslas: 3, goldfish: 9, children: 10
Sue 487: children: 8, cats: 6, vizslas: 10
Sue 488: cars: 7, akitas: 10, samoyeds: 5
Sue 489: vizslas: 9, akitas: 6, trees: 2
Sue 490: vizslas: 5, akitas: 1, children: 5
Sue 491: vizslas: 8, goldfish: 3, perfumes: 6
Sue 492: trees: 3, samoyeds: 1, pomeranians: 6
Sue 493: akitas: 1, vizslas: 5, cars: 8
Sue 494: akitas: 4, cars: 4, vizslas: 9
Sue 495: vizslas: 1, akitas: 2, cats: 2
Sue 496: trees: 7, vizslas: 5, akitas: 6
Sue 497: akitas: 8, trees: 2, perfumes: 6
Sue 498: akitas: 1, trees: 1, samoyeds: 4
Sue 499: cars: 0, akitas: 5, vizslas: 3
Sue 500: cats: 2, goldfish: 9, children: 8";

        public override int Day => 16;

    }
}
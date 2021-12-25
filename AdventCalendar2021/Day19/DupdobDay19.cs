using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventCalendar2021
{
    public class DupdobDay19 : AdvancedDay
    {
        private readonly Regex _header = new("--- scanner (\\d*) ---");

        private readonly List<Scanner> _scanners = new();

        public DupdobDay19() : base(19)
        {
        }

        private static int[] Vector(IReadOnlyList<int> a, IReadOnlyList<int> b)
        {
            var result = new int[3];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = a[i] - b[i];
            }
            return result;
        }

        private static int[] Apply(int[,] matrix, IReadOnlyList<int> vector)
        {
            var result = new int[3];
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    result[i] += matrix[i, j] * vector[j];
                }
            }
            return result;
        }

        private static int[] Translate(IReadOnlyList<int> a, IReadOnlyList<int> b)
        {
            var result = new int[3];
            for (var i = 0; i < 3; i++)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        private static int[,] Multiply(int[,] matrixA, int[,] matrixB)
        {
            var result = new int[3,3];
            for(var k = 0; k <3; k++)
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    result[i,k] += matrixA[i, j] * matrixB[j,k];
                }
            }
            return result;
        }

        protected override void ParseLine(int index, string line)
        {
            if (string.IsNullOrEmpty(line))
                return;
            if (_header.Match(line).Success)
            {
                _scanners.Add(new Scanner());
            }
            else
            {
                var coords = line.Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                _scanners[^1].Beacons.Add(coords);
            }
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390

--- scanner 2 ---
649,640,665
682,-795,504
-784,533,-524
-644,584,-595
-588,-843,648
-30,6,44
-674,560,763
500,723,-460
609,671,-379
-555,-800,653
-675,-892,-343
697,-426,-610
578,704,681
493,664,-388
-671,-858,530
-667,343,800
571,-461,-707
-138,-166,112
-889,563,-600
646,-828,498
640,759,510
-630,509,768
-681,-892,-333
673,-379,-804
-742,-814,-386
577,-820,562

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14", 79);
        }

        protected override IEnumerable<(string intput, object result)> GetTestData2()
        {
            return base.GetTestData2();
        }

        public override object GiveAnswer1()
        {
            foreach (var scanner in _scanners)
            {
                scanner.ComputeDistances();
            }
            // we need find marching pairs of scanners
            Dictionary<int, (int referenceScanner, int[,] rotation, int[] translation)> 
                transformations = new();
            for (var i = 0; i < _scanners.Count; i++)
            {
                var scannerA = _scanners[i];
                for (var j = 0; j < _scanners.Count; j++)
                {
                    if (i==j)
                        continue;
                    var scannerB = _scanners[j];
                    var pairs = scannerA.FindMatchingBeacons(scannerB);
                    if (pairs.Count < 12) continue;
                    var (rotation, translation) = scannerA.ExtractTransformation(scannerB, pairs);
                    transformations[i] = (j, rotation, translation);
                    break;
                }
            }
            
            // simplify transformation
            for (var i = 1; i < transformations.Count; i++)
            {
                var (reference, rotation, translation) = transformations[i];
                while (reference!=0)
                {
                    var (ref2, rot2, trans2) = transformations[reference];
                    rotation = Multiply(rotation, rot2);
                    translation = Translate(Apply(rot2, translation), trans2);
                    reference = ref2;
                }

                transformations[i] = (reference, rotation, translation);
            }

            var beacons = new HashSet<(int x, int y, int z)>();
            for (var i = 0; i < _scanners.Count; i++)
            {
                var (_, rotation, translation) = transformations[i];
                foreach (var translated in _scanners[i].Beacons.Select(beacon => Translate(Apply(rotation, beacon), translation)))
                {
                    beacons.Add((translated[0], translated[1], translated[2]));
                }
            }
            
            return beacons.Count;
        }

        public override object GiveAnswer2()
        {
            return base.GiveAnswer2();
        }

        protected override void CleanUp()
        {
        }

        private class Scanner
        {
            public readonly List<int[]> Beacons = new();
            private long[][] _distances;

            public void ComputeDistances()
            {
                _distances = new long[Beacons.Count][];
                for (var i = 0; i < _distances.Length; i++)
                {
                    _distances[i] = new long[_distances.Length];
                }
                for (var i = 0; i < Beacons.Count; i++)
                {
                    _distances[i][i] = 0;
                    for (var j = i + 1; j < Beacons.Count; j++)
                    {
                        var dist = Vector(Beacons[i], Beacons[j]);
                        var distance = dist[0]*dist[0] + dist[1]*dist[1] + dist[2]*dist[2];
                        _distances[i][j] = distance;
                        _distances[j][i] = distance;
                    }
                }
            }

            public List<(int,int)> FindMatchingBeacons(Scanner other)
            {
                var matchingPairs = new List<(int a, int b)>();
                for (var i = 0; i < Beacons.Count; i++)
                {
                    for (var j = 0; j < other.Beacons.Count; j++)
                    {
                        if (_distances[i].Intersect(other._distances[j]).Count() < 12) continue;
                        matchingPairs.Add((i,j));
                        break;
                    } 
                }

                return matchingPairs;
            }

            // returns the rotation matrix and the translation vector to transform other's beacons into this referential
            // to translate, one must firs rotate the beacon then translate it (adding the translation vector).
            public (int[,] rotation, int[] translation) ExtractTransformation(Scanner other, IReadOnlyList<(int a, int b)> matchingPairs)
            {
                // identify how the scanners are relatively positioned
                // we use couple of stars
                var matrix = new int[3, 3];
                for (var x = 0; x < 2; x++)
                {
                    for (var y = 0; y < 2; y++)
                    {
                        matrix[x, y] = 0;
                    }
                }

                var found = false;
                for (var i = 0; i < matchingPairs.Count; i++)
                {
                    for (var j = i + 1; j < matchingPairs.Count; j++)
                    {
                        var v = Vector(Beacons[matchingPairs[i].a], Beacons[matchingPairs[j].a]);
                        if (Math.Abs(v[0]) != Math.Abs(v[1])
                            && Math.Abs(v[0]) != Math.Abs(v[1])
                            && Math.Abs(v[1]) != Math.Abs(v[2]))
                        {
                            var w = Vector(other.Beacons[matchingPairs[i].b], other.Beacons[matchingPairs[j].b]);
                            if (Math.Abs(v[0]) == Math.Abs(w[0]))
                            {
                                matrix[0, 0] = v[0] / w[0];
                                if (Math.Abs(v[1]) == Math.Abs(w[1]))
                                {
                                    matrix[1, 1] = v[1] / w[1];
                                    matrix[2, 2] = v[2] / w[2];
                                }
                                else
                                {
                                    matrix[2, 1] = v[1] / w[2];
                                    matrix[1, 2] = v[2] / w[1];
                                }
                            }
                            else if (Math.Abs(v[0]) == Math.Abs(w[1]))
                            {
                                matrix[1, 0] = v[0] / w[1];
                                if (Math.Abs(v[1]) == Math.Abs(w[0]))
                                {
                                    matrix[0, 1] = v[1] / w[0];
                                    matrix[2, 2] = v[2] / w[2];
                                }
                                else
                                {
                                    matrix[2, 1] = v[1] / w[2];
                                    matrix[1, 2] = v[2] / w[1];
                                }
                            }
                            else
                            {
                                matrix[2, 0] = v[0] / w[2];
                                if (Math.Abs(v[1]) == Math.Abs(w[1]))
                                {
                                    matrix[1, 1] = v[1] / w[1];
                                    matrix[0, 2] = v[2] / w[0];
                                }
                                else
                                {
                                    matrix[0, 1] = v[1] / w[0];
                                    matrix[1, 2] = v[2] / w[1];
                                }
                            }

                            found = true;
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }

                // we need to find the needed translation
                var firstBeacon = Beacons[matchingPairs[0].a];
                var otherFirstBeacon = Apply(matrix, other.Beacons[matchingPairs[0].b]);
                var vector1 = Vector(firstBeacon, otherFirstBeacon);
                
                // test
                otherFirstBeacon = other.Beacons[matchingPairs[0].b];
                otherFirstBeacon = Apply(matrix, otherFirstBeacon);
                otherFirstBeacon = Translate(otherFirstBeacon, vector1);
                return (matrix, vector1);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2020.Day24
{
    public class DupdobDay24: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            _moves.Add(line);
        }

        public override object GiveAnswer1()
        {
            var tileMap = new Dictionary<(int y, int x), bool>();
            foreach (var move in _moves)
            {
                (int y, int x) start = (0, 0);
                for (var i = 0; i < move.Length; i++)
                {
                    var next = move[i].ToString();
                    if (move[i] == 'n' || move[i] == 's')
                    {
                        next += move[++i];
                    }

                    start.x += _offsets[next].dx;
                    start.y += _offsets[next].dy;
                }

                if (!tileMap.ContainsKey(start))
                {
                    tileMap[start] = true;
                }
                else
                {
                    tileMap[start] = !tileMap[start];
                }
            }

            _initialMap = tileMap;
            return tileMap.Values.Count(t => t);
        }

        public override object GiveAnswer2()
        {
            // gentle game of life
            var currentMap = _initialMap;
            for (var day = 0; day < _nbDays; day++)
            {
                var minX = currentMap.Keys.Min(p => p.x) - 1;
                var maxX = currentMap.Keys.Max(p => p.x) + 1;
                var minY = currentMap.Keys.Min(p => p.y) - 1;
                var maxY = currentMap.Keys.Max(p => p.y) + 1;
                var nexMap = new Dictionary<(int y, int x), bool>(currentMap.Count);
                for (var y = minY; y <= maxY; y++)
                {
                    var min = minX;
                    var max = maxX;
                    if (y % 2 == 0)
                    {
                        if (minX % 2 != 0)
                        {
                            min--;
                        }
                        if (maxX % 2 != 0)
                        {
                            max++;
                        }
                    }
                    else
                    {
                        if (minX % 2 == 0)
                        {
                            min--;
                        }
                        if (maxX % 2 == 0)
                        {
                            max++;
                        }
                    }
                    for (var x = min; x <= max; x+=2)
                    {
                        var neighbours = 0;
                        var currentTile = (y, x);
                        foreach (var (dy, dx) in _offsets.Values)
                        {
                            var coords = (y + dy, x + dx);
                            if (currentMap.ContainsKey(coords) && currentMap[coords])
                            {
                                neighbours++;
                            }
                        }

                        if (currentMap.ContainsKey(currentTile) && currentMap[currentTile])
                        {
                            if (neighbours > 0 && neighbours <= 2)
                            {
                                nexMap[currentTile] = true;
                            }
                        }
                        else{
                            if (neighbours == 2)
                            {
                                nexMap[currentTile] = true;
                            }
                            
                        }
                    }
                }

                currentMap = nexMap;
            }

            return currentMap.Values.Count;
        }

        private readonly List<string> _moves = new ();
        private readonly Dictionary<string, (int dy, int dx)> _offsets = new()
            {{"e", (0, 2)}, {"se", (1,1)}, {"sw", (1,-1)}, {"w", (0, -2)}, {"nw", (-1,-1)}, {"ne", (-1,1)}};

        private Dictionary<(int y, int x), bool> _initialMap;
        private int _nbDays;

        protected override void SetupTestData()
        {
            TestData = @"sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew";
            ExpectedResult1 = 10;
            ExpectedResult2 = 12;
            _nbDays = 2;
        }

        protected override void CleanUp()
        {
            _moves.Clear();
            _nbDays = 100;
        }

        protected override string Input => @"wwwnwsenwnwnwnwnwnewnewsewnwnwnww
nenwnwnewnweewsenwenwnwwswwnwswsw
seswswseneeeswswnwnwswseswnwswswswswsw
swenenwneswswswswsenwswswnwenenwseswsw
wneneenwseeeewneeseneeeneeenenene
wwwwenwwwwwnwnewwnwswwwwwse
neneneneneneeneswneeneenwneeneneenesw
seneseswseseswswseseswswseswsewsesesesw
enweneeeeeseeeesw
wwwsewswwwwswwenwneew
sesweenwseeneeeswneeseeeseswenwsese
seswswwswswswswseswswnwswswene
swwnwwwwwwswnwwwseswwwwwseww
wwnewwsewwwwswwneseweewwnw
wswswseneswswseswseneswseswseswswseswnesw
nenwnwneneseneeneswnewenweeswneeese
wwenwwwwnwwwnewsewwnwwwswwnw
newnwswneswswswneeeneenenee
swwsewswswswnwswswswnewewseswswswsw
eseneeenwseesweesw
wnenenenwnenenwnenwnwnenwnwsesenwnesene
eswneeeeeeeeneeeneenenewnee
enwenweeweewesesesweswswnweew
eseeseseseeswsewswww
wseeneewnwswswenweee
swwneswswwwwwswwswwwswwseswww
wesesesewsweseenwnesewneneswseswsw
nwswsesenwswnenenwenenwne
neeneseenenweneneneeeweeneeeswe
wwnwnwwwwsenwnwwnwwwnwwnwnwnw
eswnewswwnwnwwnwnwnwenwwsenewenww
wswsenwsewswseeneseseswswswneseswsw
sewswewwwnwwwnwwneewnwnwnwswnw
wsweswswwnwwsweswswswswnwswswswswswswe
swneseswswswswswsesenwsesw
neeeesewseenweeesweseeeeeee
nenenenenwnenenwneneneneeswswnwnenenwnwnw
nwwwewnwwnwwwwnwwnewnwwwwsenw
senwseeeesenwseeeseenwsweseseswnee
seseseeesenwseseeseeseseesesese
nwsesesesenwseseeseseseswewseswswswsw
wwwwnwwwwwnwnenwnwnwewnwwswww
neneneneswneswneneeeenwneneneeenene
seswnwseswsenenwnewseneswsenwsenwseenwswse
neeswwewwwnwneswnwwswswnw
eswwwswwnwwswneswwwsweswwweww
nwswwsewneeeneswneeeeeneeneneesw
eeswewwneseneneeneeeneseeeneneee
seseewseeseneweeeweesewsenewe
swseswwswneswswswsewsweswnwswseswwe
nwnwenwnewnwsenwsewwnwwnwnwwwnewnw
nenwsesesenwwnwsewenwwwwweswwwwe
nesenweseeeseeswsewsewswwseeeese
sewwswseseseneseswnwesenwseesenwenw
weseseseesesesesesenwseseseseesesese
swwnwnwnewnwnwwnwwwew
nwnwnwnwnenwswnwnwnwnwnwwnwnwenwsesesew
swsewwneswwnwswseswwneswswsewnewnew
wwwwnwwswwnwnwewwwwwwnwwnw
seseseseseeseseeseewnweseswnwesesese
nwwnenwnwwnenenwnwnwnwneneenenenwnenwse
nwwsenwnwnewnwwwnwwnwnwnwwnwnwwsenww
sesewsesenweseseseseseseswsesese
seewneswwwwnwwwseswwnwewnwnwnw
neneneneneneswneneneswswe
wwwwwwwwsenwwwwwwwwww
wnwwwnwwnwsewnwwwwwwswe
nwswnwnwewnwsenwneneneneswnwenene
senweweneseeweeeseneeeweeenw
wnwnwwwwenwwwswnwnwewswnwnewwne
eewnewswnenwswwswwsweeseswnwwnw
wsenenenwnenenwnwnenenenwnenenenenenenw
swseswwswwswnwswswseneseeesw
eeenwnweeeseeeeeeeeeswee
seseneewsesesenwseseeseswsesesesese
swswwswsweswswwswswsw
seseseseseweeeeeeseeseswneeenw
nenwnenwneseneswnenwnenwsene
neneneneneneneswenenenenene
nwnwnwswnwnwenwneneneneeswnwnwnwwnwne
wswnwneneenenwneswnwneswnenenwswsenwnwe
nwnwnwswnwnwnwsenwnwnewnwsenwnwnwenwswnw
seneneneneenenweenenenenenenenenenew
nwwswnwnwnwwnwnwnwnwnwwsenwnwsenwenw
seseseseswsenesesesesesesesesese
enwwwwenwswwswnewwswwsesww
wnwsenwnewenewswswswseewwwwswswswe
seswswnwswwwswswenwneesenwswswswnwesw
sesenwnwswsesesesesenewsesesenwsesesee
wseswnwswseseseeneeswsesesesesewsesene
swwseneswnwwwswnwwewwnewweewsww
neswseenwseneneenesenenenewnenewswnee
eeseeseweeeseeeeseesesesee
esesewseseseseneseswsenweseeeesese
wwnwnwwwwwwwnwnwnwsenewnwse
seenweseseseseeeseswnweeseseeenee
swswwswwwwwwwwwwnewwwwnenew
nenenenenenenwnesenenenewnenenenenenwne
enesweeeeenweeneeenwsww
eeneeeeeeneeeseeeeeeewswe
sweeeeeeeeeeeneeneee
wnwnwwwsenenwwwnwnwwnwnesesenwnewsw
neneeenweeseneeswneeweesenwneee
seneeeseseswnesewswnwneswswnwsewwnwnwsw
nwneeenwnenenenwnwnenenenwnwnewnewne
neswnwsweesewswswnwwneswswwswneswwsene
seeseseeeenwswseeenesesesenwsesewse
nwnwnwnwnenwnwnenwnwnwenwnenwenewwse
enwnwnwnwnwnwnwnwnwswnwnwswnwswnwenwnwnwne
nwwwneneswswneeewnwwnwswswseeseseee
nenwnwswnwnwsewneneenwnwwnw
senwweweswnwneeneseeeswnweseswse
wnwwnwnenwnenwsesenwnwwnwnwswnwswnwnwnenw
wwnwnwsewnwnwwnwesweswnewwwswne
nenenewneswneneswneeweswneenwnesenw
eswnwwwsweseswwswswwwnwswswnwswsenw
eeeswsenwseeweeeeeeseeeeee
nwwwnwsewwnwwnwwwwnewwsewwnwwne
seseseesesesesesesenwseseseseseseseswsw
nwnwnwswsenwnenenenenwnwnwnwnwnwwnwne
nwsesewesenwnwnwnwswsewenwswnwnenwwe
newneseneneseswnwnwswnwewswnwnwseesw
eeeeeeneweeeneeneswneeseene
nwnwwnwnwnwnwwwswnwnwnwnwwewnwwswe
swswswseswneseseseswswseseswswswnwsesesesw
eeseneneseweneneeeneeneenwneewne
eseeswseeeeseeneseeeenwseeeese
sesesenwsesesesewsesenesesesesesesesesesese
senwseseseseseseseseseseseseseseswsesenwsw
neeneneswnenenenenenenenweneneneneswne
swswswseswwswswswswnwswswswswwseswswnwsw
neeeeeeseneeneswwneeeeeneeee
enweesweneneswweneeewenewenee
sewwswswwseneswwwsenwwswwwnewwww
wewwwenwseswnwwwwwwewnwnwnw
nwwenwsewswwwsw
neneneeneneneseneneweseenenweenenene
nwswswneswswwnwesenwsweswswswsw
swenwnwnwnwnwnwnenwsenwnwwnenwnenw
swseseneseseseseneswwsewneseewswnwsene
wneeneneswnwneneewneneneeneenenee
eenwneeweeeeeeesweeeee
seneeswnenweweweeweneeswseee
swseseesewswswsenwswswswswswswsw
wswwswwwswwswwwwnewwwwsewnesw
nwnenwsewnenenenwnenwnwnwne
neenwneneneneneseneneneenewnenenwnenesw
enwnwwnwnwesenwswnenwnwnwenwnwnwswnw
swnenwswsweseeswswswseseswwnwswswseseswsw
nwswnesweswswsewswseswnwswswswswneseswesw
swneseneswswwsenwswswswsw
eeeeseeeeeeeweeeeeneee
wnwwnwswewwnwwwwwwwseww
neenenwswnenweneeeeseneseee
neneweneeeneneneswnenweweeseene
neeswnesenenwnwnenenwswnenenwswwnesenw
senenwnenenenenenwnene
sewnwneseeweswseeseenesesenweswwwne
swswnewseswswswswswwseseswswseneswswswsesw
nwnwswenwnwnwnwnwnwnwnwnwnwsewnwnwnwnenw
eseneswswnwswnweneeeeeeeeeeee
nenwnenwnwnenwnenwnwnwneswnwnenwe
senenenwsweswsweseenwnwwswnwswswswnwwe
nweneeneseneswwneswneneenewneeneene
seeseswnesenwnwenwnwnweneswwwnenwnwwnw
eseseeseeeseesesenesenweeseswsesese
swswsesenwswswswsenwswneswswseneneswsew
nenwnwnwseewnwwnwnwsewnew
swswseswswswneweswswswwswswswswswswswswse
wswsweswswswwsweswswwnwswswswswww
swnewwsenwwswwnewnwwsewwswnwnwne
wnwwwwnwwwsewnenwneenwswsewwnwww
nenweeeeeneeeesweee
sesesenwswswneswenwswwswwneswnesw
swswsewswwswswswenewswsww
seswseesesesesenesese
nweeseseneswwewseswseneseeeeeese
swnwswswseswwsweswwwswwwwewwnw
wnwnwnwnwneswnwnwnwnwwnwnwnenwnwsenwnwnwnw
nesenenwwneneswnenwnwsewneenwnwnwnwnw
eweeneneeseneeenweneeeneseenene
nwwnwnenwsewnesewwewwnewwnwese
eseeseseeeeseseenweseeseseeesw
wwnwwnwwewnwnwnwnwwnwwwnwsww
wnenweewsesewww
nwseseseseseseweeseeseseseseseesese
seseswwseswswsesweswseseswswswsesw
wswswsweswswwswswswwswswwsw
eeeneenweneeeeeeeewseseene
neneneenwseseswswenewsenenenwwsw
nwnwsenwnwsenwnwwnwnwnwnwnwnwnwnwnwnwnw
eeneseseneenenwewneeeeeneene
wwswswswwwwswswswwswwnwsewsw
senewseseewnwswseseseswseseswseswsesese
seswseswnwnwweseneseswsesesweseswsew
nenwnwseswseeweewnwnwneswnwswwnwsww
nwnwnenwnwnwnenenenenwnwnenesenwsenenenwnw
sewnwneswsewwswnwwseewnenenweswne
nwseswnwswsesenwnwnwwswnenewseswsewe
newnweseswsesenwwnwwnwnwnwnwnee
swwnweswwneeswnwswenwnwswswwswwse
sewwnwswenwwwwsewnwwseswneeswneswe
wwwwwwwnwwnesewwwwwnwnwww
seenwwswenwswnweeeeeeseeewsese
wswnwwwwwsewnewwswsewswwseenw
nenenenwwenwnwnenenenwnwnenesenwnwwse
nwnwnwnwnwnwwnwenwnwnwnwnwnwsenwnwnwnwnw
seseswseswnwseseseseseseseseswnwsesesesw
wnwwwwsenenwsenwwnenwwnwwwwenwnw
nwnewseseswsewnwnenwswnwseenesewnwnenw
neswsewwwwweneswwwweswwwnenw
nweswneneeneeeeneneneneswneneeneee
wswneneseeswswsewsenweseseswenwnwsw
sewswnwnwnwnenwnwnwnwswsewwwnwnenwwne
swswwswswswwswnenwswswsweswsenwweseswse
neseswwwnwnwnewnwseswnwwwenwnwnwe
swwswswswseswneswswwswswswwswnwswsesw
wnwnenwneswwnwwnwnwwsenwwnwwwwnw
wwnewsenwwnwwnwwwnwnwenwwnwww
swseswnweewwswnewswwenesenewswnw
eneeseenwsesweeneenweenwwesese
sesesewswwswswswswseeseseseseseswewe
nenwenwnwwnwnesenenwnwnenwswnwnenenwe
seenweeeseseseesenwseswseswsew
nenwnenesenwwnwnenenwnwnenwnwnenenenwnw
nwswseewswnwneswswneseseswsewseswnwse
nweneswwnwnwwwewsenwwwwwnesww
neeswswswwwwnwsewneswswswsewwwwww
wswseswneswesewneneesesenwseenwenw
wewwswewwnwwsewwwwwnwnwwwne
ewseneesesesesenwseesesweseseseee
nwenesweeeeeneneeneneneeneeee
nwswseswsesesewswseseswseseneseswse
wnwnewneneneswewneneneeneneenenenene
nwnwnwnwnwnwnwnwnwnwnwnwwnwenw
wwswswwnwwwwswswwweswwwwsww
seseseseewseseswnwneeeseseesenesese
neenwnwnesenwswnenweenwsewnwwswwnwse
swswseswnwswseswswswseseswseswnwsesweswswsw
eseeeseenenwenesweneweeenwwse
nwwwwwwwnewwwnwsewwwewwwsw
swswseswseeswswseswswswswsenenwwseswsw
nwneswseneneseeseseeswseswsenwseseswsese
nenwwwwwwwwseseweswwwwwww
wsesenwwseeseneseneneswwseseeseswsese
swswswneswswswwwwwwswswswswnwsewswse
eesewsesewneswseneeseenweeswese
swsenwsenwswneswswswseswseneseseswsesenwse
wwwswnwsweswwesewwwwwwswwwe
eswswnwwwwnwweenwwwwwwnwwnww
seweeeneeneeeeenwsweneneewswsee
nwswnwnwnwwnwnwnwsenwnwnenwnwnwnwwnwnw
sweswwseseswsesesw
nwwnwenwswwnwnewwswsewnwwwewwwne
nwnwnwnwwwnwnwnewnwnwnwnwnwwsenwnww
wswswswneswseswswnwweewswwswswwswsw
neneneeneswnenwnenenesenewneeneeene
neswseseseneseeewewneseseseseseswnw
sesweswnenenwneneeneneneneenenwnwsenene
nwwenenenwseewwseseswnwwwwwenewe
nwnenwnwnenenenenesewswnenwneenw
neeneneneneenewseneneneenene
wwwswwesenwswneswswswwwswneswswswsw
sewwwwwwwwwwwne
seseeeeeseseseseweneseseswsesewsenw
seeseeeseewseeneseseeesesesesee
wswwswwwwswweswsw
nwswnwnenwnwneswnenwnwnwnenwnwnwnenwnwne
wswnenwnwnenwnenwswnenwsenenenenenwnwenwne
enweeeeeeeeseeeswewweeee
seeeesweseeeeseweseneenesenwsese
nenwnwswnwnwwswwsewnwnwnwnewnwwwnw
sesesweneweseswseseseswnwsesesesesesese
senewwwswswwnewwnewsewnwwenwsenww
wwwsenwnwwnwnwnwnwwnwwnwwnwsewnw
swswswswswseswswswswnewnwwnwswwwswe
seeeneeseesenweseeeewseeseswnwsw
wwswnewewsewnwweswwwwswswww
nwnwwnwnenwnwswnwenwnwnwnwnenwnwnwnwnwnw
neneeneneeneeenweewneneswne
nesenewenenenenwnwnenenesenewsenesewe
seneeseeswesesesesenweeewese
sesesesesesesesenwesesesesw
wnwwnwwsewwwsenenw
neneeneeeneneeeeenwesweeeee
eeeseneneneeneeneneeneewewee
seseseseseseswsesenwsesesesesesese
wsweswswnewswwsweswwswwswswwnwswne
wswseswwwnwswswesewswneswnwwwnee
eseseseswneseseeseseseswseneswnwsewsesww
nwnwsenwnenwnwnenenwswwnwswwnwneeee
neneeeneneewwneeneneneeswseneneeee
nenwewnwwwnwnwsenwse
nwnwnwnwnwnwnwnwswwnesenwnenw
nwweseseseseseeeseseseeeenesesese
nenwnenwnwnewswnenenenwnesenenenenenesenw
seneseseseseseseeseseseseswew
neseeneenenenenenenenenewneenenwene
nwnwnwnenenenwnwnenesenenenenwnenewnwne
swseswswseswnewswwswswseneswneswenwsw
eeneeseenwesweeneweeseenwene
wswseseseseneseseseseswwseneswesesenenw
sesesesesesewseseseseseseseeseneswsesew
nweswnwnwesesweseswswnwnweseenwsene
seneseeseeeeseweseeeseseesesese
wneeneeneweneseneneneesenenenenene
seseesesesewwseseseseeesesenwsesese
neneweeseneewnwseneneeswneneeeneene
neswenwswseseswswneswnwenenwseswsenwnw
swswnwseseseseswswsweswnwsenwswnwsesesese
wnesenwwwweswwwwwwwswwwww
eswnwwswweweeeswnweswnenweseww
wwnwwweseswnwwnweseeneswswe
wnwnewwnewwwwwwnwsewwwwwsww
neswnenenwneneneneneesenewneneeeenene
wneewnewsenwsenwnewswesenwswwnewsw
nwsesesewwnenwnwe
nwnenwnwnwwnewenwsenenwneesenwnwnewse
seneseswseseseswseswsewnese
swseswswswseeswswswswnwsesesesw
newnwseeenwnwwswnenenwnwnenwnw
swwwnwswswswsewwswewwswwswwwnesw
nwnenwnwnwseseneenwnwnenenwnwnwnwsewsw
seeseseseseseseeswnwsesesesesesesenw
sewenewnwneneewnwnwnwnwnenenwnwnenwnw
neneneesenenwnwwneneneneneneenenwneswnw
wwwwswswswswewwenwnwwswwwswsew
senwwwwwwwnwwwwwwwwwweww
sesesesesesesesesenwsesesese
nwwnwnwnwnwnwnwnwwnwnwwnwsenwnwnww
nenwnwnwnwsenwnwnesewnenwnenenwnwnwnwnenw
nwneneeeeeeeeeeneeeeseswee
swseswseswswswswswswswseswnwsweswswswsw
swswswnwswswswswswseswswswswneswswswswse
seseewsesewwnwnesenenenenenenewnenesew
neswseswnwseswswswesesweneswnwseswnwsew
senesewnwswwseseseseseeeneeseeese
nwsweenwenwnwnwwnwnwwnwnwnwnwwnwswsw
seseswswswnenwseswneeswswswsweseseswnw
seseswseswswseseneseewseswswseswswsewse
swswswwsesewswswswswwswnwwwwneswswnesw
eeeeeeenweseeesw
eeeneeeneeweseeeee
nwnenewneneneenwsw
neeneswenwweeswewwseenenw
nwswnwewwnwnwnwwsenwnwnwnwwnwnwnwsenww
enewenwseneneswnwnesewsewnenenwneseswne
seneswswseseswswesenwsesw
nwnwnwwwnwnwwnwnewnwsenwwnwnwnwewnw
nwnwnwswswwnwsenwnenwenwnwsenwnwnenwnw
nwseneswnenwwnwnenenenwnenwnenene
wswswswswwwwswwswsenwwswswswsw
wwwnwnwwenwwsenwwwwewwwnwww
eseeneswseenwwseeeeswsesenwswwe
neseeswnenwwnewsesesewsweneseeswsese
seswswwseswswswsenwseseseseseeseswseswnw
swnwswwnweweewwwswwwwswesww
nenwsenwenwnwnwnwnwnwnwnwwnwnwnwnwnwnw
swswseswneswswswsenwseswswswnwneseewswsw
neneweenenenesenenene
swneswswswswswswswswswsw
nenenenenenenwnenenesewnenwnwnenenesenene
eneeseeeeeeeeeeseeewesee
seeseeseseeneseseeeseseseswese
seseswenwswnwseswswswswseseeswswseseseswse
nwwnwwnwnwwnwnwnwseewnwnwnwnwnw
seneswswswwswswseneswswseswswswseseswsese
nenenwneneneenwnwwnenwnwseneswnenenwnw
seseewsenwneeneswenwnwneswenenenese
nwnwsenenenwnwnwnwswnwnenwnwnenenwnenwne
nwnwnwwenwswnwnwnwnenwnwnwwnwwnwwwsww
swswnwseseseneswneneesewseenwnwsesew
neneenwnewneswnenwnenenenwne
wswswneseseswseswsesewswswseseseseseese
nwnwnwnenwnwnwwnwsewswnww
wwewnwwwnwwwwnwwwnwnwnwwwsw
swsesewsewnenesenesesesesesenweswsese
newnenwswnwneneweswsenenwenenwnenwese
swswswswswwswwswswswsewswswnwswswswwe
ewswneswnwwswwwwwsewwwwwswwww
wwwwwwswwnwnwwwwweewwwww
wswseneswnwneswnwnenenenwwsenwnenwnese
nwnwswneswnwnenwwnenwnwenwnwnwnwwnwese
neenenenenewneneneneneenene
weewneswseneneseneswnenww
sesesesesesweseseeeseesewseewwenw
nwnwnwnwnenwnwnwwwnwsenwnwwnweswnwnw
nwnwnenwwwsenwnwnwnwnwnwnenwnenwnwnwnesene
swseseseseswseseseseseswsesenwnwsesesesese
wwwnwwnwnwnwewwnwwwewswnwww
seswseenwseseeeweee
swswswnenwseswnwseeseswneseseseneesesww
neeneenwswesweneneswswneeneeswene
nwswswweesewwwnenwwewwwneww
neeeeeewseseseeeseseee
sesenesesesweseseeswseswseswwseswnewnww
newenwsewseweseseseseseswseseenesesee
seseeseewenwwseeseseseewneesesewse
nenenenesenwneeneswneneneswneeeneneee
sesesesewseseswseseseseneswseseseswsese
newnwnenenwnweneneeswnenwnwsenwnwnenene
swswswseswswseswswswneseswnwswseseswesw
nwneswswnwnwenwnwnwnwnwnwne
wnewswwsewwenwwwwnwwnwwwnew
swsenweseseeeenwseseeeseseneesewse
wwenwwnwnwwnwseewnwwewsewnw
nenenwnwneseswnenwnenwnenwnenenwnenenwnene
seneswswswswnwswswswswswswseswswswswswse
nwweswwwswseswwwwnewwwwwswwswsw
wwneswnwnwnwenwwsewwnwnenwnwwnwsenesw
sesewnwnwnwswsenewwnenwnwnenwnwnwnwnwnenw
swnwswswswwswswswswneswneseswswswswswsw
wswwneswswnewswswsesesesesesenweswsw
neseseswseseneswwswsesewseseseseswswsese
swewneeswswswswsenw
esenwnenwswsewswswseswseswnwseswswswswse
swewswneeeseseseeeeeweeenenwe
nwwnenenwwneseneseneneewseneneneeswne
nenewnenwenwneweswnwne
eeeeeeeeeseseewweseesenesene
swnwswnwenwewwwswnwswnwwnenenesesw
seenesweswsesweeeenewneneenewesw
seeseeweseseseseseseseseseseseewsese
swswswneseneswswswwweswseswseswswswsw
sweeeeeeeeeneeneneeswene
eswwsenwswswswseswswswswswswwswnwneswsw
sesesesweseeseneeseseseseseseseseswene
enweseseeeseeseseseneeesweeese
wwswwwwwswwewwnenewwwswwww
seewnewenwneseeeweeesweeeseene
seneseneneneenenewneenenenenewnenene
wnweseswswnwnweswnwenwswseeswswswswnw
nwneneneeneneswneneneneneneneneneneswnenene
nwnwnenwnwnwsenwnwswnenwnwnwnwenwnwnese
nenenwneneneneneeneeneeneneneeneswne
nenwneeseenenewsesweneenenenwneeene
neneneeneeseewneeneewwe
wenwsesweseeeenwesewenweesesese
nwnwwnenwnwnwsenwsenwenenwnenwnwnwnwnwnw
swesesewseswswseswswswseneneseseswnwsesw
nwnenwnwnwnwswnwnwnwsenwnenwnwswnwwnwne
eswwswseswsewenwnewwnewseswneswwnwsw
swseswsesesenwseseseseswsese
esweswnwewsenwsewse
wsesewwwwwewnenewnwwswwwneww
swseseswwswswseseswswneseswswsesewswneswsw
swswswswseswseswswswswnwnwswswswseswnesw
nenenesenenenenenenenenenenenenwneneneswne
eseswesewnwseeseneesweewseseenwese
swwswswswnwseswswswswwswsweswswswswswsw
neseeenwnweswenwsewseseneswwswnwsww
swswwswneneswswswswnewswseswswsenw
senwnenenweneneeneswneeneneeseeswnew
nenwnwnenwnwnenwswsenwnenenenenwnwwnwe
nwwnwnwwnwnwwnwnwnwnwnwnwenwwnwnw
swswswseswswswswswnwwseswenwswseswsweswe
wnwwwenwsewswwwwswnewwwnwwww
nwwwnwnewsewwwwnwnwwwnwnw
nwswweswewswswnesw
sewsweseswswswswnwsweswswswswswswswse
ewneneseeseneeeeeneeneenwnwnee
seswswswwseswswseswswswswswneswsese
eeeneeseeeeeseseswesese
neswsenwwenwnwsenwswnenenenenw
eswewswwneenenenenwsenwsewnwenewnw
wwswwswwenwswwewswswwwwwwsw
eenenenwwnesweeenenewseswenesene
seswneneneseswwenwnweneneneenenenene
wenwwnwswwwnwwwnwnenwnwwnwswwe
nenenenenesenwneweneenenenenenenesene
wwwwwwwwswwswwwnwwswswesww
wwsesenwnwnwnwwnwnwnwnwnwnwnwnwsewnw
senewnwenenenenenewneneneneneneneswneene
sesewnwneneneneneneneneewneeneswnw
swwswswswswswswnesw
swweneswnwwnwwneswseswswesewwswse
neneesenewnewneneenenewnenenenenene
nwnwswneswseswnweee
wenewwwseseswnewwwwnwwswswswwnw
swsewnenwenwsenenwse
nenenenenwneneneenenwnesenenenenenesenene
newwwwnwsewwwwwswswwnesewsww
wnwenwwnwwswnwwnwwwwswnwnwenwnw
weswswwwewsweswswwswwwwswww
seneeeseenwsesenwswsweesenwseeeewe
swnwenenenenenenwsenenwnwnenesenenenwnwne
nwnwnweswenwnwnwnwnwnwwnwswwne
swswwswswswseswswswswsweswnwswswswesw
nwnenwnwnwnwneswnwnenenenenenewsenenwnenw
nwsesenenenenenenenenenenenenenenenenwsenew
eseeeeeeeeeeeeweene
senwnwnwwenweswnenwnwneewnwnwwww
swnwsenwswneneneenweswseweneneneee
weeeewseseeeeseeeseseseseee
wnwenwnwenwwneswnwwnwneneeswnwenw
nwsenwnwnwwwnewwnwseneseswnwnwwnenwnw
nwnenenwsenwnenenenwnenenenewnwnwneew
senwnenwnenwnwnwnwnenwnenenwwsenenwnwnw
enwwwnwnenenewneneswenenesewenene
nenweneneseeenenwenwnenewneswseneswe
wneseneneneenwneneneneneswnenenenenenenene
neneneeseeneeneweneeneneenene
eenesweeneneneneneneneneeenene
esweseeneeeeneeneneeeeewnwse
sesesweswseseseswseswnwseswseseseswsese
nwnenwnwwsenwnwnwnwnw
nwenenweneneswsesweeneneewneeene
ewneseseeswseeneeeseew
swswwswswnwswswswswwswseswswswnewswewsw
sesenweseeseeeneneeseweswsesesesw
nwsenwseeswswsewwsenenwwnwenenwswwene
swswnwwseeneswwnwnew
swswswseswseswswnwseswswswseeenwswswsese
swnwneswnwwsweswseswwwnwswwswswewse
nwnwnwnenwewnwnwnwnwnwneneswnwnwnw
seseseswsesesweswseswswseneswwseswswsese
swwweswwneswswwswwwseesenwnewse
wswwnwwsewnewswwwneswnewweswswswne
eseenewneneswsesenwneneeneneswswnenewsw
seswesenwnwnwneswswnewwnenwneseneenew
wsweswswswswswswneswswwwswneswswwswwse
swneneneenewneneneenwwsewneeeneswne
nwneswnenwwswnenwsenwnwnwneenwseenenenwsw
seseswseswseseseneswswsweswswswsewsesw
ewneneseeneeneneswnw
eeseswnwseseeeeeseseswnweseeseee
wwewseswwwswswwwnenewswswwwswsw
nwewwenwswnenwseswwnwwwnewnwnwswe
seswseswsesesweswseswsesenwsesesesesese
swswswwwswwswewswswnewwewswwwsw
swnesesenwewwseseeswnewnenwwenese
swnwwseswenwsenwneseseseseseseenwsesw
nesenewsenwnweswwwwnwneswnwwsenenwne
ewnwnwnwnesenwnenwnwwswnwsenenwenwnw
esesewneseseswsenesenwwseswsesesenenw
seneswwwnwenwseswnwwwwwswswwene
eswwnewnenenwnwnwnenwnwnenwsenwneene
wneseseesenwnwseneneneswswwwwnwswenese
nesenwnwwnesenesenenwneswnwneswnwnwnenene
senesesesewseseseseseseseseseseesesese
neweeeeneenenwswenesweeeswneee
swneneenenenewnenenenwwnenwsweenenene
nwwenwwwseswswwneswwsenewenwnwnw
nenenenenwsewneseneswneswnenenwne
swswswwswsweswswswswswswsw
nenenwsenwnwnenwnwnenenenwnenenenwneswnw
neneswnwnwnwnwnenwnenwenwnwnwnesenwnenw
nwnwswswswewswswswswswswswseswswswswsw
weeeeeeenee
swnwswswsenwseneswseeseswswneseseswswnwse
swswwseseswswneswseswswneswseseneswswsw
weesweneseweeenesewesweneesee
seswswswseseseseseseseneseswsesenwesese
eweneseeeseneewnwneeeneneenese
swwnenwswwnwnwnwnwswwswenweenwnww
nwwwwwswwswwwwwswswswwswswwe
eneenwneseseeneneeenwseneswneenew
sesesewneswnwnenewneseeneswnwnenweew
eswswswsenwnwnwnewswnenwseswnenwnenee
enwwwesweeeeeeeeseeseeeeee
eseeeeeeeseeeneseswwseeeeee
nwnenenenenenwnenenenenwneswsenwse
weseseseeseseseseseseseeeenesewse
neeneeeweeesweeswenweeswnwenw
nwwwneswswwswswewewwseswswswww
nenewnenenwnwnwnwnwnenwnwnenenesenwnwnw
esesesenesewseseneseseswsese
sesweswneneneneswwnewnwnenweenenenwsw
nwneneswnwswswwnewneneneseseenwsewnese
wneseewnwnwenwnesenwwseseswwnewnwe
senwwsweeesenwse
wnewwnewsewwnwseswwwswnewwwsw
nwnwwnwnenwnwnwnenwnwwnwsenenwsenwsenw
enwwwwnwwnwnwwwnww
wwnwnwswewneenw
wnewewswwwwnwswwswswseswswwnesw
wneenweweneeweseeeeseeeseee
eeswswweeeneseeeneenwswseseenw
nenenenwnwnwnwnwnwswnwnewnwnenwnwnwenwne
nwswswsenwseswnesewnwswswswswseswneswsw
seeweeeeeseseseseeneseseseseswse
sesewneseswneseeseswseesewwswsesesw
nweneswsesenewweseswwneseneeseswswe
senwseseswsesesesenwesenwsesesewesesesw
eneneneeenwnwnwnwseseweneswse
neswneswnenenewswnesenwnenenenenwnwnesw
neswswseswswseswswseweswswseswswswswsesww
neeeeeweneesw
eneneeeeneswnene
swswswswneswswswswsewswswswnwseswneswswsw
wnwesweneneseseneweeesewswwwnwe
wnwewwwnwnwwnwwenwsewswnwswww
neeswnenwnenwsenwnenewweswsenenwnwswnw
nwswnwnwwnwnwnenwnwenw
swneswswswswwswswswswswswswwswswsw
swnwwnenwneswneenenenenenenwseeeesesw
wnwnwnwnwnwnwnwnenwnwnwnwnwswwswnwnenwnw
seswswnenwseswnwsesweseseseseswseneesww
seewnwnwswneweswwwenewwewsenwse
swswseswswswswnwswweswseswseneseswswsw
neneneeeneeseeswsweeeewneeeenw
swseswseswswswneneseswneswnwne
neeeneeneneneneneswnwenene
swsewewnwnwnenewnewnwwseewseesese
nwnwnwewnenwenwewnwnwnwnwnwswnwnwnwnw
nwwweeeeseswseseseseesenweseewe
sesesewswsweseseswseswswseseswswneenwnw
nenenenenenenwneneneseswneneneswne";
        public override int Day => 24;
    }
}
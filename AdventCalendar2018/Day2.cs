using System;
using System.Collections.Generic;

namespace AdventCalendar2018
{
    public static class Day2
    {
        private static int nbSingleRepeat;
        private static int nbDoubleRepeat;
        private static List<string> seen = new List<string>();

        private static void MainDay2()
        {
            var lines = Input.Split(Environment.NewLine);
            foreach(var line in lines)
            {
                ScanLine(line);
            }
            Console.WriteLine($"Checksum is {nbSingleRepeat*nbDoubleRepeat}");
  
            var answer = string.Empty;
            foreach (var line in lines)
            {
                foreach (var other in seen)
                {
                    var diffPos = -1;
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (other[i] != line[i])
                        {
                            if (diffPos != -1)
                            {
                                diffPos = -1;
                                break;
                            }

                            diffPos = i;
                        }
                    }

                    if (diffPos>=0)
                    {
                        Console.WriteLine("Found");
                        answer = line.Substring(0, diffPos) + line.Substring(diffPos + 1);
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(answer))
                {
                    break;
                }
                seen.Add(line);
             }
            Console.WriteLine( $"Prototype id is {answer}");
        }

        private static void ScanLine(string line)
        {
            var letterCounts = new Dictionary<char, int>();
            foreach (var car in line)
            {
                if (!letterCounts.ContainsKey(car))
                {
                    letterCounts[car] = 1;
                }
                else
                {
                    letterCounts[car]++;
                }
            }

            if (letterCounts.ContainsValue(2))
            {
                nbSingleRepeat++;
            }

            if (letterCounts.ContainsValue(3))
            {
                nbDoubleRepeat++;
            }
        }

        private const string Input = 
            @"qysdgimlcaghpfozuwejmhrbvx
qysdtiklcagnpfhzuwbjmhrtvx
qysdtiflcsgnpfozuwejmhruvx
qkshtiklnagnpfozuwejmhrbvx
qysdtnklcagnpmozuwejmhrrvx
qysdttkecagnpfozuwijmhrbvx
qyedtiklcagnvfozuweymhrbvx
qyzdtikzcagnpfozuwejmhqbvx
qysdtiklcagnpfozlwedmhqbvx
qjsdtiklcagnpfozubejmhrbvq
qysdtiklcagnpfozvvejmhrbex
qdsdziklcagnpfouuwejmhrbvx
qysttikqccgnpfozuwejmhrbvx
qysdtiklcagnpbozwwecmhrbvx
qysdtiklcagnpfozuwexmmrjvx
nysdtiklcqgjpfozuwejmhrbvx
cysdoiqlcagnpfozuwejmhrbvx
qysdthxlcagnpfozuwejmcrbvx
qyswtiklcrgnpfozuwejmhrbvf
qysdtiklcagnpfozurejmhvpvx
qysdtiklcegnpfdzuwejghrbvx
qysdtjkluagnpfozuwenmhrbvx
qysdtimlcagnpjwzuwejmhrbvx
qyrdtiklcegnpeozuwejmhrbvx
qysdmiklcagnpfokswejmhrbvx
qysdtizlcagnpiozuwecmhrbvx
qysdtiklcignafxzuwejmhrbvx
qycdjiklcagnpzozuwejmhrbvx
qysdtiklcagnpjozuwepihrbvx
qyedtiklcrgnpfozuvejmhrbvx
mysdtikrcagnpfozwwejmhrbvx
qysdtiklcagnpfozuhcjmhrbsx
qmsdtiklcagnpfozuwehmhrevx
qgsdtiklcagnpfozuwejvhrbvp
lysdtikleagnpfozuwejmhrnvx
qxsdtivlzagnpfozuwejmhrbvx
qysdtiklcoggpfozuwebmhrbvx
wysdtiklcagnpfozuwejmornvx
jysdtiklvagntfozuwejmhrbvx
qmsdtiklcagnpfozuwejmrrbax
qysdttklcagnpfoiuwejmhrbvh
qysdtnklcaenpfozupejmhrbvx
qysdtoklcagnpfozuwexmhrbvq
qysdtiklcagnpuoeuwejmhrjvx
iysdtitncagnpfozuwejmhrbvx
qysdtixrcagnprozuwejmhrbvx
qyfdtiplcagnpfouuwejmhrbvx
qysdtmklcagnpfowuwejmhrbox
qysdtiklcagnxiozuwejphrbvx
fysdtiklcagnptozuwejmhrbvo
qysdqiklcagnplozuwejmhmbvx
qysdtwkacagnpfosuwejmhrbvx
qysdtitlcagnpfozufajmhrbvx
qysdtcklcagopfdzuwejmhrbvx
qmfdtdklcagnpfozuwejmhrbvx
qysztiklcaonpfozuwejmhrbfx
qygdtiklcggnpfozuwejmhrhvx
qysdiiklcagnpfozukejmcrbvx
qysdtrkloagnpfozuwujmhrbvx
qycdtiklcagnpfozywejmhrlvx
qgsdtikdcagnpfozgwejmhrbvx
qyudtikleagvpfozuwejmhrbvx
pysdtiqlcagnpfozuwejmarbvx
qysdtiklcaenpfozuwehahrbvx
qhsttiklcagnpfovuwejmhrbvx
zysdtikqmagnpfozuwejmhrbvx
rysdtikacagnpfozuwevmhrbvx
zysntikllagnpfozuwejmhrbvx
qysttimlcagndfozuwejmhrbvx
qysdtiklcaxopfqzuwejmhrbvx
qysdtislcagnpfozuwejmtrbnx
qysdviklcagnpfozswejmhibvx
qmsdtiklrygnpfozuwejmhrbvx
qysztiklcagnpfozuwejmorbrx
xysdtiklcagnzwozuwejmhrbvx
qysjthklcagnpfozowejmhrbvx
qysdtiklcagnpfofxwemmhrbvx
jysdtiklcagnpfozfwehmhrbvx
qysdtgklaagnpfozhwejmhrbvx
qqsdtiklcaenpfozuwejmhrvvx
qysdtikloajppfozuwejmhrbvx
qysdtiklcagnpwozuwejmhrhsx
qpsdtiklcapnprozuwejmhrbvx
qyzdtiklcagnpcozuwejmhrbvc
qusdhiklcagnpfozuwejmhrbxx
qysdtiklcagnpfozqwejthrvvx
qysvtiklcagnpfoiuwedmhrbvx
qgsdtiklcagvpfozuwejmhrbvf
qysdtikxcawnpfozuwejmarbvx
qyvctiklcaynpfozuwejmhrbvx
qyyltiklnagnpfozuwejmhrbvx
oysdtillcagnpfozuwejmnrbvx
qysdtiklcagnpfozuvmjmhrbzx
qykdtikocagnpfhzuwejmhrbvx
qysdtvkloagnpfozuwejmkrbvx
qysetiklcagnpfozuwejmhrxvh
qysdtiklcavnpfuzuwejmhrbvh
qmndtiklcagnpfojuwejmhrbvx
qysdtialcagnpfozuwejmdrqvx
qysdtiklcagnpfozuwejtzrbvv
qysdtiklxagnpyozufejmhrbvx
qysdtiklcagnpfgzewejahrbvx
qysdtiklcagppsozuwejmhrdvx
qykdtiklcainpfozuwejqhrbvx
qysdtiklcagnpfszxwejmhubvx
qyrdtiklcagkptozuwejmhrbvx
qysdsiklcagnpfozsvejmhrbvx
qypdtiklcagypfozuwejmhrlvx
qssdtiklcagnpfozuwqjmirbvx
qyshtiklcagnpfrzuwyjmhrbvx
qysdtiklcagnpfqzuwenmgrbvx
qysdtiklcagnpfonuwejmhkwvx
qysdhiklcagnpfokuwejmhrfvx
jysrtiklcaenpfozuwejmhrbvx
qysdtiilcagnpfozuwejmhcbvl
qysdtiklcagnheozuwejmhrbvn
qysdtikucagwpfojuwejmhrbvx
qysdtinlctgnpfozuwujmhrbvx
qysdtiklcagnpiozuwejmtrbjx
qysktiklcagqpfozuwcjmhrbvx
qysddiklcagnpfozpwejmhrbvh
wysdtiplcagnpfozuwejyhrbvx
qysdtiklcagnpfjzlwejmhrcvx
qysdtikleagopbozuwejmhrbvx
qysdtqklcwgnpfozuwejmirbvx
qysdtiklcugnpmozuwejmhrbvp
qysdtiklcagnpfozpwejmnrbvz
qysdtiklcagnpcozuwejmhbbmx
uysitiklcagnpfozewejmhrbvx
qykdtiklcasnpfozuwejdhrbvx
qyjdtiklcagnpqozuzejmhrbvx
qysdtiklcagaifozuwejmhrbvh
qysdtiklcagnhfozuwyjrhrbvx
qysetiklcaanpfozuwyjmhrbvx
qyfdtiklcagnphozulejmhrbvx
qysdtikkcrgnpfozuwejmhpbvx
qysdtiklcarnpfdzuwejmhrbvq
qysdtiklcfyrpfozuwejmhrbvx
rysdtitlcagnpfoznwejmhrbvx
qysdtiilcagnffozugejmhrbvx
qysdyifloagnpfozuwejmhrbvx
qysdtiklcegnpfozuwejmlrcvx
qysdtiklcagnpfozuwajmhbbqx
qysptrklcarnpfozuwejmhrbvx
qysdtiklcagnldozuwejmhwbvx
qysdtiklczgqpfozuwejmhobvx
qyxdtiklcagcpfoiuwejmhrbvx
qysatiklczgnpfozawejmhrbvx
qysduiklcagnpfoziwejyhrbvx
qysdtgklqagnpfozujejmhrbvx
qysdtiqlcagnpfozzdejmhrbvx
qysdtiklcngnpfofuwejmzrbvx
qysdtiklcagnyfozuwejrnrbvx
qysdtiplcagnpfozowmjmhrbvx
qyswtiklcagnplozuwedmhrbvx
qyseiiklcagnpfozuwejmhibvx
qysdtiklcagnpfozutusmhrbvx
qysdtimlcagnpfozccejmhrbvx
qnsdniklcagnpfobuwejmhrbvx
qysrtiklcagnpfofuwejmhrbyx
qyzdtiklcagnpfoizwejmhrbvx
qysdtjslcdgnpfozuwejmhrbvx
qysdtiklcagnpxoyuwejmrrbvx
qysdtikllagnpfmzuwbjmhrbvx
qysdtitlcagnkfozuwejwhrbvx
qymdtiklcggnpfozuwejmzrbvx
qysdtiklclfnpfozuhejmhrbvx
qysdtyklcagnpfozuwejmhhbix
qysetiklcagnpfozuwejmhrspx
qysdipklcagnpfozuwejmhrbex
uysgtiklcagnpmozuwejmhrbvx
qysdtiklmagnpfozuwqlmhrbvx
qysdtiklcagnyfozxwejmhrmvx
qysutillcagnpfozuwejmhrbbx
casdtiklcagnpfopuwejmhrbvx
qesdtiklctgnpfmzuwejmhrbvx
qysdtiklcagopfozjwejmdrbvx
jzsdtiklcagnpfozuwejmurbvx
qysdtiklcjgnpfonuwejrhrbvx
qysdtiklcrgnpnozuwejmhqbvx
oyhdtiklcagnpfozuwekmhrbvx
qysstiklcagjpfozuwejmhrbnx
qyudtiklsagnpsozuwejmhrbvx
qysdtiilcagnpfozusejmhrbva
qysdtiklcaknpfozmwejmhgbvx
qysdbiklcpgnpfozuwejmrrbvx
qybdtiklcagvpfokuwejmhrbvx
qysatiklcagnpwofuwejmhrbvx
qysdtiklcadnpfonuwejmcrbvx
qysdtijfcagnpfozuvejmhrbvx
qysdtiklcagnpfhluuejmhrbvx
qysdtiklcagnpfoguwejqhrwvx
qlsdtiklcagnpfojuwehmhrbvx
qyhdtiolcagnpfozuwejmhrzvx
qmsdtiklcagnppozuwpjmhrbvx
qysdtiklvvgnpfvzuwejmhrbvx
qysdtiklcagnpfszuwajmhrcvx
qysdtiklcagnpfmzuwekmhrbyx
qysdtiklcagwpfozumevmhrbvx
qysdtaklcagnpfozuwejvhzbvx
qysotiklcagntffzuwejmhrbvx
qysdtiklcagnpfowuweqmhrivx
qysdtrkloagnxfozuwujmhrbvx
qasdiiklcagnpfozuwegmhrbvx
qysbtiklcagnpfozuwejthrbhx
hysdtikllagnpfozuwejmhrbbx
qyqdtiklcagnpsozuwejmcrbvx
qysdtiklcagnpiqzuwejmhrbux
qnsdtiklcagnpfozdwejmhbbvx
qysjbiklcagzpfozuwejmhrbvx
qysdtiklcagnpfifuwejmhrbvg
qysdtiklcaggpkozunejmhrbvx
qxsdtiklcavnpfozuwfjmhrbvx
qysdtikycabnpfkzuwejmhrbvx
qyswtzklcagnpfozuwejmhrlvx
qysdtikqcagnpfozuwejrhnbvx
qysdtiplaagnpfozuwejmhruvx
qjcdtiklcagnpfozujejmhrbvx
nysdtyklcagnpfozutejmhrbvx
qysrtiklcagnpfnzuwejmhrbdx
zysdtielcagnpfozuwezmhrbvx
qysdtikpvagnpfozuzejmhrbvx
qysdwiklcagnpfozueejmhrlvx
dysdmiklcagnpfozuwejzhrbvx
qysdtiklcjgnpfozuweimhmbvx
qysdtiklciynpyozuwejmhrbvx
qksdtiklcagnpbozubejmhrbvx
qysdtiklkagnpfozuwejmhrjvb
yyxdtiklcagnpfomuwejmhrbvx
qysdtiklcagnfnozuwejmhrbvv
qysdtzklcagnpfozuwejmhrmvb
qysduiklclgnpfozuwejmhrbvn
qyndtmklcavnpfozuwejmhrbvx
qisdkiklcagnpfozuwqjmhrbvx
qysdtrkycagypfozuwejmhrbvx
qhsdtiklcwgnmfozuwejmhrbvx
qysdaiklcannpfozupejmhrbvx
zysdtiklcagnpjozuwejmhrbwx
qysdtikxcagnpfozuwejmcrxvx
qysdtzklcagnpfozewejmhrbvk
qysdwtklcagnhfozuwejmhrbvx
qysdtqklcaenpfozuwejmdrbvx
qysdtiklcagnpfozuoeemhqbvx
nysdtikocagnpfozuwejmhwbvx
qysxtiklcagnpfozqwejmhrbax
qysdtielcasnpfozuwejmhsbvx
qysdtiklcaknpfozuwejcwrbvx
qysytiklcagnpfozdfejmhrbvx
qysdtiklcagmpfozuwejmgrbox
qysdtielcagnpfpzuwejhhrbvx";
    }
}
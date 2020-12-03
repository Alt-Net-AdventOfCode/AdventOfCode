using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventCalendar2018
{
    public static class Day24
    {
        internal static bool Log = false;
        private static void MainDay24()
        {
            var immuneGroup = new Family("Immune System");
            var infectionGroup = new Family("Infection");

            var specs = Input;
            
            ParseSpecifications(specs, immuneGroup, infectionGroup);

            Log = true;
            RunGame(immuneGroup, infectionGroup);
            var totalInfection = infectionGroup.TotalPower;
            var totalImmune = immuneGroup.TotalPower;
            Console.WriteLine($"End Game, Immune: {totalImmune}, Infection {totalInfection}");

            var minBoost = 0;
            var maxBoost = 100;
//            var boost = maxBoost;
            var foundMax = false;
            for (var boost = 0; boost<2000; boost++)
            {
                Console.WriteLine($"Trying with boost {boost}");
                immuneGroup.Reset();
                infectionGroup.Reset();
                immuneGroup.Boost = boost;
                RunGame(immuneGroup, infectionGroup);
                if (infectionGroup.IsDead)
                {
                    maxBoost = boost;
                    //Console.WriteLine($"Succeeded, decrease max to {maxBoost}");
                    break;
                }

                //boost = (minBoost + maxBoost)/ 2;
            }
            Console.WriteLine($"Min boost is {maxBoost} .");
            
            Console.WriteLine($"Verifying with boost {maxBoost}");
            immuneGroup.Reset();
            infectionGroup.Reset();
            immuneGroup.Boost = maxBoost;
            RunGame(immuneGroup, infectionGroup);
            totalInfection = infectionGroup.TotalPower;
            totalImmune = immuneGroup.TotalPower;
            Console.WriteLine($"End Game, Immune: {totalImmune}, Infection {totalInfection}");
        }

        private static void ParseSpecifications(string specs, Family immuneGroup, Family infectionGroup)
        {
            var lines = specs.Split(Environment.NewLine);
            Family current = null;
            var parser = new Regex(GeneralPattern);
            var specificsParser = new Regex(SpecificPattern);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("Immune System"))
                {
                    current = immuneGroup;
                    continue;
                }

                if (line.StartsWith("Infection"))
                {
                    current = infectionGroup;
                    continue;
                }

                var match = parser.Match(line);
                if (!match.Success)
                {
                    Console.WriteLine($"Failed to match {line}.");
                    continue;
                }

                var unitsCount = match.ExtractInt(1);
                var unitHPs = match.ExtractInt(2);
                var specifics = match.Extract(3);
                var power = match.ExtractInt(4);
                var attackType = match.Extract(5);
                var initiative = match.ExtractInt(6);
                var weaknesses = new List<string>();
                var immunities = new List<string>();
                // are there immunities or weaknesses
                if (!string.IsNullOrEmpty(specifics))
                {
                    var subMatch = specificsParser.Match(specifics);
                    if (!subMatch.Success)
                    {
                        Console.WriteLine($"Failed to match specific rules: {specifics}");
                        continue;
                    }

                    for (var i = 0; i < 2; i++)
                    {
                        if (subMatch.Groups[i * 3 + 1].Value == "immune")
                        {
                            immunities.AddRange(
                                subMatch.Groups[i * 3 + 2].Value.Split(',').Select(x => x.TrimEnd().TrimStart()));
                        }

                        if (subMatch.Groups[i * 3 + 1].Value == "weak")
                        {
                            weaknesses.AddRange(
                                subMatch.Groups[i * 3 + 2].Value.Split(',').Select(x => x.TrimEnd().TrimStart()));
                        }
                    }
                }

                var unit = new Unit(unitsCount, unitHPs, power, attackType, initiative, weaknesses, immunities);
                current.AddUnit(unit);

                Console.WriteLine(unit);
            }
        }

        private static void RunGame(Family immuneGroup, Family infectionGroup)
        {
            var i = 3;
            for (; !immuneGroup.IsDead && !infectionGroup.IsDead;)
            {
                var pairs = new List<(Unit attacking, Unit defending)>();
                if (Log)
                {
                    i--;
                    if (i == 0)
                    {
                        Log = false;
                    }
                    Console.WriteLine();
                }
                
                pairs.AddRange(infectionGroup.IdentifyCombat(immuneGroup));
                pairs.AddRange(immuneGroup.IdentifyCombat(infectionGroup));
                if (Log)
                {
                    Console.WriteLine();
                }
                var totalKills = 0;

                var valueTuples = pairs.OrderByDescending(x => x.attacking.Initiative).ToList();
                foreach (var (attacking, defending) in valueTuples)
                {
                    totalKills+= attacking.Attack(defending);
                }

                if (totalKills == 0)
                {
                    Console.WriteLine("Stalemate");
                    return;
                }
                if (Log)
                {
                    Console.WriteLine();
                    Console.Write(immuneGroup);
                    Console.Write(infectionGroup);
                }  
            }
        }

        private static int ExtractInt(this Match match, int id)
        {
            return int.Parse(match.Extract(id));
        }

        private static string Extract(this Match match, int id)
        {
            return match.Groups[id].Value;
        }

        private const string GeneralPattern = "(\\d+) units each with (\\d+) hit points (\\([^\\)]+\\) )?with an attack that does (\\d+) (\\w+) damage at initiative (\\d+)";
        private const string SpecificPattern = "\\((immune|weak) to ([\\w, ]+)(; (immune|weak) to ([\\w, ]+))?\\) ";

        private const string Demo = 
            @"Immune System:
17 units each with 5390 hit points (weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
989 units each with 1274 hit points (immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3

Infection:
801 units each with 4706 hit points (weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
4485 units each with 2961 hit points (immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4";
        
        private const string Input =
            @"Immune System:
3578 units each with 3874 hit points (immune to radiation) with an attack that does 10 bludgeoning damage at initiative 17
865 units each with 10940 hit points (weak to bludgeoning, cold) with an attack that does 94 cold damage at initiative 19
3088 units each with 14516 hit points (immune to cold) with an attack that does 32 bludgeoning damage at initiative 4
2119 units each with 6577 hit points (immune to slashing, fire; weak to cold) with an attack that does 22 bludgeoning damage at initiative 6
90 units each with 2089 hit points (immune to bludgeoning) with an attack that does 213 cold damage at initiative 14
1341 units each with 4768 hit points (immune to bludgeoning, radiation, cold) with an attack that does 34 bludgeoning damage at initiative 1
2846 units each with 5321 hit points (immune to cold) with an attack that does 17 cold damage at initiative 13
4727 units each with 7721 hit points (weak to radiation) with an attack that does 15 fire damage at initiative 10
1113 units each with 11891 hit points (immune to cold; weak to fire) with an attack that does 80 fire damage at initiative 18
887 units each with 5712 hit points (weak to bludgeoning) with an attack that does 55 slashing damage at initiative 15

Infection:
3689 units each with 32043 hit points (weak to cold, fire; immune to slashing) with an attack that does 16 cold damage at initiative 7
33 units each with 10879 hit points (weak to slashing) with an attack that does 588 slashing damage at initiative 12
2026 units each with 49122 hit points (weak to bludgeoning) with an attack that does 46 fire damage at initiative 16
7199 units each with 9010 hit points (immune to radiation, bludgeoning; weak to slashing) with an attack that does 2 slashing damage at initiative 8
2321 units each with 35348 hit points (weak to cold) with an attack that does 29 radiation damage at initiative 20
484 units each with 21952 hit points with an attack that does 84 radiation damage at initiative 9
2531 units each with 24340 hit points with an attack that does 18 fire damage at initiative 3
54 units each with 31919 hit points (immune to bludgeoning, cold) with an attack that does 1178 radiation damage at initiative 5
1137 units each with 8211 hit points (immune to slashing, radiation, bludgeoning; weak to cold) with an attack that does 14 bludgeoning damage at initiative 11
2804 units each with 17948 hit points with an attack that does 11 radiation damage at initiative 2";
    }

    internal class Family
    {
        private readonly string label;
        private int boost;
        private List<Unit> units = new List<Unit>();

        public bool IsDead => units.Count(unit => unit.HasUnits) == 0;
        
        public int TotalPower => units.Sum(unit => unit.Count);

        public int Boost
        {
            get => boost;
            set => boost = value;
        }

        public List<Unit> Units
        {
            get => units;
            set => units = value;
        }

        public Family(string label) 
        {
            this.label = label;
        }

        public void AddUnit(Unit unit)
        {
            unit.Id = $"{label} group #{units.Count + 1}";
            unit.Group = this;
            units.Add(unit);
        }

        public void Reset()
        {
            foreach (var unit in units)
            {
                unit.Reset();
            }
        }
        
        public IEnumerable<(Unit attacker, Unit defendant)> IdentifyCombat(Family group)
        {
            var targets = group.AliveUnits();
            var pairs = new List<(Unit, Unit)>();
            var orderedEnumerable = AliveUnits().ToList();
            orderedEnumerable.Sort((unit, unit1) =>
            {
                
                var ret = unit.EffectivePower.CompareTo(unit1.EffectivePower);
                return ret == 0 ? unit.Initiative.CompareTo(unit1.Initiative) : ret;
            });
            orderedEnumerable.Reverse();
            foreach(var unit in orderedEnumerable)
            {
                if (!unit.HasUnits)
                    continue;
                var target = unit.SelectTarget(targets);
                if (target == null)
                {
                    continue;
                }

                if (!targets.Remove(target))
                {
                    Console.WriteLine("failed to remove target");
                }
                pairs.Add((unit, target));
            }

            return pairs;
        }

        public List<Unit> AliveUnits()
        {
            return units.Where(x => x.HasUnits).ToList();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{label}:");
            if (IsDead)
            {
                builder.AppendLine("No groups remain.");
            }
            else
            {
                foreach (var unit in units)
                {
                    if (!unit.HasUnits)
                        continue;
                    builder.AppendLine($"{unit.Id} contains {unit.Count} units.");
                }
            }

            return builder.ToString();
        }
    }
    
    internal class Unit
    {
        public string Id { get; set; }
        public Family Group { get; set; }

        private readonly int initCount;
        private int count;
        private readonly int hp;
        private readonly int power;
        private readonly string attackType;
        private readonly int initiative;
        private readonly List<string> weaknesses;
        private readonly List<string> immunities;

        public int EffectivePower => (power+Group.Boost) * count;
        public int Count => count;

        public int Initiative => initiative;

        public bool HasUnits => count > 0;

        public Unit(int count, int HP, int power, string attackType, int initiative, List<string> weaknesses, List<string> immunities)
        {
            this.count = count;
            hp = HP;
            this.power = power;
            this.attackType = attackType;
            this.initiative = initiative;
            this.weaknesses = weaknesses;
            this.immunities = immunities;
            initCount = count;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            string imDesc;
            if (immunities.Count > 0)
            {
                builder.Append("Immune to:");
                foreach (var immunity in immunities)
                {
                    builder.Append($"{immunity} ");
                }
                imDesc = builder.ToString();
                builder.Clear();
            }
            else
            {
                imDesc = "No immunity";
            }

            string wkDesc;
            if (weaknesses.Count > 0)
            {
                builder.Append("Weak to:");
                foreach (var weakness in weaknesses)
                {
                    builder.Append($"{weakness} ");
                }

                wkDesc = builder.ToString();
            }
            else
            {
                wkDesc = "No weakness";
            }

            return $"{Id}: EffectivePower: {EffectivePower}, {nameof(count)}: {count}, {nameof(hp)}: {hp}, {nameof(power)}: {power}, {nameof(attackType)}: {attackType}, {nameof(initiative)}: {initiative}, {wkDesc}, {imDesc}";
        }

        public Unit SelectTarget(IEnumerable<Unit> targets)
        {
            Unit target = null;
            var maxDamage = 0;

            var orderedTargets = targets.Where(unit => unit.DamageFactor(attackType)!=0).ToList();
            if (orderedTargets.Count == 0)
                return null;
            orderedTargets.Sort((unit, unit1) =>
            {
                var first = unit.DamageFactor(attackType).CompareTo(unit1.DamageFactor(attackType));
                if (first != 0)
                    return first;
                first = unit.EffectivePower.CompareTo(unit1.EffectivePower);
                if (first != 0)
                    return first;
                return unit.Initiative.CompareTo(unit1.Initiative);
            });

            orderedTargets.Reverse();
            return orderedTargets[0];
        }

        private int DamageFactor(string type)
        {
            if (immunities.Contains(type))
                return 0;
            return weaknesses.Contains(type) ? 2 : 1;
        }

        public int Attack(Unit second)
        {
            var kills = second.TakeDamage(EffectivePower, attackType);
            if (Day24.Log)
            {
                Console.WriteLine($"{Id} attacks {second.Id},  killing {kills} units.");
            }
            return kills;
        }

        private int TakeDamage(int effectivePower, string type)
        {
            effectivePower *= DamageFactor(type);
            var killed = Math.Min(effectivePower / hp, count);
            count -= killed;
            return killed;
        }

        public void Reset()
        {
            count = initCount;
        }
    }

}
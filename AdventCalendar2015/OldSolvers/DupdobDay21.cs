using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2015.OldSolvers
{
    public class DupdobDay21: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            var val = int.Parse(line.Split(':')[1]);
            if (line.StartsWith("Hit Points"))
            {
                _monsterHitPoints = val;
            }
            else if (line.StartsWith("Damage"))
            {
                _monsterDamage = val;
            }
            else if (line.StartsWith("Armor"))
            {
                _monsterArmor = val;
            }
        }

        public override object GiveAnswer1()
        {
            var minCost = int.MaxValue;
            IEnumerable<(int cost, int weapon, int armor)> winning = null;
            foreach (var combination in Combinations())
            {
                var cost = 0;
                var weapon = 0;
                var armor = 0;
                foreach (var (deltaCost, deltaWeapon, deltaArmor) in combination)
                {
                    cost += deltaCost;
                    weapon += deltaWeapon;
                    armor += deltaArmor;
                }

                if (Math.Max(1, weapon - _monsterArmor) >= Math.Max(1, _monsterDamage - armor))
                {
                    if (cost < minCost)
                    {
                        minCost = cost;
                        winning = combination;
                    }
                }
            }

            return minCost;
        }

        public override object GiveAnswer2()
        {
            var maxCost = 0;
            IEnumerable<(int cost, int weapon, int armor)> winning = null;
            foreach (var combination in Combinations())
            {
                var cost = 0;
                var weapon = 0;
                var armor = 0;
                foreach (var (deltaCost, deltaWeapon, deltaArmor) in combination)
                {
                    cost += deltaCost;
                    weapon += deltaWeapon;
                    armor += deltaArmor;
                }

                if (Math.Max(1, weapon - _monsterArmor) < Math.Max(1, _monsterDamage - armor))
                {
                    if (cost > maxCost)
                    {
                        maxCost = cost;
                        winning = combination;
                    }
                }
            }

            return maxCost;
        }

        IEnumerable<IEnumerable<(int cost, int armor, int weapon)>> Combinations()
        {
            foreach (var weapon in Weapons)
            {
                foreach (var armorAndAddOns in ArmorAndCombination())
                {
                    yield return armorAndAddOns.Append(weapon);
                }
            }
        }

        private IEnumerable<IEnumerable<(int, int, int)>> ArmorAndCombination()
        {
            foreach (var armor in Armors)
            {
                foreach (var addOns in AddOnsCombination())
                {
                    yield return addOns.Append(armor);
                }
            }
            foreach (var addOns in AddOnsCombination())
            {
                yield return addOns;
            }
        }

        private IEnumerable<IEnumerable<(int, int, int)>> AddOnsCombination()
        {
            yield return Array.Empty<(int, int, int)>();
            for (var i = 0; i < AddOns.Length; i++)
            {
                yield return new[] {AddOns[i]};
                for (var j = i + 1; j < AddOns.Length; j++)
                {
                    yield return new[] {AddOns[i], AddOns[j]};
                }
            }
        }

        protected override void SetupTestData()
        {
        }

        protected override void CleanUp()
        {
        }

        private int _monsterHitPoints;
        private int _monsterDamage;
        private int _monsterArmor;

        private static readonly (int cost, int damage, int armor)[] Weapons = {(8, 4, 0), (10, 5, 0), (25,6, 0), (40, 7, 0), (74, 8, 0)};
        private static readonly (int cost, int damage, int armor)[] Armors = {(13, 0, 1), (31, 0, 2), (53, 0, 3), (75, 0, 4), (102, 0, 5)};
        private static readonly (int cost, int damage, int armor)[] AddOns = {(25, 1, 0), (50, 2, 0), (100, 3, 0), (20, 0, 1), (40, 0, 2), (80, 0, 3)};
        protected override string Input => @"Hit Points: 100
Damage: 8
Armor: 2";

        public override int Day => 21;
    }
}
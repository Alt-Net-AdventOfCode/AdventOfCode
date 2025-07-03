// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
// ---
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using AoC;

namespace AdventCalendar2015;

[Day(21)]
public class DupdobDay21:SolverWithParser
{
    private (int cost, int damage, int armor)[] _weapons =
    [
        (8, 4, 0), // Dagger
        (10, 5, 0), // Shortsword
        (25, 6, 0), // Warhammer
        (40, 7, 0), // Longsword
        (74, 8, 0) // Greataxe
    ];
    private (int cost, int damage, int armor)[] _armors =
    [
        (0, 0, 0), // No armor
        (13, 0, 1), // Leather
        (31, 0, 2), // Chainmail
        (53, 0, 3), // Splintmail
        (75, 0, 4), // Bandedmail
        (102, 0, 5) // Platemail
    ];
    
    private (int cost, int damage, int armor)[] _rings =
    [
        (0, 0, 0), // No ring
        (25, 1, 0), // Damage +1
        (50, 2, 0), // Damage +2
        (100, 3, 0), // Damage +3
        (20, 0, 1), // Armor +1
        (40, 0, 2), // Armor +2
        (80, 0, 3) // Armor +3
    ];
    private int _monsterHitPoints;
    private int _monsterDamage;
    private int _monsterArmor;
    private readonly List<(int price, int damage, int armor)> _equipmentList;

    public DupdobDay21()
    {
        // we will build all combinations of weapons, armors and rings
        _equipmentList = [];
        foreach (var weapon in _weapons)
        {
            foreach (var armor in _armors)
            {
                foreach (var ring1 in _rings)
                {
                    foreach (var ring2 in _rings)
                    {
                        if (ring1 == ring2) continue; // cannot use the same ring twice
                        var totalCost = weapon.cost + armor.cost + ring1.cost + ring2.cost;
                        var totalDamage = weapon.damage + armor.damage + ring1.damage + ring2.damage;
                        var totalArmor = weapon.armor + armor.armor + ring1.armor + ring2.armor;
                        _equipmentList.Add((totalCost, totalDamage, totalArmor));
                    }
                }
            }
        }

    }
    
    protected override void Parse(string data)
    {
        foreach (var line in data.SplitLines())
        {
            var parts = line.Split(':');
            if (parts.Length != 2) continue;
            var key = parts[0].Trim();
            var value = int.Parse(parts[1].Trim());
            switch (key)
            {
                case "Hit Points":
                    _monsterHitPoints = value;
                    break;
                case "Damage":
                    _monsterDamage = value;
                    break;
                case "Armor":
                    _monsterArmor = value;
                    break;
                default:
                    throw new ArgumentException($"Unknown key: {key}");
            }
        }
    }

    public override object GetAnswer1()
    {
        // order by price
        _equipmentList.Sort((a, b) => a.price.CompareTo(b.price));
        foreach (var (price, damage, armor)  in _equipmentList)
        {
            if (MatchMonster(100, damage, armor, _monsterHitPoints, _monsterDamage, _monsterArmor))
            {
                return price; // return the first one that beats the monster
            }
        }

        return -1; // no solution found
    }

    [UnitTest(true, 8, 5, 5, 12, 7, 2)]
    private static bool MatchMonster(int hitPoints, int damage, int armor, int monsterHitPoints, int monsterDamage, int monsterArmor)
    {
        while (hitPoints>0)
        {
            // player attacks monster
            monsterHitPoints -= Math.Max(1, damage - monsterArmor);
            if (monsterHitPoints <= 0)
            {
                return true; // we win
            }
            // monster attacks player
            hitPoints -= Math.Max(1, monsterDamage - armor);
        }
        // we die
        return false;
    }

    public override object GetAnswer2()
    {
        // order by decreasing price
        _equipmentList.Sort((a, b) => -a.price.CompareTo(b.price));
        foreach (var (price, damage, armor)  in _equipmentList)
        {
            // we want the most expensive one that loses
            if (!MatchMonster(100, damage, armor, _monsterHitPoints, _monsterDamage, _monsterArmor))
            {
                return price; // return the first one that beats the monster
            }
        }

        return -1;
    }
}
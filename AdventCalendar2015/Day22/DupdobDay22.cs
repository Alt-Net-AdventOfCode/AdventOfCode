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
using System.Collections.ObjectModel;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(22)]
public class DupdobDay22 : SolverWithParser
{
    private int _monsterHitPoints;
    private int _monsterDamage;

    // game state
    private record GameState(int PlayerArmor, int PlayerMana, int SpentMana, int PlayerHitPoints, int MonsterHitPoints);

    private record Spell(int Cost, int Duration, Func<GameState, GameState> Effect);

    private static readonly Dictionary<string, Spell> SpellBook = new()
    {
        ["Magic Missile"] = new Spell(53, 1,
            effect => effect with
            {
                 MonsterHitPoints = effect.MonsterHitPoints - 4 
            }),
        ["Drain"] = new Spell(73, 1,
            effect => effect with
            {
                    MonsterHitPoints = effect.MonsterHitPoints - 2,
                    PlayerHitPoints = effect.PlayerHitPoints + 2
            }),
        ["Shield"] = new Spell(113, 6,
            effect => effect with { PlayerArmor = 7 }),
        ["Poison"] = new Spell(173,
            6,
            effect => effect with
            {
                    MonsterHitPoints = effect.MonsterHitPoints - 3
            }),
        ["Recharge"] = new Spell(229, 5, effect =>
            effect with {  PlayerMana = effect.PlayerMana + 101 }),
    };
    
    private readonly int _minSpellCost = SpellBook.Min(s => s.Value.Cost);

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
                default:
                    throw new ArgumentException($"Unknown key: {key}");
            }
        }
    }

    private int PlayGame(bool playerRound, int currentMinSpent, GameState state, IDictionary<string, int> effects)
    {
        if (effects == null) throw new ArgumentNullException(nameof(effects));
        while (state.SpentMana < currentMinSpent)
        {
            var nextEffects = new OrderedDictionary<string, int>(effects.Count);
            state = state with { PlayerArmor = 0 };
            
            // apply effect
            foreach (var (spell, timer) in effects)
            {
                if (spell == "Wounded")
                {
                    if (state.PlayerHitPoints <= 1)
                    {
                        // player is dead
                        return int.MaxValue;
                    }
                    state = state with { PlayerHitPoints = state.PlayerHitPoints - 1 };
                    nextEffects[spell] = 1;
                    continue;
                }
                else if (timer > 1)
                {
                    // keep the effect for next turn
                    nextEffects[spell] = timer - 1;
                }

                state = SpellBook[spell].Effect(state);
            }
            
            if (state.MonsterHitPoints <= 0)
            {
                // monster is dead
                return state.SpentMana;
            }
            if (state.PlayerHitPoints <= 0)
            {
                // player is dead
                return int.MaxValue;
            }            
            if (playerRound)
            {
                if (state.PlayerMana < _minSpellCost)
                {
                    // can't cast any more spell
                    return int.MaxValue;
                }

                // try all spell
                foreach (var spellName in SpellBook.Keys)
                {
                    // Spell not in effect
                    if (nextEffects.ContainsKey(spellName)) continue;
                    var spell = SpellBook[spellName];
                    // enough mana
                    if (state.PlayerMana < spell.Cost) continue;
                    var spent = state.SpentMana + spell.Cost;
                    var newState = state with { PlayerMana = state.PlayerMana - spell.Cost, SpentMana = spent };
                    if (spell.Duration == 1)
                    {
                        // apply now
                        newState = spell.Effect(newState);
                        if (newState.MonsterHitPoints <= 0)
                        {
                            // monster is dead
                            return newState.SpentMana;
                        }
                    }
                    else
                    {
                        // add to effects
                        nextEffects.Add(spellName, spell.Duration);
                    }

                    currentMinSpent = Math.Min(currentMinSpent, PlayGame(false, currentMinSpent, newState, nextEffects));
                    nextEffects.Remove(spellName);
                }

                return currentMinSpent;
            }

            // take monster Damage
            state = state with { PlayerHitPoints = state.PlayerHitPoints - Math.Max(1, _monsterDamage - state.PlayerArmor) };
            if (state.PlayerHitPoints <= 0) return int.MaxValue;
            playerRound = true;
            effects = nextEffects;
        }

        return int.MaxValue;
    }

    public override object GetAnswer1() => PlayGame(true, int.MaxValue, new GameState(0, 500, 0, 50, _monsterHitPoints), new Dictionary<string, int>());

    public override object GetAnswer2() => PlayGame(true, int.MaxValue, new GameState(0, 500, 0, 50, _monsterHitPoints), new Dictionary<string, int>(){["Wounded"] = 1});
}
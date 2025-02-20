using System;
using System.Collections.Generic;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay22 : DupdobDayWithTest
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
        }

        public override object GiveAnswer1()
        {
            return PlayGame(true, int.MaxValue, new GameState(InitialPlayerMana,  0, InitialPlayerHitPoints, _monsterHitPoints),
                new Dictionary<string, int>());
        }

        // 1269 too low
        // 1309 too high
        public override object GiveAnswer2()
        {
            _wounded = true;
            return PlayGame(true, int.MaxValue, new GameState(InitialPlayerMana,  0, InitialPlayerHitPoints, _monsterHitPoints),
                new Dictionary<string, int>());
        }

        private int PlayGame(bool playerRound, int currentMinSpent, GameState state, IReadOnlyDictionary<string, int> effects)
        {
            var nextEffects = new Dictionary<string, int>(effects.Count);
            if (state.SpentMana >= currentMinSpent)
            {
                return int.MaxValue;
            }
            if (_wounded && playerRound)
            {
                state= state with {PlayerHitPoints = state.PlayerHitPoints - 1};
                if (state.PlayerHitPoints <= 0)
                {
                    return int.MaxValue;
                }
            }
            var effect = new SpellEffect(0, state);

            // apply effect
            foreach (var (spell, timer) in effects)
            {
                if (timer > 1)
                {
                    // keep the effect for next turn
                    nextEffects[spell] = timer - 1;
                }

                effect = SpellBook[spell].Effect(effect);
                if (effect.State.MonsterHitPoints<=0)
                {
                    // monster is dead
                    return state.SpentMana;
                }
            }

            if (effect.State.PlayerHitPoints <= 0)
            {
                // player is dead
                return int.MaxValue;
            }
            
            state = effect.State;
            if (playerRound)
            {
                if (state.PlayerMana < _minSpellCost)
                {
                    // can't cast any spell
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
                    var spent = effect.State.SpentMana + spell.Cost;
                    var newState = effect.State with{PlayerMana = state.PlayerMana - spell.Cost, SpentMana =  spent};
                    if (spell.Duration == 1)
                    {
                        // apply now
                        newState = spell.Effect(effect with{State = newState}).State;
                        if (newState.MonsterHitPoints<=0)
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
                    currentMinSpent = Math.Min(currentMinSpent, 
                        PlayGame(false,  currentMinSpent,
                        newState , nextEffects));
                    nextEffects.Remove(spellName);
                }

                return currentMinSpent;
            }
            // take monster Damage
            state = state with {PlayerHitPoints = state.PlayerHitPoints - Math.Max(1, _monsterDamage - effect.Armor)};
            return state.PlayerHitPoints <= 0 ? int.MaxValue : PlayGame(true, currentMinSpent, state, nextEffects);
        }
        
        protected override void SetupTestData()
        {
        }

        protected override void CleanUp()
        {
        }

        private int _monsterHitPoints;
        private int _monsterDamage;
        private const int InitialPlayerHitPoints = 50;
        private const int InitialPlayerMana = 500;

        private readonly int _minSpellCost = SpellBook.Min(s => s.Value.Cost);

        private record Spell(int Cost, int Duration, Func<SpellEffect, SpellEffect> Effect);

        private static readonly Dictionary<string, Spell> SpellBook = new()
        { ["Magic Missile"] = new(53, 1, 
                effect => effect with {State = effect.State with {MonsterHitPoints = effect.State.MonsterHitPoints - 4}}),
           ["Drain"] = new (73,  1, 
               effect => effect with {State = effect.State with { MonsterHitPoints = effect.State.MonsterHitPoints - 2 , PlayerHitPoints = effect.State.PlayerHitPoints + 2}}),
           ["Shield"] = new (113, 6, 
               effect => effect with{Armor = 7}),
           ["Poison"] = new (173, 6, 
               effect => effect with {State = effect.State with { MonsterHitPoints = effect.State.MonsterHitPoints-3}}),
           ["Recharge"] = new (229, 5, effect => 
               effect with{State = effect.State with{PlayerMana = effect.State.PlayerMana + 101}}),
        };

        private record GameState(int PlayerMana, int SpentMana, int PlayerHitPoints, int MonsterHitPoints);
        private record SpellEffect(int Armor, GameState State);

        private bool _wounded;
        protected override string Input => @"Hit Points: 55
Damage: 8";
        public override int Day => 22;
    }
}
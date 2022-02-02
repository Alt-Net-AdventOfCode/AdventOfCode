using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2016
{
    public class DupdobDay11: SolverWithLineParser
    {
        private class State : IEquatable<State>
        {
            private readonly int _elevator;
            private readonly int _hashCode;
            public readonly ImmutableSortedSet<string>[] Building;

            public State(int elevator, ImmutableSortedSet<string>[] building)
            {
                _elevator = elevator;
                Building = building;
                
                var hashCode = ComputeHashCode(building);

                _hashCode = hashCode;
            }

            public IEnumerable<State> PossibleMoves()
            {
                // we must identify which components can be moved
                // we can move any chip or pair of chips
                ImmutableSortedSet<string>[] tentativeBuilding;
                IEnumerable<string> start;
                IEnumerable<string> end;
                if (_elevator < 3)
                {
                    foreach (var fSubList in SubLists(Building[_elevator], true))
                    {
                        if (fSubList.Length>1 && !ListIsStable(fSubList))
                            continue;
                        start = Building[_elevator].Except(fSubList);
                        if (!ListIsStable(start))
                        {
                            continue;
                        }
                        end = Building[_elevator + 1].Union(fSubList);
                        if (!ListIsStable(end))
                        {
                            continue;
                        }

                        tentativeBuilding = new ImmutableSortedSet<string>[4];
                        for (var i = 0; i < tentativeBuilding.Length; i++)
                        {
                            if (i < _elevator || i > _elevator+1)
                            {
                                tentativeBuilding[i] = Building[i];
                            }
                        }
                        tentativeBuilding[_elevator] = start.ToImmutableSortedSet();
                        tentativeBuilding[_elevator + 1] = end.ToImmutableSortedSet();
                        yield return new State(_elevator + 1, tentativeBuilding);
                    }
                }

                var lowerFloorsEmpty = true;
                for (var i = 0; i < _elevator; i++)
                {
                    if (Building[i].IsEmpty) continue;
                    lowerFloorsEmpty = false;
                    break;
                }

                if (lowerFloorsEmpty)
                {
                    yield break;
                }
                foreach (var fSubList in SubLists(Building[_elevator], false))
                {
                    if (fSubList.Length>1 && !ListIsStable(fSubList))
                        continue;
                    start = Building[_elevator].Except(fSubList);
                    if (!ListIsStable(start))
                    {
                        continue;
                    }
                    end = Building[_elevator - 1].Union(fSubList);
                    if (!ListIsStable(end))
                    {
                        continue;
                    }

                    tentativeBuilding = new ImmutableSortedSet<string>[4];
                    for (var i = 0; i < tentativeBuilding.Length; i++)
                    {
                        if (i < _elevator-1 || i > _elevator)
                        {
                            tentativeBuilding[i] = Building[i];
                        }
                    }
                    tentativeBuilding[_elevator] = start.ToImmutableSortedSet();
                    tentativeBuilding[_elevator - 1] = end.ToImmutableSortedSet();
                    yield return new State(_elevator - 1, tentativeBuilding);
                }
            }

            public bool IsEnd()
            {
                for (var i = 0; i < 3; i++)
                {
                    if (Building[i].Count > 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            private static bool IsMicrochip(string item)
            {
                return item.EndsWith("-compatible");
            }

            private static bool ListIsStable(IEnumerable<string> floor)
            {
                var rtgs = floor.Where(p => !IsMicrochip(p)).ToArray();
                if (rtgs.Length == 0)
                    return true;
                var microchips = floor.Where(IsMicrochip);
                return microchips.All(c => rtgs.Any(c.StartsWith));
            }

            private static int ComputeHashCode(IReadOnlyList<ImmutableSortedSet<string>> building)
            {
                var globalHashcode = 0;
                unchecked
                {
                    for (var index = 0; index < building.Count; index++)
                    {
                        var t = building[index];
                        var hashCode = 0;
                        if (t == null)
                        {
                            continue;
                        }

                        // either there is only microchips, or there is rtgs and pairs
                        var rtgs = t.Where(i => !IsMicrochip(i));
                        var chips = t.Where(IsMicrochip).ToHashSet();
                        if (!rtgs.Any())
                        {
                            foreach (var chip in t)
                            {
                                hashCode += chip.GetHashCode();
                            }
                        }
                        else
                        {
                            foreach (var rtg in rtgs.Where(r => !chips.Contains(r + "-compatible")))
                            {
                                hashCode += rtg.GetHashCode();
                            }

                            // all chips have corresponding generator, we inject a variation for this
                            hashCode += chips.Count;
                        }

                        globalHashcode += hashCode * (4 - index);
                    }
                }

                return globalHashcode;
            }

            public bool Equals(State other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                if (_elevator != other._elevator)
                {
                    return false;
                }

                for (var index = 0; index < Building.Length; index++)
                {
                    var thisFloor = Building[index];
                    if (thisFloor == null)
                    {
                        continue;
                    }

                    var otherFloor = other.Building[index];
                    if (thisFloor.Count != otherFloor.Count)
                    {
                        return false;
                    }
                    // either there is only microchips, or there is rtgs and pairs
                    var chips = thisFloor.Where(IsMicrochip).ToHashSet();
                    if (chips.Count==thisFloor.Count)
                    {
                        // only microchips
                        if (!thisFloor.All(otherFloor.Contains))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var soloRtgs = thisFloor.Where(i => !IsMicrochip(i) && !chips.Contains(i+"-compatible")).ToList();
                        var otherSoloRtgs = otherFloor.Where(r => !IsMicrochip(r) && !otherFloor.Contains(r + "-compatible")).ToHashSet();
                        if (soloRtgs.Count != otherSoloRtgs.Count || !soloRtgs.All(otherSoloRtgs.Contains))
                        {
                            return false;
                        }
                    }
                }


                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == this.GetType() && Equals((State)obj);
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public static bool operator ==(State left, State right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(State left, State right)
            {
                return !Equals(left, right);
            }

            public override string ToString()   
            {
                var text = new StringBuilder();
                for (var i = 3; i >= 0; i--)
                {
                    text.Append(_elevator == i ? "E*" : " *");
                    text.AppendJoin(", ", Building[i].Select(t => t[..2]+(IsMicrochip(t) ? 'm' : 'g')));
                    text.AppendLine();
                }

                return text.ToString();
            }
        }

        private static IEnumerable<string[]> SubLists(ImmutableSortedSet<string> main, bool pairFirst)
        {
            if (!pairFirst)
            {
                for (var i = 0; i < main.Count; i++)
                {
                    var first = main.ElementAt(i);
                    yield return new[] { first };
                }
            }
            for (var i = 0; i < main.Count; i++)
            {
                var first = main.ElementAt(i);
                for (var j = i+1; j < main.Count; j++)
                {
                    yield return new[] { first, main.ElementAt(j) };
                }
            }

            if (!pairFirst)
            {
                yield break;
            }

            for (var i = 0; i < main.Count; i++)
            {
                var first = main.ElementAt(i);
                yield return new[] { first };
            }
        }

        public override void SetupRun(Engine engine)
        {
            engine.Day = 11;
            engine.RegisterTestData(@"The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
The second floor contains a hydrogen generator.
The third floor contains a lithium generator.
The fourth floor contains nothing relevant.");
            engine.RegisterTestResult(11);
        }

        private readonly Regex _basic = new("The (\\w*) floor contains (.*).");
        private readonly Regex _microchip = new("a (.*-compatible) microchip");
        private readonly Regex _generator = new("a (.*) generator");
        
        private State _state;
        
        protected override void ParseLine(string line, int index, int lineCount)
        {
            var match = _basic.Match(line);
            if (!match.Success)
            {
                throw new ApplicationException($"Failed to parse {line}");
            }

            // initialize the floor
            var level = match.Groups[1].Value switch
            {
                "first" => 0, "second" => 1, "third" => 2, "fourth" => 3, _ => -1
            };
            var floors = _state?.Building ?? new ImmutableSortedSet<string>[4];
            var items = new List<string>();
            var itemsList = match.Groups[2].Value.Replace(" and ", ", ").
                Split(',', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries);
            if (itemsList.Length != 1 || itemsList[0] != "nothing relevant")
            {
                foreach (var item in itemsList)
                {
                    match = _microchip.Match(item);
                    if (!match.Success)
                    {
                        match = _generator.Match(item);
                        if (!match.Success)
                        {
                            throw new ApplicationException($"Failed to parse {item}");
                        }
                        items.Add(match.Groups[1].Value);
                    }
                    else
                    {
                        items.Add(match.Groups[1].Value);
                    }
                }
            }
            floors[level] = items.ToImmutableSortedSet();
            _state = new State(0, floors);
        }

        public override object GetAnswer1()
        {
            var state = _state;
            return MoveStuff(state);
        }

        private static int MoveStuff(State start)
        {
            var toVisit = new List<State>();
            var visited = new HashSet<State>();
            var moveMetrics = new Dictionary<State, int>
            {
                [start] = 0
            };
            toVisit.Add(start);
            var previous = new Dictionary<State, State>();
            while (toVisit.Count>0)
            {
                var likeliest = toVisit[0]; /*toVisit.PickMin(p => moveMetrics[p].heuristic)*/;
                var cost = moveMetrics[likeliest];
                toVisit.RemoveAt(0);
                foreach (var neighbour in likeliest.PossibleMoves().ToList())
                {
                    var steps = cost + 1;
                    if (visited.Contains(neighbour) || (moveMetrics.ContainsKey(neighbour) && moveMetrics[neighbour] <= steps)) continue;

                    moveMetrics[neighbour] = steps;
                    previous[neighbour] = likeliest;
                    if (neighbour.IsEnd())
                    {
                        var step = neighbour;
                        var path = new Stack<State>();
                        // output path
                        while (step!=start)
                        {
                            path.Push(step);
                            step = previous[step];
                        }
                        path.Push(step);
                        Console.WriteLine("Path");
                        foreach (var state in path)
                        {
                            Console.WriteLine(state.ToString());
                        }
                        return steps;
                    }
                    toVisit.Add(neighbour);
                }
                visited.Add(likeliest);
            }
            return -1;
        }

        public override object GetAnswer2()
        {
            var floors = new ImmutableSortedSet<string>[4];
            floors[0] = _state.Building[0].Union( new [] { "elerium", "elerium-compatible", "dilithium", "dilithium-compatible"});
            floors[1] = _state.Building[1];
            floors[2] = _state.Building[2];
            floors[3] = _state.Building[3];

            var start = new State(0, floors);
            return MoveStuff(start);
        }
    }
}
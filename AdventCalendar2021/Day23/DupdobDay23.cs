using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace AdventCalendar2021
{
    public class DupdobDay23: AdvancedDay
    {
        private class AmphiPod
        {
            public char Kind;
            public int X;
            public int Y;

            private int _moveDone;
            private int _energy;

            public bool CanMove => _moveDone < 2;
            public int TotalEnergy => _energy;
            
            public AmphiPod Move((int x, int y) target)
            {
                var result = new AmphiPod
                {
                    X = target.x,
                    Y = target.y,
                };
                result.Kind = Kind;
                result._energy = _energy+ Math.Abs(result.X - X) + Math.Abs(result.Y - Y)*Energy;
                result._moveDone = _moveDone+1;
                return result;
            }
            
            private int Energy => Kind switch { 'A' => 1, 'B' => 10, 'C' => 100, 'D' => 1000 };
        }

        private readonly List<AmphiPod> _amphiPods = new(8);

        public DupdobDay23() : base(23)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] < 'A' || line[i] > 'D') continue;
                var amphi = new AmphiPod
                {
                    Kind = line[i],
                    X = i,
                    Y = index
                };
                _amphiPods.Add(amphi);
            }
        }

        protected override void CleanUp()
        {
            _amphiPods.Clear();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########", 12521);
        }

        public override object GiveAnswer1()
        {
            var state = _amphiPods;
            return Play(state, int.MaxValue);
        }

        private int Play(IReadOnlyCollection<AmphiPod> state, int minimalEnergy)
        {
            foreach (var amphiPod in state)
            {
                var newState = state.Where(p => p.X != amphiPod.X || p.Y != amphiPod.Y).ToList();
                var moves = PossibleMoves(amphiPod, newState);
                foreach (var move in moves)
                {
                    var movedPod = amphiPod.Move(move);
                    var list = newState.Prepend(movedPod).ToList();
                    var energy = list.Sum(p => p.TotalEnergy);  
                    if (energy>minimalEnergy)
                        continue;
                    if (IsAWin(list))
                    {
                        PrintGame(list);
                        minimalEnergy = Math.Min(energy, minimalEnergy);
                    }
                    else
                    {
                        minimalEnergy = Play(list, minimalEnergy);
                    }
                }
            }

            return minimalEnergy;
        }

        private void PrintGame(IReadOnlyCollection<AmphiPod> pods)
        {
            Console.Clear();
            foreach (var amphiPod in pods)
            {
                Console.SetCursorPosition(amphiPod.X, amphiPod.Y);
                Console.Write(amphiPod.Kind);
            }
        }
        
        private bool IsAWin(IEnumerable<AmphiPod> allPods)
        {
            if (allPods.Any(p => p.Y == 1))
                return false;
            for (var i = 3; i < 10; i+=2)
            {
                if (allPods.First(p => p.X == i && p.Y == 2).Kind != allPods.First(p => p.X == i && p.Y == 3).Kind)
                {
                    return false;
                }
            }

            return true;
        }
        
        private IList<(int x, int y)> PossibleMoves(AmphiPod pod, IReadOnlyCollection<AmphiPod> allPods)
        {
            var result = new List<(int x, int y)>();
            if (!pod.CanMove)
            {
                return result;
            }

            var minX = 1;
            var maxX = 12;
            foreach (var amphiPod in allPods)
            {
                if (amphiPod.Y != 1)
                {
                    continue;
                }
                if (amphiPod.X < pod.X && minX<amphiPod.X+1)
                {
                    minX = amphiPod.X+1;
                }
                else if (amphiPod.X > pod.X && maxX > amphiPod.X-1)
                {
                    maxX = amphiPod.X-1;
                }
            }
            
            if (pod.Y >= 2)
            {
                // amphipod is in a room
                if (pod.Y == 3)
                {
                    if (allPods.Any(p => p.X == pod.X && p.Y == 2))
                    {
                        // there is a pod above us ==> can't move
                        return result;
                    }
                }
                // find all possible positions in the hallway
                for (var i = minX; i < maxX; i++)
                {
                    // we do not move on top of rooms 
                    if ( i is 3 or 5 or 7 or 9)
                    {
                        continue;
                    }
                    
                    result.Add((i, 1));
                }
                // we never move the amphiPod within the room, as there is no benefit
                return result;
            }
            // we are in a hallway ==> we can only enter a room if it is empty or same kind
            for (var x = 3; x < 10; x += 2)
            {
                if (x<minX)
                    continue;
                if (x>maxX)
                    break;
                var matchingPods = allPods.Where(p => p.X == x).ToList();
                switch (matchingPods.Count)
                {
                    case 0:
                        result.Add((x, 3));
                        break;
                    case 1 when matchingPods[0].Kind == pod.X:
                        result.Add((x,2));
                        break;
                }
            }
            return result;
        }
    }
}
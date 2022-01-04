using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay23: AdvancedDay
    {
        private class AmphiPod
        {
            public readonly char Kind;

            public int EnergyFactor { get; }
            
            public int TargetRoom { get; }
            
            public AmphiPod(char kind)
            {
                Kind = kind;
                EnergyFactor = kind switch { 'A' => 1, 'B' => 10, 'C' => 100, 'D' => 1000, _=>0 };
                TargetRoom = kind switch
                {
                    'A' => 3,
                    'B' => 5,
                    'C' => 7,
                    'D' => 9,
                    _ => 9
                };
            }
        }

        private readonly Dictionary<(int x, int y), AmphiPod> _startPosition =
            new();
        private int _roomSize = 2;

        public DupdobDay23() : base(23)
        {
            _skipAnswer1 = true;
        }

        protected override void ParseLine(int index, string line)
        {
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] < 'A' || line[i] > 'D') continue;
                var amphi = new AmphiPod (line[i]);
                _startPosition[(i, index)] = amphi;
            }
        }

        protected override void CleanUp()
        {
            _startPosition.Clear();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########", 12521);
        }

        protected override IEnumerable<(string intput, object result)> GetTestData2()
        {
            yield return (@"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########", 44169);
        }

        public override object GiveAnswer2()
        {
            var newPositions = new Dictionary<(int x, int y), AmphiPod>(16);
            // adjust data
            foreach (var (position, pod) in _startPosition)
            {
                if (position.y == 3)
                {
                    // we need to shift the position
                    newPositions[(position.x, 5)] = pod;
                }
                else
                {
                    newPositions.Add(position, pod);
                }
            }
            newPositions.Add((3,3), new AmphiPod('D'));
            newPositions.Add((3,4), new AmphiPod('D'));
            newPositions.Add((5,3), new AmphiPod('C'));
            newPositions.Add((5,4), new AmphiPod('B'));
            newPositions.Add((7,3), new AmphiPod('B'));
            newPositions.Add((7,4), new AmphiPod('A'));
            newPositions.Add((9,3), new AmphiPod('A'));
            newPositions.Add((9,4), new AmphiPod('C'));
            _roomSize = 4;
            var solution = new Stack<(int xStart, int yStart, int xEnd, int yEnd)>();
            return Play(newPositions, solution);
        }

        public override object GiveAnswer1()
        {
            var solution = new Stack<(int xStart, int yStart, int xEnd, int yEnd)>();
            return Play(_startPosition, solution);
        }

        private int Play(Dictionary<(int x, int y), AmphiPod> state,
            Stack<(int xStart, int yStart, int xEnd, int yEnd)> solution, 
            int minimalEnergy = int.MaxValue,
            int currentEnergy = 0)
        {
            foreach (var position in state.Keys.ToList())
            {
                var amphiPod = state[position];
                var moves = PossibleMoves(position, amphiPod, state);
                foreach (var move in moves)
                {
                    var dist = Math.Abs(position.x - move.x) + Math.Abs(position.y - move.y);
                    var nextEnergy = currentEnergy + amphiPod.EnergyFactor * dist;
                    state.Remove(position);
                    state[move] = amphiPod;
                    if (nextEnergy < minimalEnergy)
                    {
                        solution.Push((position.x, position.y, move.x, move.y));
                        if (IsAWin(state))
                        {
                            minimalEnergy = nextEnergy;
                            PrintSolution(state, solution);
                            Console.WriteLine($"New min {minimalEnergy}.");
                        }
                        else
                        {
                            minimalEnergy = Play(state, solution, minimalEnergy, nextEnergy);
                        }

                        solution.Pop();
                    }
                    state.Remove(move);
                    state[position]= amphiPod;
                }
            }

            return minimalEnergy;
        }

        private static readonly int[] Positions = {
            1, 2, 11, 10, 4, 6, 8
        };

        private void PrintSolution(Dictionary<(int x, int y), AmphiPod> pods,
            Stack<(int startX, int startY, int endX, int endY)> moves)
        {
            var copy = new Dictionary<(int x, int y), AmphiPod>(pods);
            
            // rebuild initial state
            foreach (var move in moves)
            {
                if (copy.ContainsKey((move.startX, move.startY)))
                {
                    throw new Exception();
                }
                copy[(move.startX, move.startY)] = copy[(move.endX, move.endY)];
                copy.Remove((move.endX, move.endY));
                PrintGame(copy);
            }
            
            Console.WriteLine("Solution");
            PrintGame(copy);
            foreach (var move in moves.Reverse())
            {
                copy[(move.endX, move.endY)] = copy[(move.startX, move.startY)];
                copy.Remove((move.startX, move.startY));
                PrintGame(copy);
            }
            Console.WriteLine("End solution");
        }
        
        private void PrintGame(Dictionary<(int x, int y), AmphiPod> pods)
        {
            var map = new List<char[]>()
            {
                "#############".ToArray(),
                "#...........#".ToArray(),
                "###.#.#.#.###".ToArray()
            };
            for (var i = 1; i < _roomSize; i++)
            {   
                map.Add("  #.#.#.#.#".ToArray());                
            }
            map.Add("  #########".ToArray());                
            
            foreach (var ((x, y), value) in pods)
            {
                map[y][x] = value.Kind;
            }

            foreach (var line in map)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine();
        }
        
        private bool IsAWin(Dictionary<(int x, int y), AmphiPod> allPods)
        {
            if (allPods.Count!=_roomSize*4)
            {
                throw new Exception("error");
            }
            foreach (var (pos, pod) in allPods)
            {
                if (pos.y == 1)
                {
                    return false;
                }

                if (pos.x != pod.TargetRoom)
                {
                    return false;
                }
            }
            return true;
        }
        
        private IEnumerable<(int x, int y)> PossibleMoves((int x, int y) pos, AmphiPod pod,
            Dictionary<(int X, int Y), AmphiPod> allPods)
        {
            var minX = 1;
            var maxX = 11;
            
            foreach (var (x, y) in allPods.Keys)
            {
                if (x == pos.x && y == pos.y)
                {
                    continue;
                }
                if (y != 1)
                {
                    continue;
                }
                if (x < pos.x && minX<x+1)
                {
                    minX = x+1;
                }
                else if (x > pos.x && maxX > x-1)
                {
                    maxX = x-1;
                }
            }
            
            if ( pos.y!= 1)
            {
                // if there is any pod above us ==> can't move
                for (var y = pos.y-1; y > 1; y--)
                {
                    if (allPods.ContainsKey((pos.x, y)))
                        yield break;
                }

                var isOnTarget = true;
                for (var y = pos.y; y < 2+_roomSize; y++)
                {
                    if (allPods[(pos.x, y)].TargetRoom == pos.x) continue;
                    isOnTarget = false;
                    break;
                }

                if (isOnTarget)
                {
                    yield break;
                }
                    
                // find all possible positions in the hallway
                foreach (var position in Positions)
                {
                    if (position >= minX && position <= maxX)
                        yield return (position, 1);
                }

                // we never move the amphiPod within the room, as there is no benefit
                yield break;
            }
            // we are in a hallway ==> we can only enter our target room room if it is empty or same kind
            if (minX <= pod.TargetRoom && maxX >= pod.TargetRoom)
            {
                var y = _roomSize + 1;
                while (allPods.ContainsKey((pod.TargetRoom, y)))
                {
                    if (allPods[(pod.TargetRoom, y)].TargetRoom != pod.TargetRoom)
                    {
                        yield break;
                    }
                    y--;
                }

                yield return (pod.TargetRoom, y);
            }
        }
    }
}
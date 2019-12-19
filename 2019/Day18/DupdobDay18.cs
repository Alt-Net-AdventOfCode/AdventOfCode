using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2019.Day18
{
    public class DupdobDay18
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay18();
            runner.ParseInput();
            
            Console.WriteLine("Answer 1:{0}", runner.FindShortestPathForKeys());
        }

        private int FindShortestPathForKeys()
        {
            var totalDistance = 0;
            var heldKeys = new HashSet<char>();
            var startRoom = _map.Values.First( r =>r.Content == '@');

            var currentBestDistance = int.MaxValue;
            return TotalDistance(startRoom, heldKeys, totalDistance, ref currentBestDistance);
        }

        private int TotalDistance(Room startRoom, HashSet<char> heldKeys, int totalDistance, ref int currentBestDistance)
        {
            var neededKeysCount = _map.Values.Count(room => char.IsLower(room.Content));
            Console.Clear();
            Console.WriteLine($"Min distance: {currentBestDistance}");
            Console.Write($"Keys: {string.Join(',', heldKeys)}");
            var distances = new Dictionary<Room, int>();
            var minDistance = int.MaxValue;
            if (totalDistance > currentBestDistance)
            {
                return int.MaxValue;
            }
            if (heldKeys.Count == neededKeysCount)
            {
                if (totalDistance < currentBestDistance)
                {
                    currentBestDistance = totalDistance;
                }
                return totalDistance;
            }
            FinDistanceToRooms(startRoom, heldKeys, distances);
            var accessibleKeys =
                distances.Where(e =>
                        e.Value < int.MaxValue && char.IsLower(e.Key.Content) && !heldKeys.Contains(e.Key.Content))
                    .Select(t => t.Key).OrderBy(t => distances[t]);
            
            foreach (var accessibleKey in accessibleKeys)
            {
                // let's try picking a key
                var keyAttempt = new HashSet<char>(heldKeys) {accessibleKey.Content};
                var nextStep = TotalDistance(accessibleKey, keyAttempt, distances[accessibleKey] + totalDistance, ref currentBestDistance);
                if (nextStep < minDistance)
                {
                    minDistance = nextStep;
                }
            }

            return minDistance;
        }

        private void FinDistanceToRooms(Room startRoom, HashSet<char> holdKeys, Dictionary<Room, int> distances)
        {
            var roomToVisit = _map.Values.ToList();
            foreach (var room in roomToVisit)
            {
                distances[room] = int.MaxValue;
            }

            distances[startRoom] = 0;

            while (roomToVisit.Count > 0)
            {
                var closestDist = int.MaxValue;
                Room closestRoom = null;
                foreach (var room in roomToVisit.Where(room => distances[room] < closestDist))
                {
                    closestRoom = room;
                    closestDist = distances[room];
                }

                if (closestRoom == null)
                {
                    break;
                }

                roomToVisit.Remove(closestRoom);
                if (closestRoom.CanCrossTheRoom(holdKeys))
                {
                    closestDist++;
                    foreach (var neighbour in closestRoom.Neighbours)
                    {
                        if (distances[neighbour] > closestDist)
                        {
                            distances[neighbour] = closestDist;
                        }
                    }
                }
            }
        }

        private void ParseInput(string input = Input)
        {
            var lineIndex = 0;
            var maxX = 0;
            foreach (var line in input.Split('\n'))
            {
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i] == '#')
                    {
                        continue;
                    }

                    var room = new Room(line[i]);
                    _map[(i, lineIndex)] = room;
                }

                maxX = Math.Max(line.Length, maxX);
                lineIndex++;
            }

            // detect neighbours
            for (var y = 0; y < lineIndex; y++)
            {
                for (var x = 0; x < maxX; x++)
                {
                    var room = GetRoom(x, y);    
                    if (room == null)
                    {
                        continue;
                    }
                    for (var dir = 0; dir < _directions.Length; dir++)
                    {
                        var next = GetRoom(x, y, dir);
                        if (next != null)
                        {
                            room.AddNeighbour(next);
                        }
                    }
                }
            }
            
            // scan to prune dead ends
            var scanNeeded = true;
            var toRemove = new List<Room>();
            while (scanNeeded)
            {
                scanNeeded = false;
                foreach (var room in _map.Values.Where(r => r.Content =='.' && r.Neighbours.Count()==1))
                {
                    var roomScan = room;
                    for (;;)
                    {
                        
                        if (room.Neighbours.Count() != 1 || room.IsKey)
                        {
                            break;
                        }

                        var toPrune = roomScan;
                        roomScan = roomScan.Neighbours.First();
                        toPrune.Neighbours.Clear();
                        roomScan.Neighbours.Remove(toPrune);
                        toRemove.Add(toPrune);
                        scanNeeded = true;
                    }
                }
            }

            var map = new Dictionary<(int x, int y), Room>();
            foreach (var entry in _map)
            {
                if (!toRemove.Contains(entry.Value))
                {
                    map.Add(entry.Key, entry.Value);
                }
            }

            _map = map;
        }

        private Room GetRoom(int x, int y, int direction=-1)
        {
            if (direction >= 0)
            {
                x += _directions[direction].dx;
                y += _directions[direction].dy;
            }

            return _map.TryGetValue((x, y), out var room) ? room : null;
        }
        
        private readonly (int dx, int dy)[] _directions = {(0, -1),(1, 0),(0, 1),(-1, 0)};
    
        private class Room
        {
            private readonly IList<Room> _neighbours = new List<Room>();
            private char _content;

            public char Content => _content;

            public IList<Room> Neighbours => _neighbours;

            public bool IsDoor => char.IsUpper(_content);
            
            public bool IsKey => char.IsLower(_content);
            
            public Room(char content)
            {
                this._content = content;
            }

            public void AddNeighbour(Room next)
            {
                _neighbours.Add(next);
            }

            public bool CanCrossTheRoom(HashSet<char> keys)
            {
                return !char.IsUpper(_content) || keys.Contains(char.ToLower(_content));
            }

            public override string ToString()
            {
                if (char.IsUpper(_content))
                {
                    return $"Door {_content}";
                }
                else if (char.IsLower(_content))
                {
                    return $"Key {_content}";
                }

                return "Room";
            }
        }
        
        private IDictionary<(int x, int y), Room> _map = new Dictionary<(int x, int y), Room>();
        private const string Input =
@"#################################################################################
#.#.......#.................#.D...#.....#.........#.......#.#...#.......#.....V.#
#.#.#####.#####.###.#######M###.#.#.###########H#.###.#.#.#.#.#.###.###.#######.#
#.#...#.....N.#.#...#...#...#...#.#.....#.....#.#...#.#.#...#.#...#.#.#.......#.#
#.###.#######.###.###.###.###.###.###.#.#.###.#.###.#.#.#####.###.#.#.#######.#.#
#...#...#.........#.#.....#.....#...#.#.#.#...#.#...#.#...#...#.....#.......#...#
#.#.###L###########Q#.#######.#.###.###.#.###.#.#.#######.#.#.#######.#.###.###.#
#.#...#t..........#.#.#.....#.#.#...#..o#...#...#.......#.#.#.#.....#.#.#...#...#
#.#.#############.#.#.#.###.###.#.###.#.###.###########.#.#.###.###.#.#.#####.###
#.#.#.........#...#...#.#.......#.#...#.#...#.........#.#.#.....#...#.#.U.#...#.#
#.#.#.#######.#.###.###W#########.#.###.#.#########.#.#.#.#######.#######.#.###.#
#j#...#.....#.#.#...#.....#.....#...#.#.#...#.....#.#.#...#.......#.....#.#.#...#
#.#########.#.#T#.###.###.#.#.#.#####.#.#.#.#.###.#.#.#.###.#######.#.#.#.#.###.#
#...#.......#.#x#...#.#.#.#.#.#.#k..#...#.#.....#.#.#.#.#...#.....#.#.#...#...#.#
###.###.#####.#.###.#.#.#.###.#.#.#.#.###.#######.#.#.#.#.###.###.#.#.###.###.#.#
#.#.E.#.#.....#.#...#...#z..#.#.#.#.#...#.#b......#.#...#.....#...#.#.#...#...#.#
#.###.#.#.###.#.###.#######.#.#.###.###.###.#######.###########.#####.#.###.###.#
#...#...#.#v..#...#...#.....#.#.......#.#...#.....#.#.....#...#.#.....#.#...#...#
#.#.###.#.#.#####.###.#.#####.#########.#.###.###.#.#####.#.#.#.#.#.#####J###.###
#.#.#...#.#.#...#.#.....#.#...........#.#.#.#.#.#...#...#.#.#...#.#.......#.....#
#.###.###.#.#.#.#.#######.#.#########.#.#P#.#.#.#####.#.#.#.#####.#############.#
#.#...#.#.#.#.#f#...#.#.#...#...#...#.#.#.#...#...#...#...#.#.#.......#...#.....#
#C#.###.#.###.#.###.#X#.#.#####.#.#.#.#.#.#.###.#.#.#######.#.#######.#.#.#.###.#
#...#...#.....#...#...#...#.....#.#...#.#.#...#.#...#.....#.#...#.....#.#...#...#
#.#####.#########.#####.###.#.#.#.#.###.#.###.#.#####.###.#.###.###.###.#########
#.#...#.....#...#.....#...#.#.#.#.#.#...#...#...#...#.#.......#...#.#...#.......#
#.#.#.###.#.#.#.###.#####.#.#.###.###.#####.#.###.#.#.###########.#.#.###.#####.#
#...#...#.#.#.#...#.....#.#.#.#...#...#.#...#.#...#.#.#.....#...#.#.#...#.....#.#
#######.#.###.###.###.###.###.#.###.###.#.###.#.###.#.#.###.#.#.#.#.###.#####.#.#
#.....#.#.#...#.#.#...#...#...#.....#...#.#...#.#...#...#.#...#...#...#...#...#e#
#.#####.#.#.###.#.#.###.###.#.#########.#.#.###.#.#######.###########.#.###.###.#
#.Y.....#.......#.#.....#...#.#.........#.#.#...#.........#.........#.#.......#.#
#.###############.#######.###.###.###.###.#.#.#########.###.#####.#.#.#######.#.#
#.....#.........#.........#.#...#.#.#...#.#.#.........#.....#.#...#.#.....#...#.#
#####.#.#.#####.#####.#####.###.#.#.###.#.###########.#######.#.###.#####.#####.#
#.#...#.#.#...#......s#.#.......#.....#.#.#.........#.#.......#.#.....#...#.....#
#.#.#####.#.#.#########.#.#############.#.#.#####.#.#.#####.###.#####.#.###.###.#
#.#.....#.#.#.......#...#.#...........#.#.#.#...#.#.#...#...#...#...#.#.#...#...#
#.#####.#.#.#.#######.#.#.###.#######.#.#.#.###.#.#####.#.#.#.###.#.###.#.###.###
#.........#.#.........#.......#.................#.........#.#.....#.......#.....#
#######################################.@.#######################################
#.....#.....#.........#.....#.................#.....#.......#.........#..y....#.#
#.###.#.#.#.#.#######.#.###.#.###.#####.#.###.###.###.###.#.###.#.###.#.#####.#.#
#...#.#.#.#...#.....#.#...#.#...#.#...#.#...#.....#...#...#...#.#.#...#.#...#...#
###.#.###.#########.#.###.#.###.#.#.#.#.###.#####.#.###.#####.#.#.#.###.#.#.###.#
#...#.#...#...#.....#.#...#.....#.#.#...#...#...#.#.#...#...#.#.#.#.....#.#..c#.#
#.###.#.###.#.#.###.#.###########.###.###.###.#.###.#.###.#.#.###.#######.###.#.#
#.#.#...#...#...#...#.......#...#...#.#.#.#...#.....#.#.#.#.#...#...#...#.#.#...#
#.#.#####.#######.#########.#.#.###.#.#.#.#.#########.#.#.#.###.###.#.#.#.#.#####
#.#.......#.....#.......#...#.#.#...#...#p..#.........#.#.#...#.....#.#.#...#...#
#.###.###.#.###.#######.#.###.#.#.#####.#####.#########.#.###.#######.#.###.###.#
#...#...#.....#.#.......#.....#.#.#.....#.....#.........#...#.........#...#...#.#
#.#.#####.#####.#.#############.#.#####.#.#####.#######.###.#####.#.#####.###.#.#
#.#.....#.#.....#.....#.......#.#.....#.#...#...#.....#...#...#.#.#.#...#...#.#.#
#######.#.#.###########.#####.#.#####.#.#.#.###.#.###.#.#####.#.#.###.#.#####.#.#
#.......#.#..u..#.......#.....#...#...#.#.#...#.#.#.#.#.........#.....#.......#.#
#.###########.#.#.#######.#.#####.#.###.#####.#.#.#.#.#########.###############.#
#.G.#...#...#.#...#.......#.#.....#.#...#.....#.#.#.#.........#.....#...#.......#
#.#.#.#.#.#I#.#####.#########.#####.#####.#######.#.#########.#####.#.#.###.#.#.#
#.#...#...#.#.#...#.........#...#.......#.........#.......#...#...#...#...#.#.#.#
#.#########.#.###.#####.###.###.#.#####.###############.###.#####.#######.###.#.#
#.#...#.....#.....#...#.#...#...#...#...#.....#.......#.#...#...#.......#...F.#g#
#.###.#.#########.#.###.#.###.#####B#.#.###.#.###.###.#.#.#####.#####.#########.#
#.....#.#...#...#.#.#...#...#.#.....#.#.#...#...#.#.#.#.#.#.....#.....#....r#...#
#.#####.#.#R#.#.###.#####.#.#.#.#####.#.#.#####.#.#.#.#.#.#.#####.#####.#.#.#.###
#.#...#...#...#...#.....#.#.#.#.....#.#.#.....#...#.....#.#.#.....#.....#.#.#...#
###.#.###########.#####.#.#.#.#######.#.#####.#.#######.#.#.###.###.#####.#####.#
#...#.#...#...#.......#.#.#.#.......#.#.#.....#n#.....#l#...#...#...#...#...#...#
#.###.#A#.#.#.#.#####.#.###.#######.#.#.#.#######.###.#######.#.#.#####.###.#.###
#...#...#...#.#.#.#...#...#.....#...#.#.#...#...#.#.#.........#.#.#...#...#.#.#.#
###.#########.#.#.#.#####.#####.#.###.#.###.#Z#.#.#.#####.#.#####.#.#.#.###.#.#.#
#...#.....#.#...#.....#...#...#.#...#.#.#...#.#.#.....#...#.#...#...#.#.#...#..i#
#.###.###.#.#####.#####.###.#.#.###.#.#.#.###.#.#.#####.###.#.#.#####K#.#.#####.#
#.....#.#.#.......#.....#...#...#...#.#.#q#...#...#...#...#.#.#.....#...#.......#
#.#####.#.#####.###.#####.#.###.#.###.###.#.#######.#.###.#.#.#.#######.#########
#.#...#.#.....#...#...#...#.#...#.#.#...#...#...#.#d#.....#.#.#..a......#.....#.#
#.#.#.#.#####.#######.#.###.###.#.#.#.#.#.###.#.#.#.#########.###########.#.#.#.#
#...#.#..w..#.#.....#.#.#.#...#.#.#...#.#.#...#..m#.#.....#...#.....#...#.#.#...#
#####.#####.#.#.###.#.#.#.###.###.#####S#.###.###.#.#.###.#.###.###.#.#.###.###.#
#...........#.....#...#.....#......h....#.....#...#.O...#.......#.....#.....#...#
#################################################################################";
    }
}
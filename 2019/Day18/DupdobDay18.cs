using System;
using System.Collections;
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
            
            Console.WriteLine("Answer 1:{0}.                ", runner.FindShortestPathForKeys());

            runner.PatchMap();
            Console.WriteLine("Answer 2:{0}.                ", runner.FindShortestPathWithDroids());
        }

        private int FindShortestPathWithDroids()
        {
            var heldKeys = new HashSet<char>();
            var droids = (Room[])_droids.Clone();

            var currentBestDistance = int.MaxValue;
            TotalDistance(droids, heldKeys, 0, ref currentBestDistance);
            return currentBestDistance;
        }

        private void PatchMap()
        {
            // turn start point into a wall
            _start.TurnIntoWall();
            // and its neighbours
            for (var i = 0; i < _directions.Length; i++)
            {
                GetRoom(_start.X, _start.Y, i).TurnIntoWall();
            }
            // create 4 new start points;
            (int dx, int dy)[] droidsOffset = {(-1, -1), (1, -1), (1, 1), (-1, 1)};
            var allKeys = new string(_map.Values.Where(r => r.IsKey).Select(r => r.Content).ToArray());
            _keyToStartMap.Clear();
            for (var i = 0; i < droidsOffset.Length; i++)
            {
                _droids[i] = GetRoom(_start.X + droidsOffset[i].dx, _start.Y + droidsOffset[i].dy);
                _droids[i].TurnToStart();
                
                foreach (var room in _map.Values.Where(r => r.IsKey))
                {
                    if (FindDistanceToRoom(_droids[i], room, allKeys) < int.MaxValue)
                    {
                        _keyToStartMap.Add(room, i);
                    }
                }
            }
            PrintMap();
        }

        private int FindShortestPathForKeys()
        {
            var heldKeys = new HashSet<char>();
            var currentBestDistance = int.MaxValue;
            TotalDistance(new []{_start}, heldKeys, 0, ref currentBestDistance);
            return currentBestDistance;
        }

        private void TotalDistance(Room[] startRooms, ICollection<char> heldKeys, int totalDistance,
            ref int currentBestDistance)
        {
            if (heldKeys.Count == _neededKeysCount)
            {
                if (totalDistance >= currentBestDistance)
                {
                    return;
                }
                Console.WriteLine($"Min distance: {totalDistance}.          ");
                Console.Write($"Keys: {string.Join(',', heldKeys)}");
                Console.CursorTop--;
                Console.CursorLeft = 0;
                currentBestDistance = totalDistance;

                return;
            }

            var keyRing = new string(heldKeys.OrderBy(x => x).ToArray());
            var missingKeys = _map.Values.Where(r => r.IsKey && !heldKeys.Contains(r.Content));
            foreach (var accessibleKey in missingKeys)
            {
                var i = _keyToStartMap[accessibleKey];
                // find the droid that can access it
                // let's try picking a key
                var nexStep = GetDistance(startRooms[i], accessibleKey, keyRing);
                if (nexStep == int.MaxValue)
                {
                    nexStep = FindDistanceToRoom(startRooms[i], accessibleKey, keyRing);
                    if (nexStep == int.MaxValue)
                    {
                        continue;
                    }
                }

                var newPos = (Room[])startRooms.Clone();
                var keyAttempt = new HashSet<char>(heldKeys) {accessibleKey.Content};
                newPos[i] = accessibleKey;
                var index = (_start, startRooms[i], new string(keyAttempt.OrderBy(x => x).ToArray()));
                var nextDist = totalDistance + nexStep;
                if (!_cache.ContainsKey(index) || _cache[index] > nextDist)
                {
                    // we find a shorter path to this state
                    _cache[index] = nextDist;
                    TotalDistance(newPos, keyAttempt, totalDistance + nexStep, ref currentBestDistance);
                }
            }
        }

        private int FindDistanceToRoom(Room startRoom, Room target, string keyRing)
        {
            var distances = new  Dictionary<Room, int>();
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

                if (closestRoom.IsKey)
                {
                    SetDistance(startRoom, closestRoom, keyRing, closestDist);
                }

                if (closestRoom == target)
                {
                    return closestDist;
                }
                roomToVisit.Remove(closestRoom);
                if (closestRoom.CanCrossTheRoom(keyRing))
                {
                    foreach (var neighbour in closestRoom.Neighbours)
                    {
                        var distance = closestDist + closestRoom.GetDistance(neighbour);
                        if (distances[neighbour] > distance)
                        {
                            distances[neighbour] = distance;
                        }
                    }
                }
            }
            SetDistance(startRoom, target, keyRing, int.MaxValue);
            return int.MaxValue;
        }

        private int GetDistance(Room from, Room to, string keys)
        {
            return _cache.TryGetValue((from, to, keys), out var dist) ? dist : int.MaxValue;
        }

        private void SetDistance(Room from, Room to, string keys, int distance)
        {
            _cache[(from, to, keys)] = distance;
            _cache[(to, from, keys)] = distance;
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

                    var room = new Room(line[i], i, lineIndex);
                    _map[(i, lineIndex)] = room;
                }

                _maxX = maxX = Math.Max(line.Length, maxX);
                lineIndex++;
            }

            _maxY = lineIndex;
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
            
            _map = Simplify();
            _neededKeysCount = _map.Values.Count(room => char.IsLower(room.Content));
            _start = _map.Values.First(r => r.IsStart);
            foreach (var room in _map.Values.Where(r => r.IsKey))
            {
                _keyToStartMap[room] = 0;
            }
            PrintMap();
        }

        private IDictionary<(int x, int y), Room> Simplify()
        {
                var roomToRemove = new List<Room>();

                // step two = remove dead ends
                var deadEnd = _map.Values
                    .FirstOrDefault(r => r.NeighboursCount == 1 && (r.IsEmpty || r.IsGate));
                while (deadEnd != null)
                {
                    roomToRemove.Add(deadEnd);
                    deadEnd.Prune();
                    
                    deadEnd = _map.Values
                        .FirstOrDefault(r => r.NeighboursCount == 1 && (r.IsEmpty || r.IsGate));
                }
                
                // Step one = simplify intermediate steps
                foreach (var room in _map.Values.Where(r => r.NeighboursCount == 2 && r.IsEmpty))
                {
                    // looking for an end of this branch
                    var next = room.Neighbours.ToList();
                    var leftBranch = next[1];
                    var rightBranch = next[0];
                    var previousLeft = room;
                    var distance = 2;
                    while (leftBranch.IsEmpty && leftBranch.NeighboursCount == 2)
                    { 
                        next = leftBranch.Neighbours.ToList();
                        if (next[0] == previousLeft)
                        {
                            previousLeft = leftBranch;
                            leftBranch = next[1];
                        }
                        else
                        {
                            previousLeft = leftBranch;
                            leftBranch = next[0];
                        }
                        roomToRemove.Add(previousLeft);
                        distance++;
                    }

                    var previousRight = room;
                    while (rightBranch.IsEmpty && rightBranch.NeighboursCount == 2)
                    { 
                        next = rightBranch.Neighbours.ToList();
                        if (next[0] == previousRight)
                        {
                            previousRight = rightBranch;
                            rightBranch = next[1];
                        }
                        else
                        {
                            previousRight = rightBranch;
                            rightBranch = next[0];
                        }
                        roomToRemove.Add(previousRight);
                        distance++;
                    }
                    roomToRemove.Add(previousLeft);
                    roomToRemove.Add(previousRight);
                    previousRight.Prune();
                    previousLeft.Prune();
                    leftBranch.AddNeighbour(rightBranch, distance);
                    rightBranch.AddNeighbour(leftBranch, distance);
                }
            
                var map = _map.Where(entry => !roomToRemove.Contains(entry.Value))
                    .ToDictionary(entry => entry.Key, entry => entry.Value);
                Console.WriteLine("Removed {0} useless rooms.", _map.Count - map.Count);
                return map;
        }

        private void PrintMap()
        {
            for (var y = 0; y < _maxY; y++)
            {
                for (var x = 0; x < _maxX; x++)
                {
                    Console.Write(_map.TryGetValue((x, y), out var room) ? room.Content : ' ');
                }
                Console.WriteLine();
            }    
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
        
        private class Room
        {
            private readonly IDictionary<Room, int> _neighbours = new Dictionary<Room, int>();

            public int X { get; }
            public int Y { get; }
            public char Content { get; set; }

            public ICollection<Room> Neighbours => _neighbours.Keys;

            public int NeighboursCount => _neighbours.Count;
            
            public bool IsKey => char.IsLower(Content);
            public bool IsGate => char.IsUpper(Content);
            public bool IsStart => Content == '@';
            public bool IsEmpty => Content == '.';

            public Room(char content, int x, int y)
            {
                this.Content = content;
                this.X = x;
                this.Y = y;
            }

            public void TurnIntoWall()
            {
                Prune();
                Content = '#';
            }

            public int GetDistance(Room next)
            {
                return _neighbours[next];
            }
            
            public void AddNeighbour(Room next, int distance = 1)
            {
                _neighbours.Add(next, distance);
            }

            public bool CanCrossTheRoom(string keys)
            {
                return !IsGate || keys.Contains(char.ToLower(Content));
            }

            public override string ToString()
            {
                if (Content == '@')
                {
                    return "Start";
                }
                if (char.IsUpper(Content))
                {
                    return $"Door {Content}";
                }

                return char.IsLower(Content) ? $"Key {Content}" : "Room";
            }

            public void Prune()
            {
                foreach (var neighbour in _neighbours)
                {
                    neighbour.Key.PruneNeighbour(this);
                }
                _neighbours.Clear();
            }

            private void PruneNeighbour(Room toPrune)
            {
                _neighbours.Remove(toPrune);
            }

            public void TurnToStart()
            {
                Content = '@';
            }
        }
        
        private readonly (int dx, int dy)[] _directions = {(0, -1),(1, 0),(0, 1),(-1, 0)};
        private readonly Dictionary<(Room from, Room to, string keys), int> _cache = new Dictionary<(Room from, Room to, string keys), int>();
    
        private IDictionary<(int x, int y), Room> _map = new Dictionary<(int x, int y), Room>();
        private int _maxX;
        private int _maxY;
        private int _neededKeysCount;
        private Room _start;
        private readonly Room[] _droids = new Room[4];
        private readonly IDictionary<Room, int> _keyToStartMap = new Dictionary<Room, int>();

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
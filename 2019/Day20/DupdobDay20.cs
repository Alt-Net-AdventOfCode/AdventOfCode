using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2019.Day20
{
    public class DupdobDay20
    {
        public static void GiveAnswers()
        {
            var runner = new DupdobDay20();
            runner.ParseInput();
            
            Console.WriteLine("Answer 1:{0}", runner.FinDistanceToExit());
            Console.WriteLine("Answer 2:{0}", runner.FinDistanceToExitWhenRecursing());
        }

        private int FinDistanceToExit()
        {
            var start = _start;
            var exit = _exit;
            
            var roomToVisit = _map.Values.ToList();
            var distances = new Dictionary<Room, int>();
            foreach (var room in roomToVisit)
            {
                distances[room] = int.MaxValue;
            }

            distances[start] = 0;

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
                foreach (var neighbour in closestRoom.Neighbours)
                {
                    var newDist = closestDist + neighbour.Value;
                    if (distances[neighbour.Key] > newDist)
                    {
                        distances[neighbour.Key] = newDist;
                    }
                }

                if (closestRoom == exit)
                {
                    return distances[closestRoom];
                }
            }

            return -1;
        }
        private int FinDistanceToExitWhenRecursing()
        {
            var currentDepth = 0;
            var start = (_start, currentDepth);
            var exit = (_exit, currentDepth);
            
            var distances = new Dictionary<(Room room, int depth), int>();
            var roomToVisit = distances.Keys.ToList();
            distances[start] = 0;
            roomToVisit.Add(start);
            while (roomToVisit.Count > 0)
            {
                var closestDist = int.MaxValue;
                (Room room, int depth) closestRoom = (null, 0);
                foreach (var room in roomToVisit.Where(room => distances[room] < closestDist))
                {
                    closestRoom = room;
                    closestDist = distances[room];
                }

                roomToVisit.Remove(closestRoom);
                foreach (var neighbour in closestRoom.room.GetNeighbours(closestRoom.depth))
                {
                    var nextRoom = neighbour.cell;
                    var newDist = closestDist + neighbour.distance;
                    if (!distances.ContainsKey(nextRoom))
                    {
                        distances[nextRoom] = newDist;
                        roomToVisit.Add(nextRoom);
                    }
                    else if (distances[nextRoom]>newDist)
                    {
                        distances[nextRoom] = newDist;
                    }
                }

                if (closestRoom == exit)
                {
                    return distances[closestRoom];
                }
            }

            return -1;
        }

        private void ParseInput(string input = Input)
        {
            var lineIndex = 0;
            var maxX = 0;
            foreach (var line in input.Split('\n'))
            {
                for (var i = 0; i < line.Length; i++)
                {
                    // don't care for blocking rooms
                    if (line[i] == '#' || line[i] == ' ')
                    {
                        continue;
                    }

                    var room = new Room(i, lineIndex, line[i].ToString());
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
                    if (room == null || room.Content!=".")
                    {
                        continue;
                    }
                    for (var dir = 0; dir < _directions.Length; dir++)
                    {
                        var next = GetRoom(x, y, dir);
                        if (next == null)
                        {
                            continue;
                        }
                        if (next.Content.Length == 1 && char.IsUpper(next.Content[0]) )
                        {
                            // we're on a gateway
                            var outerRim = x == 2 || y == 2 || y == lineIndex - 3 || x == maxX - 3;
                            room.IsOuter = outerRim;
                            if (dir == 0 || dir == 3)
                            {
                                room.Content = GetRoom(x, y, dir, 2).Content + next.Content ;
                            }
                            else
                            {
                                room.Content = next.Content + GetRoom(x, y, dir, 2).Content;
                            }
                            if (!_gateways.ContainsKey(room.Content))
                            {
                                _gateways[room.Content] = (room, null);
                            }
                            else
                            {
                                _gateways[room.Content] = (_gateways[room.Content].Item1, room);
                            }
                            continue;
                        }
                        room.AddNeighbour(next);
                    }
                }
            }
            
            // link gateways' ends
            foreach (var pair in _gateways.Values)
            {
                if (pair.Item2 == null)
                {
                    continue;
                }
                pair.Item1.AddNeighbour(pair.Item2);
                pair.Item2.AddNeighbour(pair.Item1);
            }

            _exit = _map.Values.FirstOrDefault(r => r.IsExit);
            _start = _map.Values.FirstOrDefault(r => r.IsEntry);
            _map = PruneDeadEnds();
        }

        private Dictionary<(int x, int y), Room> PruneDeadEnds()
        {
            // scan to prune dead ends
            var scanNeeded = true;
            var toRemove = new List<Room>();

            var deadEnd = _map.Values.FirstOrDefault(r => !r.IsGateway && r.Neighbours.Count == 1);
            while (deadEnd != null)
            {
                deadEnd.Prune();
                toRemove.Add(deadEnd);
                deadEnd = _map.Values.FirstOrDefault(r => !r.IsGateway && r.Neighbours.Count == 1);
            }

            Console.WriteLine($"Removed {toRemove.Count} useless rooms.");
            var map = _map.Where(entry => !toRemove.Contains(entry.Value))
                .ToDictionary(entry => entry.Key, entry => entry.Value);
            return map;
        }

        private Room GetRoom(int x, int y, int direction = -1, int times = 1)
        {
            if (direction >= 0)
            {
                for(var k = 0; k<times; k++)
                {
                    x += _directions[direction].dx;
                    y += _directions[direction].dy;
                }            
            }

            return _map.TryGetValue((x, y), out var room) ? room : null;
        }
        
        private class Room
        {
            private readonly int _x;
            private readonly int _y;

            public string Content { get; set; }

            public bool IsGateway => Content.Length > 1;
            public bool IsExit => Content == "ZZ";
            public bool IsEntry => Content == "AA";
            
            public IDictionary<Room, int> Neighbours { get; set; } = new Dictionary<Room, int>();
            public bool IsOuter { get; set; }
            public bool IsInner => IsGateway && !IsOuter;

            public Room(int x, int y, string content)
            {
                Content = content;
                _x = x;
                _y = y;
            }

            public void AddNeighbour(Room next, int distance = 1)
            {
                Neighbours.Add(next, distance);
            }

            public override string ToString()
            {
                return IsGateway ? $"Gateway {Content} {_x}:{_y}" : $"Room {_x}:{_y}";
            }

            public void Prune()
            {
                foreach (var room in Neighbours.Keys)
                {
                    room.RemoveNeighbour(this);
                }
                Neighbours.Clear();
            }

            private void RemoveNeighbour(Room room)
            {
                Neighbours.Remove(room);
            }

            public ICollection<((Room room, int depth) cell, int distance)> GetNeighbours(int depth)
            {
                IList<((Room, int), int)> result = new List<((Room, int), int)>(Neighbours.Count);
                foreach (KeyValuePair<Room,int> keyValuePair in Neighbours)
                {
                    var nextRoom = keyValuePair.Key;
                    if (nextRoom.IsOuter)
                    {
                        if (IsInner)
                        {
                            // access to lower level
                            result.Add(((nextRoom, depth+1), keyValuePair.Value));
                        }
                        else
                        {
                            if (depth == 0 && !nextRoom.IsEntry && !nextRoom.IsExit)
                            {
                                // we skip outer gateways
                                continue;;
                            }

                            if (depth > 0 && (nextRoom.IsEntry || nextRoom.IsExit))
                            {
                                continue;
                            }
                            result.Add(((nextRoom, depth), keyValuePair.Value));
                        }
                    }
                    else if (nextRoom.IsInner)
                    {
                        if (IsOuter)
                        {
                            result.Add(((nextRoom, depth-1), keyValuePair.Value));
                        }
                        else
                        {
                            result.Add(((nextRoom, depth), keyValuePair.Value));
                        }
                    }
                    else
                    {
                        result.Add(((nextRoom, depth), keyValuePair.Value));
                    }
                }
                
                return result;
            }
        }

        private Room _exit;
        private Room _start;
        private readonly (int dx, int dy)[] _directions = {(0, -1),(1, 0),(0, 1),(-1, 0)};
        private IDictionary<(int x, int y), Room> _map = new Dictionary<(int x, int y), Room>();
        private readonly IDictionary<string, (Room, Room)> _gateways = new Dictionary<string, (Room, Room)>();

        private const string Input =
@"                                 T     P           U       U       Z   O                                     
                                 H     Z           A       U       W   K                                     
  ###############################.#####.###########.#######.#######.###.###################################  
  #.#.#.#.#.............#.#...#.#.....#...#.....#.....#.....#...#.....#...#...#...#.#.......#.....#.......#  
  #.#.#.#.#####.#.#####.#.###.#.#.###.###.#.#.#.#.#.#.#.###.###.#.###.#.###.###.###.#####.#####.###.#.#####  
  #...#.........#.#...........#...#...#.#...#.#.#.#.#.#.#.#.....#...#.#.#...........#.#.#.#.#.......#...#.#  
  ###.###.###.#########.###.###.#####.#.#.###.#####.#.#.#.#####.#.#.#.#.#.#######.###.#.#.#.###.#########.#  
  #...#.#.#.......#.#.....#.....#.......#.#.#.#.#...#.#.#.#.#...#.#.#.#.......#.....#...#.#.#.#.....#.....#  
  ###.#.#####.###.#.#########.#.#.###.###.#.#.#.#####.#.#.#.#######.#.#.#.#############.#.#.#.#.###.#####.#  
  #.....#.....#.#.#...........#.#.#...#.#.#.......#...#.......#...#.#.#.#.#...#.#.#.#.#.........#.#...#...#  
  #.###.#####.#.#######.#####.#######.#.#####.#####.###.#.#.#####.###.#.###.###.#.#.#.#.#########.#####.###  
  #.#.#.#...#.#.....#.#.#.#...#...#.......#.#...#.....#.#.#.....#.....#.............................#.....#  
  ###.#.#.#####.#####.###.###.#.###.#.#.###.#.#####.#.###.#.#.#.#####.###.#######.###.###.#.###.#.###.#####  
  #.........#.#...#.#.......#.#...#.#.#.#.....#.....#.#...#.#.#.#...#.#...#.#...#.#.#.#...#...#.#.#.....#.#  
  #####.#####.#.###.#####.#.###.#.###.###.###.#.###.###.#####.#####.#.#.#.#.#.#####.#####.###########.###.#  
  #.#.#...#.#.....#...#...#.....#.#.....#.#...#...#...#.#.#.#.#...#.#.#.#...#.#.......#...........#.#.#.#.#  
  #.#.#.###.#.#####.###.#########.#.#.#######.###.#.#####.#.#.#.#.###.#.#####.#.#.#.#####.#.#.#.###.#.#.#.#  
  #.....#.....#.#.#...#.#...#.....#.#...#.....#...#...#...#.#.#.#.#...#.#.......#.#.#...#.#.#.#.#...#.....#  
  ###.###.#.#.#.#.#.#######.#####.###.#####.###.#.#######.#.#.#.#.#.#.#.#.###.#######.#########.#.#####.###  
  #...#.#.#.#.#.#...#...#.....#...#.....#.#...#.#.#.#.......#.#.#.#.#.#...#.#...#...#.#.....#...#.....#.#.#  
  ###.#.#####.#.###.#.#######.###.###.###.###.#.###.###.#.###.#.#.#.###.###.#.###.#.#.#.#########.#.###.#.#  
  #.....#.#.......#.#...#.....#.#.......#...#.#.....#.#.#.....#.#.#...#.#.........#.....#.#...#.#.#.....#.#  
  ###.#.#.###.#####.#.#####.#.#.#.#.#.###.#.#.#.#####.#.#.###.#.#.###.#.###.#############.#.###.###.#.#.#.#  
  #.#.#.#.#.......#.#.#...#.#...#.#.#.#.#.#.#.#.......#.#.#...#.#...#.#.#...#...#.....#.....#.#...#.#.#.#.#  
  #.#.###.#####.###.#.###.#####.#.#####.#.###.#####.#.#.#######.###.#.###.###.###.#########.#.#.#.#####.#.#  
  #.......#...#...#...#.#.........#...........#.....#.#.......#...#.....#.........#...#.....#.#.#...#.#...#  
  ###.#.###.###.#.###.#.#######.#######.#######.#############.###.###########.#######.###.#.#.###.###.#.###  
  #.#.#...#...#.#.....#...#    Y       R       R             U   X           L    #.....#.#.#.#...#.#...#.#  
  #.#####.#.#######.#####.#    O       F       U             A   Z           Q    #.#.#####.#.###.#.###.#.#  
  #.#.#.#...#...#.....#.#.#                                                       #.#.#.........#...#.....#  
  #.#.#.###.#.#####.#.#.#.#                                                       ###.#.#.#.#.###.#####.###  
RF........#...#.#.#.#......TH                                                     #.#.#.#.#.#.....#.......#  
  #.#######.###.#.#####.###                                                       #.#.#######.###.###.#.###  
BN......#...#.#.#.#.#......ER                                                   BH........#.#.#...#...#.#.#  
  #.#.#####.#.#.#.#.#.###.#                                                       #####.###.#.#######.###.#  
  #.#.#.#...........#.#...#                                                       #.....#.#.#.#.#...#.....#  
  #.###.#.#####.###.###.###                                                       ###.###.#.#.#.###.#.#####  
  #.......#.#.#.#.......#.#                                                       #...#.#.#.#...#.......#..LP
  #######.#.#.#######.###.#                                                       ###.###.#.#.#####.#.###.#  
ON..#.#.#.#.......#.#.#...#                                                       #.#...............#.....#  
  #.#.#.#.###.###.#.#####.#                                                       #.#######################  
  #.#...#.#.....#.........#                                                       #.......................#  
  #.#.#.#####.###.###.###.#                                                       #.#####.#.#.#.#.#####.###  
  #.#.#.....#...#...#.#....CU                                                     #...#...#.#.#.#.#.#...#..CU
  #.#.#.###.#.###.#######.#                                                       #.#####.#.#######.#.###.#  
  #...#.#.....#.#.#...#...#                                                       #.#...#.#.....#.#...#...#  
  ###.#########.#####.#.#.#                                                       #.#.#######.#.#.###.###.#  
YO..#.#.#.#...#.....#.#.#.#                                                     BT..#.....#.#.#.#...#.....#  
  #.###.#.#.#.#.#.###.#####                                                       #.###.###.#######.#######  
  #.........#...#..........UK                                                     #.#.....#.............#..GR
  #.#.###.#.###.###########                                                       ###.#.#####.#########.#.#  
  #.#.#...#...#.#.....#...#                                                       #...#.......#.#...#.....#  
  ###.#########.#.#####.#.#                                                       #.###.#.#.#.#.#.#####.###  
ZH..#...#.#...#.#.#...#.#..GR                                                   OK....#.#.#.#.#.#.#.#...#.#  
  #.#####.###.###.#.#.#.###                                                       #############.#.#.#####.#  
  #.................#...#.#                                                       #.#...#...#.#............KP
  #######################.#                                                       #.#.###.#.#.#.#######.###  
  #.......................#                                                     RK..#.....#.........#.....#  
  ###.#####.#.#.#######.###                                                       #.#.#######.#######.#.###  
BT..#.#...#.#.#.#.....#.#..BN                                                     #.....#...#.#.#...#.#...#  
  #.#.#.#.#####.#.#.###.#.#                                                       #####.###.###.###.#######  
  #.#...#.#...#.#.#.....#.#                                                       #.#.#.#.............#...#  
  #.#####.#.#####.#######.#                                                       #.#.#.#.#.###.#.#######.#  
  #.......#.#.#.#.........#                                                     ON....#.#.#.#...#.#...#....LW
  #########.#.#.###########                                                       #.#####.#######.###.###.#  
  #...............#........ZW                                                     #.#.........#.........#.#  
  #.#.#.###.###.#.###.#####                                                       #.#####.#############.#.#  
UK..#.#.#.#.#...#.#...#...#                                                       #.........#...#.#.#.....#  
  #.#.###.#####.#.###.#.###                                                       #############.#.#.###.###  
  #.#.#.....#.#.#.#.......#                                                       #.............#...#...#.#  
  ###.#.###.#.###.#.#######                                                       #.###.#.#.###.#.#.#####.#  
  #.#.#.#.#...#.#.....#...#                                                       #...#.#.#...#...#.....#..IG
  #.#####.###.#.#######.###                                                       #.#.###.#####.#.#.###.#.#  
  #.......#.............#.#                                                       #.#.#.#.#.....#.#...#...#  
  ###.#.###.#.#.###.###.#.#                                                       #.###.#.#.#.#########.#.#  
LQ....#...#.#.#...#.#...#..UU                                                   PZ......#.#.#...#.......#.#  
  #.#######.###.#.#####.#.#                                                       #.###.#########.###.#####  
ZZ............#.#.#.#...#.#                                                       #.#.#.......#...#.....#.#  
  #.#.#.#.#.#######.###.#.#                                                       #.#.#.#####.#.#####.###.#  
  #.#.#.#.#.........#.....#                                                       #.#.....#...#.#.........#  
  #.#.###.#.#.###.#.#.#.#.#    L     K           Z     L     E       A       I    #.###.#####.#####.#.#.#.#  
  #.#...#.#.#.#...#.#.#.#.#    W     P           H     P     G       U       G    #.#.....#.#...#...#.#.#.#  
  #.#######.#####.#.#####.#####.#####.###########.#####.#####.#######.#######.###########.#.###.#.###.#.###  
  #.#.........#...#.#.......#.....#.#.....#.......#.......#...#.#...#.#.#.#...#.......#.#.#.#...#...#.#...#  
  #.###.###.###.#####.#######.###.#.#.###.#.###.###.###.###.###.#.#.#.#.#.###.#.###.###.#.#.###.###.###.###  
  #.#...#.....#.#.#.#.#.........#...#.#.#.#.#.#...#...#.#...#.....#.#...#.#.......#.#.........#.#...#.#...#  
  #.###.###.#####.#.###.#.#.#.#########.#.#.#.#####.###.###.###.###.#.###.###.###########.#.###.#####.#.###  
  #...#.#...#.......#.#.#.#.#.#.#.....#...#.......#...#.#.....#.#.....#...#...#.#...#.#...#...#.#.......#.#  
  #####.###.#####.#.#.#.###.#.#.#####.#.#.###.#####.#########.#.###.#.#.#.#.###.#.###.###.#.#.#.###.#.#.#.#  
  #.....#...#.#...#.....#...#.......#...#.#.......#.........#.#...#.#.#.#...#...#.#.....#.#.#.#...#.#.#...#  
  #.#.#.#.#.#.#.#####.#########.###.#.###.#.#######.###.#####.#.#####.#####.###.#.###.#.#.###.#.#######.###  
  #.#.#.#.#...#.#.....#.#.#.#.....#.#...#.#.#.#.#.#...#.#.....#.....#.#...#...........#.#.#.#.#.#...#.#...#  
  #.#######.#.#########.#.#.#.#.#########.#.#.#.#.#####.###.#.#.#########.#.#.###.###.#####.###.###.#.#.###  
  #.#.#.#...#...#.............#...#.#.....#.#.#...#.....#...#.#.....#.#.....#.#.#.#...#.#.#...#.#...#.#...#  
  #.#.#.###.#.#######.#.###.#.#####.#####.#.#.#.#####.#.###.###.###.#.###.#.###.#.#####.#.###.###.###.#####  
  #.#.....#.#.#.#.....#.#...#.....#...#...#.......#.#.#...#...#...#.#.#...#.....#.#...#.#.................#  
  #####.###.###.#####.###.#.###.###.#####.#####.#.#.#####.###.###.###.#######.#######.#.#.###.#.###.###.#.#  
  #.........#.........#...#.#.#.#.#.....#.#.....#...#.#.#.#...#.....#...........#.........#.#.#...#.#...#.#  
  #.#####.#.#.#####.###.###.#.#.#.#.#.###.#.#########.#.#.#.###.###.#####.###.#.#.#####.###.#.#######.###.#  
  #.#.#...#.#.#.#...#...#...#...#...#.....#.......#.......#.#.#.#...#.#...#.#.#...#.#.#.#...........#.#...#  
  ###.#.#######.#############.#####.#####.#.#####.#######.#.#.#.#####.###.#.###.###.#.#.###########.#.#.###  
  #...#...#.#.....#.......#.......#.#...#.#.#...#.#.#...#.#.#.....#...#...#...#...#...#...#...#...#.#.#...#  
  ###.#####.#####.#.#####.#.#####.#.#.#.###.#.#.#.#.###.#.#.#.#.###.###.###.#.#####.#######.###.#.###.###.#  
  #...#.................#.....#.#.#...#.#.#.#.#.#.#.......#.#.#.#.....#.....#...................#.#...#...#  
  #.#.###.###.###.#.#.#####.###.#.#.#.#.#.###.#.#.#.#######.###.###.#.#.#####.###.#########.#.#.###.#.###.#  
  #.#.....#...#...#.#...#.....#...#.#.#...#...#...#.......#...#.....#.#.#.......#.........#.#.#...#.#.#...#  
  #############################.#########.###.#########.#####.###.#######.###.#############################  
                               B         R   A         R     E   E       A   X                               
                               H         U   U         K     R   G       A   Z                               ";
    }
}
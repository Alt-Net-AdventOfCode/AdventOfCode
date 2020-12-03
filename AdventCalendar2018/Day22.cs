using System;
using System.Collections.Generic;

namespace AdventCalendar2018
{
    public static class Day22
    {
        private const int depth = 11817;
        private const int targetX = 9;
        private const int targetY = 751;

        
        private static Dictionary<Coord, int> cache = new Dictionary<Coord, int>();
        private static int minDistToTarget;

        private static void MainDay22()
        {

            var risk = 0;
            for (var xIndex = 0; xIndex <= targetX; xIndex++)
            {
                for (var yIndex = 0; yIndex <= targetY; yIndex++)
                {
                    risk += ErosionLevel(xIndex, yIndex) % 3;
                }
            }
            
            Console.WriteLine($"Risk level : {risk}");

            var visitedRooms = new Dictionary<Coord, int>();
            var done = new HashSet<Coord>();
            var start = new Coord(0, 0, Equipment.Torch);
            minDistToTarget = start.ManhattanDistance(new Coord(targetX, targetY, Equipment.Torch))*8;
            for (visitedRooms.Add(start, 0);;)
            {
                var minDist = int.MaxValue;
                Coord closestRoom = null;
                foreach (var pair in visitedRooms)
                {
                    if (pair.Value < minDist)
                    {
                        minDist = pair.Value;
                        closestRoom = pair.Key;
                    }
                }

                visitedRooms.Remove(closestRoom);
                done.Add(closestRoom);

                if (minDist > minDistToTarget)
                {
                    // we have found the shortest possible path
                    break;
                }
                // examine rooms around
                if (closestRoom.X > 0)
                {
                    DistToNeighbout(closestRoom,  closestRoom.X-1, closestRoom.Y, minDist , visitedRooms, done);
                }
                DistToNeighbout(closestRoom, closestRoom.X+1, closestRoom.Y, minDist, visitedRooms, done);
                if (closestRoom.Y > 0)
                {
                    DistToNeighbout(closestRoom,  closestRoom.X, closestRoom.Y-1, minDist , visitedRooms, done);
                }
                DistToNeighbout(closestRoom, closestRoom.X, closestRoom.Y+1, minDist, visitedRooms, done);
                // stay in place, but change gear
                UpdateDistToRoom(closestRoom.ChangeEquipment(), minDist+7, visitedRooms, done);
            }
            
            Console.WriteLine($"Shortest path: {minDistToTarget}");
        }

        private static void DistToNeighbout(Coord closestRoom, int X, int Y, int minDist,
            IDictionary<Coord, int> visitedRooms, HashSet<Coord> done)
        {
// take a look at the room on the left
            var type = ErosionLevel(X, Y)%3;
            
            if (type == 0)
            {
                if (closestRoom.Equipment!=Equipment.Nothing)
                {
                    UpdateDistToRoom(new Coord(X, Y, closestRoom.Equipment)
                        , minDist + 1, visitedRooms, done);
                }

            }
            else if (type == 1)
            {
                if (closestRoom.Equipment != Equipment.Torch)
                {
                    UpdateDistToRoom(new Coord(X, Y, closestRoom.Equipment)
                        , minDist + 1, visitedRooms, done);
                }
            }
            else
            {
                if (closestRoom.Equipment != Equipment.ClimbingGear)
                {
                    UpdateDistToRoom(new Coord(X, Y, closestRoom.Equipment)
                        , minDist + 1, visitedRooms, done);
                }
            }
        }

        private static void UpdateDistToRoom(Coord room, int dist, IDictionary<Coord, int> visitedRooms,
            HashSet<Coord> done)
        {
            int curDist;
            if (done.Contains(room))
                return;
            if (room.X == targetX && room.Y == targetY && room.Equipment == Equipment.Torch)
            {
                // this is the target
                if (minDistToTarget > dist)
                {
                    minDistToTarget = dist;
                }
            }
            if (visitedRooms.TryGetValue(room, out curDist) && curDist< dist)
            {
                return;
            }

            visitedRooms[room] = dist;
        }
        private static int ErosionLevel(int x, int y)
        {
            var point = new Coord(x, y);

            if (!cache.ContainsKey(point))
            {
                cache[point] = ComputeErosionLevel(x, y);
            }

            return cache[point];
        }

        private static int ComputeErosionLevel(int x, int y)
        {
            if (x == 0)
            {
                return (depth + (y * 48271)) % modulo;
            }

            if (y == 0)
            {
                return (depth + (x * 16807)) % modulo;
            }

            if (x == targetX && y == targetY)
            {
                return depth % modulo;
            }
            
            return (ErosionLevel(x - 1, y) * ErosionLevel(x, y - 1) + depth) % modulo;
        }

        private const int modulo = 20183;
        
        private enum Equipment
        {
            Nothing,
            Torch,
            ClimbingGear
        }
        private class Coord
        {
            private int x;
            private int y;
            private Equipment equipment = Equipment.Nothing;

            public int X => x;

            public int Y => y;

            public Equipment Equipment => equipment;

            public Coord(int x, int y, Equipment equipment = Equipment.Nothing)
            {
                this.x = x;
                this.y = y;
                this.equipment = equipment;
            }

            private bool Equals(Coord other)
            {
                return x == other.x && y == other.y && equipment == other.equipment;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Coord) obj);
            }

            public int ManhattanDistance(Coord other)
            {
                return Math.Abs(x - other.x) + Math.Abs(y - other.y);
            }

            public Coord ChangeEquipment()
            {
                var newEquipment = Equipment.Nothing;
                switch (ErosionLevel(x, y) % 3)
                {
                    case 0:
                        newEquipment = Equipment == Equipment.ClimbingGear ? Equipment.Torch : Equipment.ClimbingGear;
                        break;
                    case 1:
                        newEquipment = Equipment == Equipment.ClimbingGear ? Equipment.Nothing : Equipment.ClimbingGear;
                        break;
                    case 2:
                        newEquipment = Equipment == Equipment.Nothing ? Equipment.Torch : Equipment.Nothing;
                        break;
                } 
                return new Coord(x, y, newEquipment);
            }
            
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = x;
                    hashCode = (hashCode * 397) ^ y;
                    hashCode = (hashCode * 397) ^ (int) equipment;
                    return hashCode;
                }
            }
        }
    }

}
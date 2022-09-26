
using System;
using System.Collections.Generic;
using System.Linq;
using AoC;

namespace AdventCalendar2016
{
    public class DupdobDay13 : SolverWithParser
    {
        private int _targetX;
        private int _targetY;
        private int _seed;
        public override void SetupRun(Automaton automaton)
        {
            automaton.Day = 13;
            automaton.RegisterTestData("10,7,4");
            automaton.RegisterTestResult(11);
        }

        public override object GetAnswer1()
        {
            return AStar();
        }

        private int AStar()
        {
            var queue = new List<(int x, int y)>();
            var closed = new HashSet<(int x, int y)>();
            var costs = new Dictionary<(int x, int y), (int cost, int heuristic)>();
            (int x, int y) position = (1, 1);
            queue.Add(position);
            costs[position] = (0, ManhattanDistance(position, (_targetX, _targetY)));
            while (queue.Count > 0)
            {
                // closest location
                var heuristic = int.MaxValue;
                foreach (var pos in queue)
                {
                    if (costs[pos].heuristic >= heuristic)
                    {
                        continue;
                    }
                    position = pos;
                    heuristic = costs[pos].heuristic;
                }

                var cost = costs[position].cost;
                if (position == (_targetX, _targetY))
                {
                    // done
                    return cost;
                }

                queue.Remove(position);
                // scan neighbours
                foreach (var neighbour in Neighbours(position))
                {
                    if (!closed.Contains(neighbour) && (!queue.Contains(neighbour) || costs[neighbour].cost > cost + 1))
                    {
                        costs[neighbour] = (cost + 1, cost+1+ManhattanDistance(neighbour, (_targetX, _targetY)));
                        queue.Add(neighbour);
                    }
                    closed.Add(neighbour);
                }
            }

            return -1;
        }

        private IEnumerable<(int x, int y)> Neighbours((int x, int y) pos)
        {
            if (pos.x >0  && IsEmpty((pos.x - 1, pos.y)))
            {
                yield return (pos.x - 1, pos.y);
            }
            if (IsEmpty((pos.x + 1, pos.y)))
            {
                yield return (pos.x + 1, pos.y);
            }
            if (pos.y>0 && IsEmpty((pos.x, pos.y-1)))
            {
                yield return (pos.x, pos.y-1);
            }
            if (IsEmpty((pos.x, pos.y+1)))
            {
                yield return (pos.x, pos.y+1);
            }
        }

        private static int ManhattanDistance((int x, int y) src, (int x, int y) dst)
        {
            return Math.Abs(src.x - dst.x) + Math.Abs(src.y - dst.y);
        }
        
        public override object GetAnswer2()
        {
            return Djisktra(50);
        }

        private int Djisktra(int max)
        {
            var queue = new List<(int x, int y)>();
            var closed = new HashSet<(int x, int y)>();
            var costs = new Dictionary<(int x, int y), int>();
            (int x, int y) position = (1, 1);
            queue.Add(position);
            costs[position] = 0;
            while (queue.Count > 0)
            {
                // closest location
                var cost = int.MaxValue;
                foreach (var pos in queue)
                {   
                    if (costs[pos] >= cost)
                    {
                        continue;
                    }
                    position = pos;
                    cost = costs[pos];
                }

                if (cost > max)
                {
                    // done
                    return costs.Values.Count(c => c <= max);
                }

                queue.Remove(position);
                // scan neighbours
                foreach (var neighbour in Neighbours(position))
                {
                    if (!closed.Contains(neighbour) && (!queue.Contains(neighbour) || costs[neighbour] > cost + 1))
                    {
                        costs[neighbour] = cost + 1;
                        queue.Add(neighbour);
                    }
                    closed.Add(neighbour);
                }
            }

            return -1;
        }

        protected override void Parse(string data)
        {
            var parameters = data.Split(',').Select(int.Parse).ToList();
            _seed = parameters[0];
            if (parameters.Count > 1)
            {
                _targetX = parameters[1];
                _targetY = parameters[2];
            }
            else
            {
                _targetX = 31;
                _targetY = 39;
            }
        }

        private bool IsEmpty((int x, int y) pos)
        {
            var (x, y) = pos;
            return CountBitOne(x * x + 3 * x + 2 * x * y + y + y * y + _seed) % 2 == 0;
        }
        
        private static int CountBitOne(long val)
        {
            var count = 0;
            while (val!=0)
            {
                count += (val % 4) switch { 0 => 0, 1=>1, 2=>1, 3=>2};
                val >>= 2;
            }
            return count;
        }
    }
}
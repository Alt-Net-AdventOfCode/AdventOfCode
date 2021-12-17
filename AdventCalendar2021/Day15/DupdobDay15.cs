using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay15 : AdvancedDay
    {
        private readonly List<List<int>> _map = new();
        public DupdobDay15() : base(15)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            _map.Add(line.Select( c=> int.Parse(c.ToString())).ToList());
        }

        public override object GiveAnswer1()
        {
            var localMap = _map;
            var distToStart = new Dictionary<(int x, int y), int>();

            var vertices = new HashSet<(int x, int y)>();
            for (var y = 0; y < localMap.Count; y++)
            {
                for (var x = 0; x < localMap[0].Count; x++)
                {
                    var coords = (x, y);
                    distToStart[coords] = int.MaxValue;
                }
            }

            distToStart[(0, 0)] = 0;
            vertices.Add((0,0));
            while (vertices.Count>0)
            {
                var vertex = vertices.OrderBy(c => distToStart[c]).First();
                vertices.Remove(vertex);
                if (vertex.x == localMap[0].Count - 1 && vertex.y == localMap.Count - 1)
                    break;
                var risk = distToStart[vertex];
                (int x, int y) next; 
                if (vertex.x > 0)
                {
                    next = (vertex.x - 1, vertex.y);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
                if (vertex.y > 0)
                {
                    next = (vertex.x , vertex.y-1);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
                if (vertex.x < localMap[0].Count-1)
                {
                    next = (vertex.x + 1, vertex.y);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
                if (vertex.y < localMap.Count-1)
                {
                    next = (vertex.x, vertex.y+1);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
            }

            return distToStart[(localMap[0].Count - 1, localMap.Count - 1)];
        }

        public override object GiveAnswer2()
        {
            var localMap = new List<List<int>>();
            for(var y = 0; y < _map.Count; y++)
            {
                var line = new List<int>(_map[0].Count*5);
                for (var i = 0; i < 5; i++)
                {
                    foreach (var entry in _map[y])
                    {
                        line.Add((entry+i-1)%9+1);
                    }
                }
                localMap.Add(line);
            }

            for (var i = 1; i < 5; i++)
            {
                for (int y = 0; y < _map.Count; y++)
                {
                    var newLine = new List<int>();
                    foreach (var entry in localMap[y])
                    {
                        newLine.Add((entry+i-1)%9+1);
                    }
                    localMap.Add(newLine);
                }
            }
            
            var distToStart = new Dictionary<(int x, int y), int>();

            var vertices = new HashSet<(int x, int y)>();
            for (var y = 0; y < localMap.Count; y++)
            {
                for (var x = 0; x < localMap[0].Count; x++)
                {
                    var coords = (x, y);
                    distToStart[coords] = int.MaxValue;
                }
            }

            distToStart[(0, 0)] = 0;
            vertices.Add((0,0));
            while (vertices.Count>0)
            {
                var vertex = vertices.OrderBy(c => distToStart[c]).First();
                vertices.Remove(vertex);
                if (vertex.x == localMap[0].Count - 1 && vertex.y == localMap.Count - 1)
                    break;
                var risk = distToStart[vertex];
                (int x, int y) next; 
                if (vertex.x > 0)
                {
                    next = (vertex.x - 1, vertex.y);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
                if (vertex.y > 0)
                {
                    next = (vertex.x , vertex.y-1);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
                if (vertex.x < localMap[0].Count-1)
                {
                    next = (vertex.x + 1, vertex.y);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
                if (vertex.y < localMap.Count-1)
                {
                    next = (vertex.x, vertex.y+1);
                    if (risk + localMap[next.y][next.x] < distToStart[next])
                    {
                        distToStart[next] = risk + localMap[next.y][next.x];
                        vertices.Add(next);
                    }
                }
            }

            return distToStart[(localMap[0].Count - 1, localMap.Count - 1)];
        }

        protected override void CleanUp()
        {
            _map.Clear();
        }

        protected override void SetupTestData()
        {
            ExpectedResult1 = 40;
            ExpectedResult2 = 315;
            TestData = @"1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581";
        }
    }
}
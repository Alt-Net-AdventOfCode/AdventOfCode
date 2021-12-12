using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventCalendar2021
{
    public class DupdobDay12 : AdvancedDay
    {
        private readonly Dictionary<string, HashSet<string>> _data = new();
        private readonly HashSet<string> _smallCaves = new();
        public DupdobDay12() : base(12)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var edge = line.Split('-', StringSplitOptions.TrimEntries);
            AddEdge(edge[0], edge[1]);
            AddEdge(edge[1], edge[0]);
        }

        private void AddEdge(string p0, string p1)
        {
            if (p0.All(char.IsLower))
            {
                _smallCaves.Add(p0);
            }
            if (!_data.ContainsKey(p0))
            {
                _data.Add(p0, new HashSet<string>());
            }

            _data[p0].Add(p1);
        }


        public override object GiveAnswer1()
        {
            return FindPaths("start", new HashSet<string>());
        }

        private int FindPaths(string current, ISet<string> currentPath)
        {
            if (current == "end")
            {
                return 1;
            }
            if (_smallCaves.Contains(current))
            {
                if (currentPath.Contains(current))
                    return 0;
            }

            currentPath.Add(current);
            var count = _data[current].Sum(nextVertex => FindPaths(nextVertex, currentPath));

            currentPath.Remove(current);
            return count;
        }

        private int FindPathsAlt(string current, HashSet<string> currentPath, string extraPassage)
        {
            if (current == "end")
            {
                return 1;
            }

            if (_smallCaves.Contains(current))
            {
                if (currentPath.Contains(current))
                {
                    if (!string.IsNullOrEmpty(extraPassage) || current == "start")
                    {
                        return 0;
                    }

                    extraPassage = current;
                }
            }

            currentPath.Add(current);
            var count = _data[current].Sum(nextVertex => FindPathsAlt(nextVertex, currentPath, extraPassage));
            if (current != extraPassage)
            {
                currentPath.Remove(current);
            }
            return count;
        }

        public override object GiveAnswer2()
        {
            return FindPathsAlt("start", new HashSet<string>(), string.Empty);
        }

        protected override void SetupTestData(int id)
        {
            _expectedResult1 = 19;
            _expectedResult2 = 103;
            _testData = @"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc";
        }

        protected override void SetupRunData()
        {
            _data.Clear();
            _smallCaves.Clear();
        }
    }
}
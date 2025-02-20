using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AOCHelpers;

namespace AdventCalendar2015
{
    public class DupdobDay17 : DupdobDayBase
    {
        protected override string Input => @"43
3
4
10
21
44
4
6
47
41
34
17
17
44
36
31
46
9
27
38";

        protected override void ParseLine(int index, string line)
        {
            _eggNogContainer.Add(int.Parse(line));
        }

        public override object GiveAnswer1()
        {
            const int targetVolume = 150;
            var validCombinations = 0;
            var containers = _eggNogContainer.ToList();
            containers.Sort();
            CountSolutions(containers, targetVolume, ref validCombinations, Array.Empty<int>());
            return validCombinations;
        }

        private void CountSolutions(IEnumerable<int> containers, in int targetVolume, ref int validCombinations, 
            IEnumerable<int> selected)
        {
            var list = containers.ToList();
            while (list.Count>0)
            {
                var container = list[0];
                list.RemoveAt(0);
                var remaining = targetVolume - container;
                if (remaining < 0)
                {
                    return;
                }

                if (remaining == 0)
                {
                    validCombinations++;
                    _combinations.Add(selected.Append(container).ToList());
                    continue;
                }
                
                CountSolutions(list, 
                    remaining, 
                    ref validCombinations, 
                    selected.Append(container));
            }
        }

        public override object GiveAnswer2()
        {
            return _combinations.Count( t => t.Count == _combinations.Min(t => t.Count));
        }

        public override int Day => 17;
        
        private readonly IList<List<int>> _combinations = new Collection<List<int>>();
        private readonly IList<int> _eggNogContainer = new List<int>();
    }
}
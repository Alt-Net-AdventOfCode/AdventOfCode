// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2025 Cyrille DUPUYDAUBY
// ---
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(24)]
public class DupdobDay24: SolverWithParser
{
    private int[] _packages;
    
    protected override void Parse(string data) => _packages = data.SplitLines().Select(int.Parse).ToArray();

    
    [Example(1, @"1
2
3
4
5
7
8
9
10
11", 99)]
    public override object GetAnswer1()
    {
        var lotWeight = _packages.Sum()/3;
        var maxPackCount = _packages.Length / 3;
        var packages = _packages.OrderBy(p => -p).ToList();
        // we use a recursive function to find the subsets
        var bestSets = FindSubSet(lotWeight, packages, 0, 0, maxPackCount);
        return bestSets.Min( set => set.Aggregate(1L, (i, i1) => i * i1));
    }

    private List<List<int>> FindSubSet(int lotWeight, List<int> packages, int index, int nbPack, int maxPackCount)
    {
        // nothing to add and a valid number of packet
        if (lotWeight == 0 && nbPack <= maxPackCount)
        {
            // we found a valid subset
            return [[]];
        }
        // we need to add some packet, but we have no more packets, or we already have too many packets
        if (index == packages.Count || nbPack > maxPackCount || lotWeight<0)
        {
            // impossible
            return [];
        }

        var sets = new List<List<int>>();
        // we look for subsets with each remaining package
        for (var i = index; i < packages.Count; i++)
        {
            var subSetSize = sets.FirstOrDefault()?.Count ?? maxPackCount;
            var subSets = FindSubSet(lotWeight - packages[i], packages, i + 1, nbPack + 1, subSetSize+1);
            foreach (var subSet in subSets.Where(subSet => sets.Count == 0 || sets[0].Count > subSet.Count + 1))
            {
                // we found a smaller subset, so we clear the list
                sets.Clear();
                sets.Add(subSet.Prepend(packages[i]).ToList());
            }
        }
        return sets;
    }

    [ReuseExample(1, 44)]
    public override object GetAnswer2()
    {
        var lotWeight = _packages.Sum()/4;
        var maxPackCount = _packages.Length /4;
        var packages = _packages.OrderBy(p => -p).ToList();
        // we use a recursive function to find the subsets
        var bestSets = FindSubSet(lotWeight, packages, 0, 0, maxPackCount);
        return bestSets.Min( set => set.Aggregate(1L, (i, i1) => i * i1));
    }
}
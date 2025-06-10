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

using System;
using System.Collections.Generic;
using System.Linq;
using AoC;

namespace AdventCalendar2015;

[Day(19)]
public class DupdobDay19: SolverWithParser
{
    private readonly Dictionary<string, List<string>> _transformations = new();
    private string _molecule;

    protected override void Parse(string data)
    {
        foreach (var line in data.SplitLines())
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(" => ", StringSplitOptions.TrimEntries);
            if (parts.Length == 1)
            {
                // This is the molecule
                if (_molecule != null)
                {
                    throw new InvalidOperationException("Molecule defined twice");
                }
                _molecule = parts[0];
                continue;
            }

            if (_transformations.TryGetValue(parts[0], out var list))
            {
                list.Add(parts[1]);
            }
            else
            {
                _transformations[parts[0]] = [parts[1]];
            }
        }
    }
    
    [Example("H => HO\nH => OH\nO => HH\nHOH", 4)]
    public override object GetAnswer1()
    {
        var results = new HashSet<string>();
        for(var i = 0; i<_molecule.Length; i++)
        {
            foreach (var (key, values) in _transformations)
            {
                if (!_molecule[i..].StartsWith(key)) continue;
                foreach (var newMolecule in values.Select(value => _molecule[..i] + value + _molecule[(i + key.Length)..]))
                {
                    results.Add(newMolecule);
                }
            }
        }

        return results.Count;
    }

    [Example("e => H\ne => O\nH => HO\nH => OH\nO => HH\nHOH", 3)]
    [Example("e => H\ne => O\nH => HO\nH => OH\nO => HH\nHOHOHO", 6)]
    public override object GetAnswer2()
    {
       // find how to move from target molecule to e
       var reverseTransformations = new Dictionary<string, List<string>>();
       foreach (var (from, listTo) in _transformations)
       {
           foreach (var to in listTo)
           {
               if (reverseTransformations.TryGetValue(to, out var list))
               {
                   list.Add(from);
               }
               else
               {
                   reverseTransformations[to] = [from];
               }
           }
       }
       
       var candidates = new PriorityQueue<(string, int), int>();
       candidates.Enqueue((_molecule, 0), 0);
       while (candidates.Count>0)
       {
           var (nextMolecule, step) = candidates.Dequeue();
           step += 1;
           for(var i= 0; i<nextMolecule.Length; i++)         
           {
               foreach (var (key, values) in reverseTransformations)
               {
                   if (!nextMolecule[i..].StartsWith(key)) continue;
                   foreach (var newMolecule in values.Select(value => nextMolecule[..i] + value + nextMolecule[(i + key.Length)..]))
                   {
                       if (newMolecule == "e")
                       {
                           return step;
                       }
                       candidates.Enqueue((newMolecule, step), 1000*newMolecule.Length+step);
                   }
               }
           }
       }

       return -1;
    }
}
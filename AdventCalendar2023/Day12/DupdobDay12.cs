// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2023 Cyrille DUPUYDAUBY
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

using System.Text;
using AoC;

namespace AdventCalendar2023;

public class DupdobDay12 : SolverWithLineParser
{
    private List<(List<int> blocks, string pattern)> _patterns = new();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 12;
        automatonBase.RegisterTestDataAndResult(@"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1", 21, 1);

    }

    public override object GetAnswer1()
    {
        var sum = 0L;
        foreach (var p in _patterns)
        {
            var possibleArrangements = PossibleArrangements(p.blocks, p.pattern, 0, 0);
            sum += possibleArrangements;
            _cache.Clear();
        }

        return sum;
    }

    private readonly Dictionary<(int, int), long> _cache = new();
    
    private long PossibleArrangements(IReadOnlyList<int> blocks, string pattern, int fromBlock, int fromPos, 
        IEnumerable<(int pos, int len)>? solution = null, bool print = false)
    {
        if (fromPos == 0 && print)
        { 
            Console.WriteLine(pattern);
        }
        if (_cache.TryGetValue((fromBlock, fromPos), out var value))
        {
            return value;
        }

        var currentBlock = blocks[fromBlock++];
        var neededSpace = blocks.Skip(fromBlock).Sum() + blocks.Count - fromBlock -1;
        var patternCount = 0L;
        var remainingSpace = pattern.Length - neededSpace - currentBlock;
        for (var i = fromPos; i < remainingSpace; i++)
        {
            var cantContinue = pattern[i] == '#';
            if (pattern[i..(i+currentBlock)].Contains('.') || 
                (i+currentBlock<pattern.Length && pattern[i+currentBlock] == '#'))
            {
                // we can't place our block
                if (cantContinue)
                {
                    break;
                }
                continue;
            }

            // we have a possible position
            if (fromBlock == blocks.Count)
            {
                // if it is the last block and there are sure positions afterwards
                if (pattern.Skip(i + currentBlock).Contains('#'))
                {
                    if (cantContinue)
                    {
                        // we can't continue, there must be a block here
                        // there is no possible solution
                        return 0;
                    }
                    // nope, we should have at least an extra block
                    continue;
                }
                
                // we placed all blocks
                patternCount++;
                // print solution if requested
                if (solution != null)
                {
                    var solutionText = pattern.ToArray();
                    var inError = false;
                    foreach ((int begin, int len) current in solution.Append((i, currentBlock)))
                    {
                        for (var k = current.begin; k < current.begin + current.len; k++)
                        {
                            inError |= pattern[k] == '.';
                            solutionText[k] = '_';
                        }
                    }

                    inError |= solutionText.Contains('#');
                    if (inError)
                    {
                        Console.WriteLine($"Invalid answer:{new string(solutionText)}");
                        Console.WriteLine($"pattern is    :{pattern}");
                    }
                    if (print)
                    {
                        Console.WriteLine(solutionText);
                    }
                }
            }
            else
            {
                // try to place remaining blocs
                patternCount += PossibleArrangements(blocks, pattern, fromBlock, i+1+currentBlock, solution?.Append((i, currentBlock)), print);
            }
            
            if (cantContinue)
            {
                // we try further away, there must be a block here
                break;
            }
        }

        _cache[(fromBlock - 1, fromPos)] = patternCount;
        if (fromBlock == 1 && solution != null && print)
        {
            Console.WriteLine($"{patternCount} solutions found.");
        }
        return patternCount;
    }

    public override object GetAnswer2()
    {
        var sum = 0L;
        foreach (var p in _patterns)
        {
            var possibleArrangements = PossibleArrangements(p.blocks.Concat(p.blocks).Concat(p.blocks).Concat(p.blocks).Concat(p.blocks).ToList()
                , p.pattern+'?'+p.pattern+'?'+p.pattern+'?'+p.pattern+'?'+p.pattern, 0, 0);
            Console.WriteLine($"{possibleArrangements} found for {p.pattern} {string.Join(',', p.blocks)}");
            sum += possibleArrangements;
            _cache.Clear();
        }

        return sum;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;
        var parts = line.Split(' ');
        var compressedPattern = new StringBuilder();
        foreach (var letter in parts[0])
        {
            if (letter=='.' && compressedPattern.Length>0 && compressedPattern[^1] == '.')
                // skip repeating dots
                continue;
            compressedPattern.Append(letter);
        }
        _patterns.Add((parts[1].Split(',').Select(int.Parse).ToList(), compressedPattern.ToString()));
    }
}
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

using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2017;

public partial class DupdobDay25: SolverWithParser
{
    public override void SetupRun(DayAutomaton dayAutomatonBase)
    {
        dayAutomatonBase.Day = 25;
        dayAutomatonBase.AddExample("""
                                 Begin in state A.
                                 Perform a diagnostic checksum after 6 steps.

                                 In state A:
                                   If the current value is 0:
                                     - Write the value 1.
                                     - Move one slot to the right.
                                     - Continue with state B.
                                   If the current value is 1:
                                     - Write the value 0.
                                     - Move one slot to the left.
                                     - Continue with state B.

                                 In state B:
                                   If the current value is 0:
                                     - Write the value 1.
                                     - Move one slot to the left.
                                     - Continue with state A.
                                   If the current value is 1:
                                     - Write the value 1.
                                     - Move one slot to the right.
                                     - Continue with state A.
                                 """).Answer1(3);
    }

    public override object GetAnswer1()
    {
        var currentSate = _initState;
        var ones = new HashSet<int>();
        var headPosition = 0;
        for (var i = 0; i < _numberOfSteps; i++)
        {
            var state = _instructions[currentSate];
            var operation = ones.Contains(headPosition) ? state.on1 : state.on0;
            if (operation.Value == 0)
            {
                ones.Remove(headPosition);
            }
            else
            {
                ones.Add(headPosition);
            }

            headPosition += operation.GoLeft ? -1 : 1;
            currentSate = operation.NextState;
        } 

        return ones.Count;
    }

    public override object GetAnswer2()
    {
        return null;
    }

    private readonly Regex _stateDeclaration = MyRegex2();

    private readonly Regex _ruleDeclaration = MyRegex1();

    private readonly Regex _setupDeclaration = MyRegex();

    private record Operation(int Value, bool GoLeft, string NextState);

    private readonly Dictionary<string, (Operation on0, Operation on1)> _instructions = [];

    private string _initState = null!;
    private int _numberOfSteps;

    protected override void Parse(string input)
    {
        var blockIndex = 0;
        foreach (var block in input.SplitLineBlocks())
        {
            ParseBlock(block.ToList(), blockIndex++);
        }
    }

    private void ParseBlock(List<string> block, int blockIndex)
    {
        if (blockIndex == 0)
        {
            var input = string.Join(Environment.NewLine, block);
            var match = _setupDeclaration.Match(input);
            if (!match.Success)
            {
                throw new Exception($"Failed to parse {input}");
            }

            _initState = match.Groups[1].Value;
            _numberOfSteps = int.Parse(match.Groups[2].Value);
        }
        else
        {
            var match = _stateDeclaration.Match(block[0]);
            if (!match.Success)
            {
                throw new Exception($"Failed to parse {block[0]}");
            }

            var state = match.Groups[1].Value;
            var input = string.Join(Environment.NewLine, block[1..]);
            var matches = _ruleDeclaration.Matches(input);
            if (matches.Count != 2)
            {
                throw new Exception($"Failed to parse {input}");
            }
            
            _instructions.Add(state, (OperationFromMatch(matches[0]), OperationFromMatch(matches[1])));
        }
    }

    private Operation OperationFromMatch(Match match)
    {
        var value = int.Parse(match.Groups[2].Value);
        var goLeft = match.Groups[3].Value == "left";
        var nextState = match.Groups[4].Value;
        return new Operation(value, goLeft, nextState);
    }

    [GeneratedRegex("""
                    Begin in state (\w+)\.
                    Perform a diagnostic checksum after (\d+) steps\.
                    """)]
    private static partial Regex MyRegex();
    [GeneratedRegex("""
                      If the current value is (0|1):
                        - Write the value (0|1)\.
                        - Move one slot to the (right|left)\.
                        - Continue with state (\w+)\.
                    """)]
    private static partial Regex MyRegex1();
    [GeneratedRegex("In state (\\w+):")]
    private static partial Regex MyRegex2();
}
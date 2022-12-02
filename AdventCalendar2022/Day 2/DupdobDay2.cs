// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2022 Cyrille DUPUYDAUBY
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
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using AoC;

namespace AdventCalendar2022;

public class DupdobDay2 : SolverWithLineParser
{
    private readonly List<(int opponent, int player)> _strategies = new();
    public override void SetupRun(Automaton automaton)
    {
        automaton.Day = 2;
        automaton.RegisterTestDataAndResult(@"A Y
B X
C Z", 15, 1);
        automaton.RegisterTestDataAndResult(@"A Y
B X
C Z", 12, 2);
    }

    public override object GetAnswer1()
    {

    var score = 0;
        foreach (var (opponent, player) in _strategies)
        {
            score += player;
            // what is the result
            if (opponent == player)
            {
                score += 3;
            }
            else if (player == 1 && opponent == 3)
            {
                // win
                score += 6;
            }
            else if (player== 2 && opponent == 1)
            {
                // win
                score += 6;
            }
            else if (player== 3 && opponent == 2)
            {
                // win
                score += 6;
            }
        }

        return score;
    }

    public override object GetAnswer2()
    {
        var score = 0;
        foreach (var (opponent, player) in _strategies)
        {
            switch (player)
            {
                case 1:
                    // we loose
                    score += opponent == 1 ? 3 : opponent - 1;
                    break;
                case 2:
                    // draw
                    score += 3 + opponent * 2;
                    break;
                case 3:
                    // we win
                    score += 6+ (opponent == 3 ? 1 : opponent + 1);
                    break;
            }
        }

        return score;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }
        _strategies.Add((line[0]-'A'+1, line[2]-'X'+1));
    }
}
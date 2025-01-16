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

using AoC;

namespace AdventCalendar2017;

public class DupdobDay22 : SolverWithDataAsLines
{
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 22;
        automatonBase.AddExample("..#\n#..\n...\n").WithParameters(70, 100)
            .Answer1(41).Answer2(26);
    }

    private readonly (int dy, int dx)[] _vectors = [(-1, 0), (0,1), (1, 0), (0, -1)];
    
    public override object GetAnswer1()
    {
        var infected = _infectedCells.ToHashSet();
        var bursts = GetParameter(0, 10000);
        var infections = 0;
        (int y, int x) virus = (0, 0);
        var direction = 0;
        for (var i = 0; i < bursts; i++)
        {
            if (infected.Add(virus))
            {
                direction = (direction + 3) % _vectors.Length;
                infections++;
            }
            else
            {
                infected.Remove(virus);
                direction = (direction + 1) % _vectors.Length;
            }

            var vector = _vectors[direction];
            virus = (virus.y + vector.dy, virus.x + vector.dx);
        }

        return infections;
    }

    private enum State
    {
        Clean, Weakened, Infected, Flagged
    }
    public override object GetAnswer2()
    {
        var cells = _infectedCells.ToDictionary(p=>p, _ => State.Infected);
        var bursts = GetParameter(1, 10000000);
        var infections = 0;
        (int y, int x) virus = (0, 0);
        var direction = 0;
        for (var i = 0; i < bursts; i++)
        {
            var currentState = cells.GetValueOrDefault(virus, State.Clean);
            var turn = currentState switch
            {
                State.Clean => 3,
                State.Weakened => 0,
                State.Infected => 1,
                State.Flagged => 2,
                _ => throw new ArgumentOutOfRangeException()
            };
            direction = (direction + turn) % _vectors.Length;
            // change state
            currentState = (State)(((int)currentState + 1) % 4);
            if (currentState ==State.Clean)
            {
                cells.Remove(virus);
            }
            else
            {
                cells[virus] = currentState;
            }

            if (currentState == State.Infected)
            {
                infections++;
            }

            var vector = _vectors[direction];
            virus = (virus.y + vector.dy, virus.x + vector.dx);
        }

        return infections;
    }

    private readonly List<(int y, int x)> _infectedCells = [];
    
    protected override void ParseLines(string[] lines)
    {
        var offsetX = (lines[0].Length - 1) / 2;
        var offsetY = (lines.Length - 1) / 2;
        for (var y = 0; y < lines.Length; y++)
        {
            var currentLine = lines[y];
            for (var x = 0; x < currentLine.Length; x++)
            {
                if (currentLine[x] == '#')
                {
                    _infectedCells.Add((y-offsetY, x-offsetX));
                }
            }
        }
        
    }
}
// MIT License
// 
//  AdventOfCode
// 
//  Copyright (c) 2024 Cyrille DUPUYDAUBY
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

using System.Globalization;
using System.Text.RegularExpressions;
using AoC;

namespace AdventCalendar2023;

public partial class DupdobDay18 : SolverWithLineParser
{
    private static readonly (int dy, int dx)[] Vectors = { (0, 1), (1, 0), (0, -1), (-1, 0) };
    private static readonly string Symbols = "RDLU";
    private readonly List<(char symbol, int len, int len2, int dir)> _path= new ();
    private readonly Regex _parser = MyRegex();
    public override void SetupRun(Automaton automatonBase)
    {
        automatonBase.Day = 18;
        automatonBase.RegisterTestDataAndResult(@"R 6 (#70c710)
D 5 (#0dc571)
L 2 (#5713f0)
D 2 (#d2c081)
R 2 (#59c680)
D 2 (#411b91)
L 5 (#8ceee2)
U 2 (#caa173)
L 1 (#1b58a2)
U 2 (#caa171)
R 2 (#7807d2)
U 3 (#a77fa3)
L 2 (#015232)
U 2 (#7a21e3)", 62, 1);
        automatonBase.RegisterTestResult(952408144115L, 2);
    }

    public override object GetAnswer1()
    {
        return ComputeArea(_path.Select(p => (p.len, Symbols.IndexOf(p.symbol))));
    }

    public override object GetAnswer2()
    {
        return ComputeArea(_path.Select(p => (p.len2, p.dir)));
    }

    private static object ComputeArea(IEnumerable<(int len2, int dir)> entries)
    {
        var area = 0L;
        var perimeter = 0L;
        (long y, long x) cursor = (0L, 0L);
        foreach (var (len, symbol)  in entries)
        {
            var vector = Vectors[symbol]; 
            perimeter += len;
            (long y, long x) newPos = (cursor.y + len*vector.dy, cursor.x + len*vector.dx);
            area += (cursor.y * newPos.x - cursor.x * newPos.y);
            cursor = newPos;
        }
        return (Math.Abs(area)+perimeter)/2+1;
    }

    protected override void ParseLine(string line, int index, int lineCount)
    {
        var match = _parser.Match(line);
        if (match.Success == false)
        {
            Console.WriteLine("Failed to parse {0}", line);
            return;
        }
        _path.Add((match.Groups[1].Value[0], 
            int.Parse(match.Groups[2].Value), 
            int.Parse(match.Groups[3].Value.Substring(0, 5), NumberStyles.HexNumber),
            int.Parse(match.Groups[3].Value[5].ToString())));
    }

    [GeneratedRegex(@"([RDLU]) (\d+) \(#(\w+)\)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
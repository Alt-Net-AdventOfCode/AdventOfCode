using System.Data;
using System.Text;
using AoC;

namespace AdventCalendar2025;
[Day(4)]
public class DupdobDay04: SolverWithParser
{
    private List<string> _map;
    protected override void Parse(string data) => _map = data.SplitLines().ToList();

    [Example(1, """
                ..@@.@@@@.
                @@@.@.@.@@
                @@@@@.@.@@
                @.@@@@..@.
                @@.@@@@.@@
                .@@@@@@@.@
                .@.@.@.@@@
                @.@@@.@@@@
                .@@@@@@@@.
                @.@.@@@.@.
                """, 13)]
    public override object GetAnswer1()
    {
        var count = 0;
        for (var y = 0; y < _map.Count ; y++)
        {
            for (var x = 0; x < _map[y].Length; x++)
            {
                if (_map[y][x] != '@') continue;
                var neighbors = 0;
                for (var u = -1; u <= 1; u++)
                {
                    for (var v = -1; v <= 1; v++)
                    {
                        if (x + v < 0 || x + v >= _map[y].Length || y + u < 0 || y + u >= _map.Count ||
                            (v == 0 && u == 0)) continue;
                        if (_map[y + u][x + v] == '@') neighbors++;
                    }
                }

                if (neighbors < 4)
                {
                    count++;
                }
            }
        }
        return count;
    }

    [ReuseExample(1, 43)]
    public override object GetAnswer2()
    {
        var count = 0;
        var map = _map;
        int prevCount;
        do
        {
            prevCount = count;
            var nextMap = new List<string>();
            for (var y = 0; y < map.Count; y++)
            {
                var line = new StringBuilder();
                for (var x = 0; x < map[y].Length; x++)
                {
                    if (map[y][x] != '@')
                    {
                        line.Append('.');
                        continue;
                    }

                    var neighbors = 0;
                    for (var u = -1; u <= 1; u++)
                    {
                        for (var v = -1; v <= 1; v++)
                        {
                            if (x + v < 0 || x + v >= map[y].Length || y + u < 0 || y + u >= map.Count ||
                                (v == 0 && u == 0)) continue;
                            if (map[y + u][x + v] == '@') neighbors++;
                        }
                    }

                    if (neighbors < 4)
                    {
                        line.Append('.');
                        count++;
                    }
                    else
                    {
                        line.Append('@');
                    }
                }

                nextMap.Add(line.ToString());
            }

            map = nextMap;
        } while (prevCount != count);

        return count;
    }
}
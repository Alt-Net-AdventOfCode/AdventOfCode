using AoC;

namespace AdventCalendar2025;

[Day(5)]
public class DupdobDay05: SolverWithParser
{
    private List<(long low, long high)> _ranges = [];
    private List<long> _Ids=[];
    
    protected override void Parse(string data)
    {
        var blocks = data.SplitLineBlocks();
        foreach (var range in blocks[0])
        {
            var parts = range.Split('-');
            _ranges.Add((long.Parse(parts[0]), long.Parse(parts[1])));
        }
        _Ids = blocks[1].Select(long.Parse).ToList();
    }

    [Example(1, """
                3-5
                10-14
                16-20
                12-18
                
                1
                5
                8
                11
                17
                32
                """, 3)]    
    public override object GetAnswer1()
    {
        var count = 0;
        foreach (var id in _Ids)
        {
            foreach (var (low, high)  in _ranges)
            {
                if (id >= low && id <= high)
                {
                    count++;
                    break;
                }
            }
        }
        return count;
    }

    [ReuseExample(1,14)]
    public override object GetAnswer2()
    {
        var sortedRanges = _ranges.OrderBy(r => r.low).ToList();
        var mergedRanges = new List<(long low, long high)>();
        foreach (var range in sortedRanges)
        {
            if (mergedRanges.Count == 0 || range.low > mergedRanges.Last().high + 1)
            {
                mergedRanges.Add(range);
            }
            else if (mergedRanges.Last().high< range.high)
            {
                var last = mergedRanges.Last();
                mergedRanges[^1] = (last.low, range.high);
            }
        }

        var count = 0L;
        foreach (var (low, high)  in mergedRanges)
        {
            count += (high - low) + 1;
        }

        return count;
    }

}
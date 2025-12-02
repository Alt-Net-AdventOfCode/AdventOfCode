using System.Diagnostics;
using AoC;

namespace AdventCalendar2025;

[Day(2)]
public class DupdobDay02: SolverWithParser
{
    private List<(long start, long end)>? _ranges;

    
    protected override void Parse(string data)
    {
        _ranges = data.Split(',').Select(part =>
        {
            var splits = part.Split('-');
            return (long.Parse(splits[0]), long.Parse(splits[1]));
        }).ToList();
    }

    [Example(1,"11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124", 1227775554)]
    public override object GetAnswer1()
    {
        var total = 0L;
        Debug.Assert(_ranges != null, nameof(_ranges) + " != null");
        foreach (var (start, end) in _ranges)
        {
            var band = 10L;
            var range = 10L;
            while(start>band*10)
            {
                band*=100;
                range *= 10;
            }
            var sequenceStart = start / range;
            if (sequenceStart * (range + 1) < start)
            {
                sequenceStart++;
            }

            sequenceStart = Math.Max(sequenceStart, range/10);
            var sequenceEnd = end / range;
            if (sequenceEnd*(range+1) > end)
            {
                sequenceEnd--;
            }
            sequenceEnd = Math.Min(sequenceEnd, range - 1);
            if (sequenceEnd<sequenceStart)
            {
                continue;
            }

            total+=(sequenceEnd - sequenceStart + 1) * (sequenceEnd+sequenceStart)/2*(range +1);
        }

        return total;
    }
    private readonly int[][] _computedRanges =
    [
        [],   //1
        [11], // 2
        [111], // 3
        [0101], // 4
        [11111], // 5
        [001001, 010101, -111111], //6
        [1111111], //7
        [00010001], //8
        [001001001], // 9
        [0000100001, 0101010101, -1111111111] // 10
    ];
    
    [ReuseExample(1, 4174379265L)]
    public override object GetAnswer2()
    {
        var result = 0L;
        Debug.Assert(_ranges != null, nameof(_ranges) + " != null");
        foreach (var (start, end) in _ranges)
        {
            var index = 0;
            var block = 1L;
            // number of digits
            while (start>10*block)
            {
                block *= 10;
                index++;
            }

            var blockStart = start;
            // for each multiplier/range pair
            while (blockStart < end)
            {
                var blockEnd = Math.Min(end, block * 10 - 1);
                foreach (var multiplier  in _computedRanges[index])
                {
                    var mul = Math.Abs(multiplier);
                    var first = blockStart/mul;
                    if (blockStart%mul >0)
                    {
                        // round up
                        first++;
                    }
                    var last = blockEnd / mul;
                    result += (first + last) * (last - first + 1) * multiplier / 2;
                }

                index++;
                block *= 10;
                blockStart = blockEnd;
            }
        }

        return result;
    }
}
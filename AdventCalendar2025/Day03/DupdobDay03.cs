using System.Diagnostics;
using AoC;

namespace AdventCalendar2025;

[Day(3)]
public class DupdobDay03: SolverWithParser
{
    private List<List<int>>? _batteries;
    
    protected override void Parse(string data) => _batteries = data.SplitLines().Select( l=> l.Select(c => c-'0').ToList()).ToList();

    [Example(1, """
                987654321111111
                811111111111119
                234234234234278
                818181911112111
                """, 357)]
    public override object GetAnswer1()
    {
        var total = 0;
        Debug.Assert(_batteries != null, nameof(_batteries) + " != null");
        foreach (var batteryLine in _batteries)
        {
            var maxFirst = 0;
            var maxSecond = 0;
            for(var i= 0; i< batteryLine.Count; i++)
            {
                var battery = batteryLine[i];
                if (battery > maxFirst && i<batteryLine.Count -1)
                {
                    maxSecond = 0;
                    maxFirst = battery;
                }
                else if (battery > maxSecond)
                {
                    maxSecond = battery;
                }
            }

            total += maxFirst * 10 + maxSecond;
        }

        return total;
    }

    [ReuseExample(1, 3121910778619L)]
    public override object GetAnswer2()
    {
        var total = 0L;
        Debug.Assert(_batteries != null, nameof(_batteries) + " != null");
        foreach (var batteries in _batteries)
        {
            var maxBattery = 0L;
            var end = batteries.Count - 11;
            var start = 0;
            for (var i = 0; i < 12; i++)
            {
                var (next, pos) = GetMaximum(batteries[start..end]);
                maxBattery = maxBattery * 10 + next;
                start += pos + 1;
                end++;
            }
            total+= maxBattery;
        }
        return total;
    }
    
    private static (int max, int pos) GetMaximum(IEnumerable<int> batteries)
    {
        var max = 0;
        var pos = -1;
        var i = -1;
        foreach (var battery in batteries)
        {
            i++;
            if (battery <= max) continue;
            max = battery;
            pos = i;
            if (max == 9)
            {
                break;
            }
        }

        return (max, pos);
    }
}

using AoC;

namespace AdventCalendar2025;

[Day(1)]
public class DupdobDay01: SolverWithParser
{
    private int[]? _numbers;

    protected override void Parse(string data)
    {
        _numbers = data.SplitLines().Select(line => line[0]=='L' ? ( -int.Parse(line[1..]) ) : int.Parse(line[1..]) ).ToArray();
    }
    
    [Example(1, """
                L68
                L30
                R48
                L5
                R60
                L55
                L1
                L99
                R14
                L82
                """, 3)
    ]
    
    //method 0x434C49434B
    public override object GetAnswer1()
    {
        var password = 0;
        var dial = 50;
        foreach (var number in _numbers)
        {
            dial+=number;
            if (dial<0) dial+=100;
            dial%=100;
            if (dial == 0) password++;
        }
        return password;
    }

    [ReuseExample(1, 6)]
    //method 0x434C49434B
    public override object GetAnswer2()
    {
        var password = 0;
        var dial = 50;
        foreach (var number in _numbers)
        {
            // dial is always between 0 and 99
            dial+=number;
            if (number<0)
            {
                password += -dial / 100 + (dial > 0 || dial == number ? 0 : 1);
                dial = (dial%100+100)%100;
            }
            else
            {
                password+= dial/100;
                dial %= 100;
            }

        }

        return password;
    }
}
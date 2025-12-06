using AoC;

namespace AdventCalendar2025;

public class DupdobDay06: SolverWithParser
{
    private readonly List<List<long>> _operands = [];
    private List<string> _operations = [];
    private readonly List<List<long>> _operands2 = [];
    
    protected override void Parse(string data)
    {
        var lines = data.SplitLines();
        _operations = lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        for (var i = 0; i < _operations.Count; i++)
        {
            _operands.Add(new List<long>());
        }
        foreach (var line in lines[..^1])
        {
            var operands = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            for (var i = 0; i < operands.Count; i++)
            {
                _operands[i].Add(operands[i]);
            }
        }
        // parsing for second half
        var columns = new List<int>();
        for (var i = 0; i < lines[^1].Length; i++)
        {
            if (lines[^1][i] != ' ')
            {
                columns.Add(i);
                _operands2.Add(new List<long>());
            }
        }
        columns.Add(lines[^1].Length+1);
        //now compute operands
        for(var i = 0; i < columns.Count - 1; i++)
        {
            var start = columns[i];
            var end = columns[i + 1];
            for (var j = start; j < end-1; j++)
            {
                var value = 0;
                foreach (var line in lines[..^1])
                {
                    if (line[j] == ' ')
                    {
                       continue;
                    }

                    value = value * 10 + line[j] - '0';
                }
                _operands2[i].Add(value);
            }
        }
    }

    [Example(1, """
                        123 328  51 64 
                         45 64  387 23 
                          6 98  215 314
                        *   +   *   +  
                        """, 4277556)]
    public override object GetAnswer1()
    {
        var result = 0L;
        for (var i = 0; i < _operations.Count; i++)
        {
            if (_operations[i] == "+")
            {
                result += _operands[i].Sum();
            }
            else if (_operations[i] == "*")
            {
                result += _operands[i].Aggregate(1L, (current, val) => current * val);
            }
        }

        return result;
    }

    [ReuseExample(1,3263827)]
    public override object GetAnswer2()
    {
        var result = 0L;
        for (var i = 0; i < _operations.Count; i++)
        {
            if (_operations[i] == "+")
            {
                result += _operands2[i].Sum();
            }
            else if (_operations[i] == "*")
            {
                result += _operands2[i].Aggregate(1L, (current, val) => current * val);
            }
        }

        return result;
    }
}
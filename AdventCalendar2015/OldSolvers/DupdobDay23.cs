using System.Collections.Generic;
using System.Threading;
using AOCHelpers;

namespace AdventCalendar2015.OldSolvers
{
    public class DupdobDay23: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            _program.Add(line);
        }

        protected override void SetupTestData()
        {
            TestData = @"inc b
jio b, +2
tpl b
inc b";
            ExpectedResult1 = 2;
        }

        protected override void CleanUp()
        {
            _program.Clear();
        }

        public override object GiveAnswer1()
        {
            var registers = new Dictionary<string, int>() {["a"] = 0, ["b"] = 0};
            RunProgram(registers);
            return registers["b"];
        }

        public override object GiveAnswer2()
        {
            var registers = new Dictionary<string, int>() {["a"] = 1, ["b"] = 0};
            RunProgram(registers);
            return registers["b"];        
        }

        private void RunProgram(Dictionary<string, int> registers)
        {
            var ipc = 0;
            while (ipc < _program.Count)
            {
                var line = _program[ipc++];
                var (opcode, pars) = line.SplitAtFirst(' ');
                switch (opcode)
                {
                    case "hlf":
                        registers[pars] /= 2;
                        break;
                    case "tpl":
                        registers[pars] *= 3;
                        break;
                    case "inc":
                        registers[pars]++;
                        break;
                    case "jmp":
                        ipc = ipc - 1 + int.Parse(pars);
                        break;
                    case "jie":
                    {
                        var (register, offset) = pars.SplitAtFirst(',');
                        if (registers[register] % 2 == 0)
                        {
                            ipc = ipc - 1 + int.Parse(offset);
                        }

                        break;
                    }
                    case "jio":
                    {
                        var (register, offset) = pars.SplitAtFirst(',');
                        if (registers[register] == 1)
                        {
                            ipc = ipc - 1 + int.Parse(offset);
                        }

                        break;
                    }
                }
            }
        }

        private readonly List<string> _program = new();
        protected override string Input => @"jio a, +22
inc a
tpl a
tpl a
tpl a
inc a
tpl a
inc a
tpl a
inc a
inc a
tpl a
inc a
inc a
tpl a
inc a
inc a
tpl a
inc a
inc a
tpl a
jmp +19
tpl a
tpl a
tpl a
tpl a
inc a
inc a
tpl a
inc a
tpl a
inc a
inc a
tpl a
inc a
inc a
tpl a
inc a
tpl a
tpl a
jio a, +8
inc b
jie a, +4
tpl a
inc a
jmp +2
hlf a
jmp -7";
        public override int Day => 23;
    }
}
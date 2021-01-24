using System;
using System.Collections.Generic;
using AOCHelpers;

namespace AdventCalendar2016
{
    public class DupdobDay1 : DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            foreach (var bloc in line.Split(','))
            {
                var trimmed= bloc.Trim();
                _instructions.Add(new Instruction(trimmed[0], int.Parse(trimmed.Substring(1))));
            }
        }

        public override object GiveAnswer1()
        {
            (int x, int y) pos = (0, 0);
            var dir = 0;
            foreach (var instruction in _instructions)
            {
                dir = (dir+ (instruction.Turn == 'L' ? 3 : 1)) % vectors.Length;

                pos.x += instruction.Len * vectors[dir].dx;
                pos.y += instruction.Len * vectors[dir].dy;
            }

            return Math.Abs(pos.x) + Math.Abs(pos.y);
        }

        public override object GiveAnswer2()
        {
            (int x, int y) pos = (0, 0);
            var visited = new HashSet<(int, int)>();
            var dir = 0;
            var found = false;
            foreach (var instruction in _instructions)
            {
                dir = (dir+ (instruction.Turn == 'L' ? 3 : 1)) % vectors.Length;
                for (var i = 0; i < instruction.Len; i++)
                {
                    pos.x += vectors[dir].dx;
                    pos.y += vectors[dir].dy;
                    if (!visited.Add(pos))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }

            return Math.Abs(pos.x) + Math.Abs(pos.y);        
        }

        protected override void SetupTestData(int id)
        {
            _testData = "R5, L5, R5, R3";
            _expectedResult1 = 12;
        }

        protected override void SetupRunData()
        {
            _instructions.Clear();
        }

        private record Instruction(char Turn, int Len);

        private (int dx, int dy)[] vectors = {(0, -1), (1, 0), (0, 1), (-1, 0)};
        private readonly List<Instruction> _instructions = new List<Instruction>();
        protected override string Input => @"R3, L5, R2, L2, R1, L3, R1, R3, L4, R3, L1, L1, R1, L3, R2, L3, L2, R1, R1, L1, R4, L1, L4, R3, L2, L2, R1, L1, R5, R4, R2, L5, L2, R5, R5, L2, R3, R1, R1, L3, R1, L4, L4, L190, L5, L2, R4, L5, R4, R5, L4, R1, R2, L5, R50, L2, R1, R73, R1, L2, R191, R2, L4, R1, L5, L5, R5, L3, L5, L4, R4, R5, L4, R4, R4, R5, L2, L5, R3, L4, L4, L5, R2, R2, R2, R4, L3, R4, R5, L3, R5, L2, R3, L1, R2, R2, L3, L1, R5, L3, L5, R2, R4, R1, L1, L5, R3, R2, L3, L4, L5, L1, R3, L5, L2, R2, L3, L4, L1, R1, R4, R2, R2, R4, R2, R2, L3, L3, L4, R4, L4, L4, R1, L4, L4, R1, L2, R5, R2, R3, R3, L2, L5, R3, L3, R5, L2, R3, R2, L4, L3, L1, R2, L2, L3, L5, R3, L1, L3, L4, L3";
        public override int Day => 1;
    }
}
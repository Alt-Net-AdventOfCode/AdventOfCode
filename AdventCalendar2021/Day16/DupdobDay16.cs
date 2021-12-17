using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventCalendar2021
{
    public class DupdobDay16 : AdvancedDay
    {
        private Packet _data;
        
        public DupdobDay16() : base(16)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var bits = new StringBuilder(line.Length * 4);
            string[] hex =
            {
                "0000","0001","0010","0011","0100","0101","0110","0111",
                "1000","1001","1010","1011","1100","1101","1110","1111"
            };
            foreach (var car in line)
            {
                bits.Append(car is >= '0' and <= '9' ? hex[car - '0'] : hex[car - 'A' + 10]);
            }

            using var text = bits.ToString().GetEnumerator();
            text.MoveNext();
            _data = ReadPacket(text);
        }

        private Packet ReadPacket(CharEnumerator text)
        {
            Packet packet;
            int version;
            int typeId;
            try
            {
                version = (int)ReadBits(text, 3);
                typeId = (int)ReadBits(text, 3);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            if (typeId == 4)
            {
                // literal value;
                var buffer = new List<byte>();
                bool last;
                do
                {
                    last = ReadBits(text, 1) == 0;
                    buffer.Add((byte)ReadBits(text, 4));
                } while (last == false);

                packet = new Literal { Version = version, Value = buffer };
            }
            else
            {
                // this an operator
                var mode = ReadBits(text, 1) == 0;
                var subs = new List<Packet>();
                packet = new Operator { Version = version, Type = typeId, SubPackets = subs};
                if (mode)
                {
                    var length = (int)ReadBits(text, 15);
                    var subString = new StringBuilder(length);
                    for (var i = 0; i < length; i++)
                    {
                        subString.Append(text.Current);
                        text.MoveNext();
                    }

                    var sub = subString.ToString().GetEnumerator();
                    sub.MoveNext();
                    for(;;)
                    {
                        var readPacket = ReadPacket(sub);
                        if (readPacket == null)
                        {
                            break;
                        }
                        subs.Add(readPacket);
                    }
                }
                else
                {
                    var count = ReadBits(text, 11);
                    for (var i = 0; i < count; i++)
                    {
                        subs.Add(ReadPacket(text));
                    }
                }
            }

            return packet;
        }

        private long ReadBits(CharEnumerator text, int i)
        {
            var result = 0L;
            for (int j = 0; j < i; j++)
            {
                result <<= 1;
                result += text.Current == '1' ? 1 : 0;
                text.MoveNext();
            }
            return result;
        }

        private abstract class Packet
        {
            public long Version;

            public abstract long TotalVersion();
            public abstract long Evaluate();
        }

        private class Literal : Packet
        {
            public List<byte> Value;
            public override long TotalVersion() => Version;

            public override long Evaluate() => Value.Aggregate(0L, (current, b) => current * 16 + b);
        }
        
        private class Operator : Packet
        {
            public long Type;
            public List<Packet> SubPackets;
            public override long TotalVersion() => Version +  SubPackets.Sum(s=> s.TotalVersion());

            public override long Evaluate()
            {
                return Type switch
                {
                    0 => SubPackets.Sum(s => s.Evaluate()),
                    1 => SubPackets.Aggregate(1L, (l, packet) => l * packet.Evaluate()),
                    2 => SubPackets.Min(p => p.Evaluate()),
                    3 => SubPackets.Max(p => p.Evaluate()),
                    5 => SubPackets[0].Evaluate() > SubPackets[1].Evaluate() ? 1 : 0,
                    6 => SubPackets[0].Evaluate() < SubPackets[1].Evaluate() ? 1 : 0,
                    7 => SubPackets[0].Evaluate() == SubPackets[1].Evaluate() ? 1 : 0,
                    _ => 0L
                };
            }
        }
        public override object GiveAnswer1()
        {
            return _data.TotalVersion();
        }

        public override object GiveAnswer2()
        {
            return _data.Evaluate();
        }

        protected override void CleanUp()
        {
            _data = null;
        }

        protected override IEnumerable<(string intput, object result)> GetTestData(bool secondQuestion)
        {
            if (secondQuestion)
            {
                yield return ("C200B40A82", 3L);
                yield return ("04005AC33890", 54L);
                yield return ("880086C3E88112", 7L);
                yield return ("CE00C43D881120", 9L);
                yield return ("D8005AC2A8F0", 1L);
                yield return ("F600BC2D8F", 0L);
                yield return ("9C005AC2F8F0", 0L);
                yield return ("9C0141080250320F1802104A08", 1L);
            }
            else
            {
                yield return ("8A004A801A8002F478", 16L);
                yield return ("620080001611562C8802118E34", 12L);
                yield return ("C0015000016115A2E0802F182340", 23L);
                yield return ("A0016C880162017C3686B18A3D4780", 31L);
            }
        }
    }
}
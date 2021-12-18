using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace AdventCalendar2021
{
    public class DupdobDay18 : AdvancedDay
    {
        class NailNumber
        {
            private NailNumber _previous;
            private NailNumber _next;
            private NailNumber _left;
            private NailNumber _right;
            private int _value;

            private bool IsNumber => _left == null;

            private NailNumber(NailNumber other)
            {
                if (other.IsNumber)
                {
                    _value = other._value;
                }
                else
                {
                    _left = new NailNumber(other._left);
                    _right = new NailNumber(other._right);
                    _left.SetNext(_right.Left());
                    _right.SetPrevious(_left.Right());
                }
            }
            private NailNumber(NailNumber left, NailNumber right)
            {
                _left = left;
                _right = right;
                _left.SetNext(_right.Left());
                _right.SetPrevious(_left.Right());
            }

            private NailNumber Right()
            {
                return IsNumber ? this : _right.Right();
            }

            private NailNumber Left()
            {
                return IsNumber ? this : _left.Left();
            }

            private void SetNext(NailNumber next)
            {
                if (IsNumber)
                {
                    _next = next;
                }
                else
                {
                    Right().SetNext(next);
                }
            }

            private void SetPrevious(NailNumber previous)
            {
                if (IsNumber)
                {
                    _previous = previous;
                }
                else
                {
                    Left().SetPrevious(previous);
                }
            }
            private NailNumber(int parse)
            {
                _value = parse;
            }

            private NailNumber CheckSplit(ref bool split)
            {
                if (IsNumber)
                {
                    if (_value > 9)
                    {
                        var node = new NailNumber(new NailNumber(_value / 2), new NailNumber((_value + 1) / 2));
                        node._left.SetPrevious(_previous);
                        _previous?.SetNext(node._left);
                        node._right.SetNext(_next);
                        _next?.SetPrevious(node._right);
                        split = true;
                        return node;
                    }
                }
                else
                {
                    _left = _left.CheckSplit(ref split);
                    if (split)
                    {
                        return this;
                    }
                    _right = _right.CheckSplit(ref split);
                }
                return this;
            }

            private NailNumber CheckExplode(ref bool explode, int depth = 0)
            {
                if (IsNumber)
                {
                    return this;
                }

                if (depth == 4)
                {
                    // too deep
                    explode = true;
                    var node = new NailNumber(0);
                    if (_left._previous != null)
                    {
                        _left._previous._value += _left._value;
                        node.SetPrevious(_left._previous);
                        _left._previous.SetNext(node);
                    }

                    if (_right._next == null) return node;
                    _right._next._value += _right._value;
                    node.SetNext(_right._next);
                    _right._next.SetPrevious(node);
                    return node;
                }

                _left = _left.CheckExplode(ref explode, depth + 1);
                _right = _right.CheckExplode(ref explode, depth + 1);
                return this;
            }
            
            public static NailNumber Parse(CharEnumerator enumerator)
            {
                NailNumber result = null;
                if (!enumerator.MoveNext()) return result;
                switch (enumerator.Current)
                {
                    case '[':
                        var left = NailNumber.Parse(enumerator);
                        enumerator.MoveNext();
                        if (enumerator.Current != ',')
                        {
                            throw new InvalidOperationException("Failed to parse");
                        }
                        var right = NailNumber.Parse(enumerator);
                        enumerator.MoveNext();
                        if (enumerator.Current != ']')
                        {
                            throw new InvalidOperationException("Failed to parse");
                        }

                        result = new NailNumber(left, right);
                        break;
                    case >='0' and <='9':
                        result = new NailNumber(enumerator.Current - '0');
                        break;
                }
                return result;
            }

            public NailNumber Add(NailNumber a)
            {
                var result = new NailNumber(new NailNumber(this), new NailNumber(a));
                bool redo;
                do
                {
                    redo = false;
                    result.CheckExplode(ref redo);
                    if (!redo)
                    {
                        result.CheckSplit(ref redo);
                    }
                } while (redo);

                return result;
            }
            public override string ToString()
            {
                if (IsNumber)
                    return _value.ToString();
                return $"[{_left},{_right}]";
            }

            public long Score()
            {
                if (IsNumber)
                {
                    return _value;
                }
                else
                {
                    return 3 * _left.Score() + 2 * _right.Score();
                }
            }
        }

        private readonly List<NailNumber> _data = new();

        public DupdobDay18() : base(18)
        {
        }

        protected override void ParseLine(int index, string line)
        {
            var nailNumber = NailNumber.Parse(line.GetEnumerator());
            _data.Add(nailNumber);
        }

        protected override void CleanUp()
        {
            _data.Clear();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData1()
        {
            yield return (@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]", 4140L);
        }

        
        public override object GiveAnswer1()
        {
            var a = _data[0];
            for (var i = 1; i < _data.Count; i++)
            {
                a = a.Add(_data[i]);
            }
            return a.Score();
        }

        protected override IEnumerable<(string intput, object result)> GetTestData2()
        {
            yield return (@"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]", 3993L);
        }

        public override object GiveAnswer2()
        {
            var result = 0L;
            (int i, int j) max = (0, 0);
            for (var i = 0; i < _data.Count; i++)
            {
                for (var j = 0; j < _data.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var addition = _data[i].Add(_data[j]);
                    var nextScore = addition.Score();
                    if (nextScore > result)
                    {
                        result = nextScore;
                        max = (i, j);
                    }
                }
            }

            return result;
        }
    }
}
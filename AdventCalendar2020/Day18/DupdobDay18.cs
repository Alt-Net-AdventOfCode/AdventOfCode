using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mail;
using AOCHelpers;

namespace AdventCalendar2020.Day18
{
    public class DupdobDay18: DupdobDayWithTest
    {
        protected override void ParseLine(int index, string line)
        {
            _expressions.Add(line);
        }

        protected override void SetupTestData(int id)
        {
            _testData = @"((2+4*9)*(6+9*8+6)+6)+2+4*2
2 * 3 + (4 * 5)
5 + (8 * 3 + 9 + 3 * 4 * 3)";
            _expectedResult1 = 14095L;
            _expectedResult2 = 23340+1491L;
        }

        protected override void SetupRunData()
        {
            _expressions.Clear();
        }

        public override object GiveAnswer1()
        {
            var expressions = new List<Expression>();
            foreach (var line in _expressions)
            {
                var stack = new Stack<Expression>();
                foreach (var car in line)
                {
                    switch (car)
                    {
                        case ' ':
                            continue;
                        case >= '0' and <= '9':
                        {
                            var digit = new Digit();
                            digit.Value = car - '0';
                            if (stack.Count>0 && stack.Peek() is BinaryExpression binExp)
                            {
                                binExp.operand2 = digit;
                            }
                            else
                            {
                                stack.Push(digit);
                            }

                            break;
                        }
                        case '(':
                        {
                            var sub = new SubExpression();
                            stack.Push(sub);
                            break;
                        }
                        case '*':
                        case '+':
                        case '-':
                        {
                            var bin = new BinaryExpression();
                            bin.operation = car;
                            bin.operand1 = stack.Pop();
                            stack.Push(bin);
                            break;
                        }
                        case ')':
                        {
                            var subExp = stack.Pop();
                            var top = stack.Pop() as SubExpression;
                            top.Sub = subExp;
                            if (stack.Count>0 && stack.Peek() is BinaryExpression binExp)
                            {
                                binExp.operand2 = top;
                            }
                            else
                            {
                                stack.Push(top);
                            }

                            break;
                        }
                    }
                }
                expressions.Add(stack.Pop());
            }
            return expressions.Sum(t => t.Result);
        }
        
        //48378251354718 too low

         public override object GiveAnswer2()
        {
            var expressions = new List<Expression>();
            foreach (var line in _expressions)
            {
                var stack = new Stack<Expression>();
                foreach (var car in line)
                {
                    switch (car)
                    {
                        case ' ':
                            continue;
                        case >= '0' and <= '9':
                        {
                            var digit = new Digit {Value = car - '0'};
                            if (stack.Count>0 && stack.Peek() is BinaryExpression binExp)
                            {
                                binExp.operand2 = digit;
                            }
                            else
                            {
                                stack.Push(digit);
                            }

                            break;
                        }
                        case '(':
                        {
                            var sub = new SubExpression();
                            stack.Push(sub);
                            break;
                        }
                        case '*':
                        {
                            var previous = stack.Pop();
                            if (stack.TryPeek(out var topexp) && topexp is SubExpression {Auto: true} subexp)
                            {
                                subexp.Sub = previous;
                                previous = stack.Pop();
                                if (stack.TryPeek(out topexp) && topexp is BinaryExpression binExp)
                                {
                                    binExp.operand2 = previous;
                                    previous = stack.Pop();
                                }
                            }
                            var bin = new BinaryExpression {operation = car, operand1 = previous};
                            stack.Push(bin);
                            break;
                        }
                        case '+':
                        {
                            var bin = new BinaryExpression {operation = car};
                            var previous = stack.Pop();
                            if (previous is BinaryExpression {operation:'*'})
                            {
                                stack.Push(previous);
                                Expression sub = new SubExpression(true);
                                stack.Push(sub);
                                if (previous is BinaryExpression {operation:'*'} mult)
                                {
                                    bin.operand1 = mult.operand2;
                                }
                            }
                            else
                            {
                                bin.operand1 = previous;
                            }
                            stack.Push(bin);
                            break;
                        }
                        case ')':
                        {
                            var subExp = stack.Pop();
                            var top = stack.Pop() as SubExpression;
                            if (top.Auto)
                            {
                                if (stack.TryPeek(out var topExpression) && topExpression is BinaryExpression topMult)
                                {
                                    topMult.operand2 = subExp;
                                    stack.Pop();
                                    subExp = topMult;
                                    top = stack.Pop() as SubExpression;
                                }
                            }
                            if (stack.TryPeek(out var topexp) && topexp is SubExpression {Auto: true} subexp)
                            {
                                subexp.Sub = subExp;
                                if (stack.TryPeek(out var topExpression) && topExpression is BinaryExpression topMult)
                                {
                                    topMult.operand2 = subexp;
                                }
                                subExp = stack.Pop();
                            }
                            top.Sub = subExp;
                            if (stack.Count>0 && stack.Peek() is BinaryExpression binExp)
                            {
                                binExp.operand2 = top;
                            }
                            else
                            {
                                stack.Push(top);
                            }

                            break;
                        }
                    }
                }

                var lastExpression = stack.Pop();
                if (stack.TryPeek(out var secondLast) && secondLast is SubExpression {Auto: true} lastSub)
                {
                    lastSub.Sub = lastExpression;
                    lastExpression = stack.Pop();
                    if (stack.TryPeek(out var lastBin) && lastBin is BinaryExpression binExp)
                    {
                        binExp.operand2 = lastExpression;
                        lastExpression = stack.Pop();
                    }
                }
                expressions.Add(lastExpression);
                if (lastExpression.Result == 0)
                {
                    
                }
            }
            return expressions.Sum(t => t.Result);
        }

        private readonly List<string> _expressions = new List<string>();

        abstract class Expression
        {
            public abstract long Result { get; }
        }

        class Digit: Expression
        {
            public int Value { get; set; }

            public override long Result => Value;
        }

        class SubExpression : Expression
        {
            public SubExpression(bool auto = false)
            {
                Auto = auto;
            }

            public bool Auto;
            public Expression Sub { get; set; }
            public override long Result => Sub.Result;
        }
        
        class BinaryExpression : Expression
        {
            public Expression operand1;
            public Expression operand2;
            public char operation;

            public override long Result
            {
                get
                {
                    return operation switch
                    {
                        '+' => operand1.Result + operand2.Result,
                        '-' => operand1.Result - operand2.Result,
                        '*' => operand1.Result * operand2.Result,
                        _ => 0
                    };
                }
            }
        } 
        
        protected override string Input => @"(7 * (3 + 8 + 8 + 7) + (6 + 8 * 2 + 5 + 2 * 6) * (5 + 2) * 9) + ((7 * 4 + 8) * 6 * 8 + 9) * 7 * 2 * 2
6 * ((9 + 4) * (6 * 7 + 5 + 8 * 2))
7 * 8 + 2 + 8 * (8 * 4) * (4 + 8)
(9 + 3 + 2 * 5 * 8) + 9 + 5 * 2 * 5 * (6 * 6 * 4 + 6 * 9 * 3)
6 * 6 * 4 * (6 + (3 * 9 * 2) + 9 + (4 + 7 + 7))
7 * ((4 * 6 + 4 + 6 * 8 + 6) + (2 * 7 * 8 + 5 * 3 + 7) + (5 * 5 * 5 * 7) + 8 * 7)
(8 + 9 * 7 * 9 + 6) + (7 + 5) + 6
5 * (6 * (3 + 2 + 9)) + 8 + 3 + 5 * (4 + 3 * 8 * 8 * 6 * 2)
4 + (3 + 4 * (4 + 9 + 3) * (4 * 8 * 4 * 9))
4 * 4 * 6 * 2 + ((8 * 4 * 3 + 6) * 5) + 6
(8 * (8 + 5 + 5 * 7) * 9 + 2 + 9) * 4 + 9 * ((3 + 3) + 8) + 9
((9 + 8 * 3 + 2 + 9 + 8) * 3 + (3 + 7 * 6) * 9 * 2) + 3 * 7
5 + 5 * (5 * (6 + 6 + 6 + 7) * 7 + (9 * 4) * 4 * (8 * 9))
7 * (2 * 7 + 3 + 5 + 5 * 2) + 4 + 3 * 3 + 5
(4 + 5 * (2 * 4 + 5 * 2 * 7 + 5) + 2 + 9) + 4 + 9 * 9 * 3
7 * 6 + (6 * 2 + 4 + 5 * 8) + (6 * 2 * (4 * 7 * 2) + 9 * (5 * 2 * 4) + (3 + 2 * 5)) * 8
2 + ((9 * 3 * 3 * 3) * (5 * 9 + 6 + 7 + 6 + 8) * 3)
7 * ((9 + 7 + 4 * 3 * 4 + 4) + 4 * 3) * 5 * 3 * 7
8 * (3 + 5 * 9 + 4) * 6 + 4
7 + 6 * 7 * ((7 * 6 * 2) * 3) * 5 * 7
8 + 5 * 4 * (3 + 5 * (8 + 5 * 2) + 6) + 6
4 * 8 + ((3 * 5 * 4) + 7) + (4 * 2)
((5 + 8 + 7 + 6 * 6 + 3) + 4 * 3 * (5 * 2) * 5 * (2 * 9)) + 6 + 3 * (2 * (2 + 4 + 8) * 4) + (9 + 3 + 6 * (9 + 5 * 6 * 3 * 8 * 5) + (4 * 6) + (6 * 3 + 2 + 8))
(3 + 4 + 8) + 6 * (7 * 6 * (9 * 3 * 5 * 5 * 4 * 5) + (6 + 4) * (5 * 2 * 3 + 3) + (2 * 7)) + 2 * 4
6 * (6 * 9 * (3 * 9) * 4 + 9) + 6
4 + 6 * 7 * ((3 + 5 + 3) * (6 + 3 * 9 + 6 * 8 * 9) + 6 * 4) * 8
5 + (6 + 9 * (7 * 8 * 8 * 3 * 4) + 2) * 4 * 2
(2 * 7 * 6 + 9 + (3 * 6 + 4 * 7 + 5) + 8) + 7 + 7 * 8
2 + (6 + 4 + (5 + 5) * 7) * 2 + 7 * 5
4 + 9 * 3 + 8 * 6
(5 + 5 + 9 * 8 * (4 * 5 + 6 + 3 * 8 * 3) + 6) + 9 + 9
(8 * 4 * 7 + 9 + 6 * 9) * (9 + 2 * 2) + 3
((9 * 4 + 8 + 4 * 2 + 2) * 3 + (6 + 9 * 5 * 4) * 4 + 6 * 5) * 5 * 3 * 5 + (3 + 5 * 4)
5 + (5 * 3) * (2 + 8) * (3 + 6 + 4 * 6 + 7 + 7) * 5 * 7
6 * 6 * 7 * (5 * 4 + 2 + 6 + (8 + 6 + 4 + 9 * 5))
2 * 9 + (2 * (8 * 7 + 6 + 5 + 3) + 2 + 9 * (3 * 9 * 4 * 7 * 7 + 5) + 3) + (4 + 6 * 4 + 6)
(9 + (8 * 8 + 2 + 2 * 6 + 6)) * 8 + 4 + 3 + (9 + 3 + 9 + 9 * 4 * 8) * 2
8 * 4 * (4 * (3 + 7 * 7 + 3) + 8 * (5 + 2 + 3) * 2) + 9
5 + (6 * (3 * 3 + 7 * 3) * (4 * 4) + 2 * 4 * 4)
3 + (5 * 8 * 9 * 3 + 9 * (6 * 7)) * 9 + 3
3 * (9 * 3 + (5 + 4 * 7 + 3 + 8)) + 4 * 2 * 3
8 * 9 + 6 + (7 + 7 + 9 * 8 * 5 * 6) * 5
(8 + 9 + 3 + 7 * 7) + 2 + 9 + (8 + 7 + 4 + 2) + 9
4 * (2 * (7 * 4 + 2 + 9 + 6 + 7) + (4 + 2) * 3 + 9 + 6) + 6 * 4 + 7 * 5
(3 * 4 + (5 * 8 * 2) * (7 + 8)) + 7 + 8 + 5 + ((6 * 9) * 4 * 5)
4 + ((9 + 9) + (7 * 8 + 2 * 8 * 2) + 9) + 2 + 7 * 2
8 * 3 * (9 + 5 + 8 + 5)
4 + 4 + (6 + 2 * 7 + 9 + 6 * 5)
((8 + 3 * 8 * 2) + 7 + 7 * (7 * 7 * 7 + 3) * 9 * (9 * 4 + 7)) + 4 + 6
6 + (7 * 2 + 4 + 5 + 4) + 9 + ((7 * 9) * 7) * 5
(7 * 3 * 2) * ((9 + 6 * 2 * 6 * 3) + (6 * 7) + 3 + (6 + 5 * 5 + 4 + 9 * 6) * 4 + 9) + 4 + 7 + 6
9 * (3 * (9 + 8)) * 4
6 * (4 * 2 * (9 * 5 * 4 + 7 + 9) + 6 + (3 + 6 * 2)) * 5 * (3 * (2 + 6 * 8 * 6 + 3 + 7) * 8 * 7 * (9 * 8 * 7 + 7 + 9) * 7) + 3 + 6
7 + 3 * ((6 * 3 * 9) + 3 * 9)
7 * ((4 + 4 * 4 * 9 * 7) + 2 * 9) * 8 * 6 * 6 + 3
6 + 5 + (5 + 3 * (3 + 7 + 9 * 3) * 6) + 3 + ((2 * 3 * 4 + 4) * 6 + (3 + 3) + 2)
9 + ((8 * 9 * 3 * 2) * 5 + 8)
(2 + 7) * 3 * (8 + 5) + 9
4 * (5 + 4 + 6 * (9 * 4 + 8 * 4 * 7 * 3) * (4 * 4 * 6) * (9 + 7 + 8 * 4)) * (4 + 4 + 9 + 8) + 7 * 8 + 5
((7 * 4 * 4 * 3) * 4 + 2 * 2 + 2) * 9 + 9 * 8 + 9
4 + 6 + 2 * (5 + 3 + 8 * 4)
((7 + 6) + 3 * 3 * 5) * ((9 * 3 + 5 + 8) + (3 + 8 * 8 + 7 + 5 * 6) + 2 * 5 + (7 + 9 + 2)) + (4 * 2 * (5 + 8 + 5 + 3) * 5 + 6 * 2) + 6 * 5 * (2 * (9 + 4) * (7 * 8) * 8 + 7)
6 + 5 + 5 + 2 * 5
5 + 5 + 8 * 8 + 2 + 6
(3 * (4 * 6 * 8 * 2)) + 3
(5 + 6 * 3 + 5 * 2) * 5 * (3 * 5 + 2 * 7 * 9 * 3) + 5 * ((6 + 2 * 7 + 2 + 4 + 2) * 9 + 6) + 7
(7 + 7 * 4 * 7) * ((8 * 5 * 9) + 6 * 9 + 3 * 3 * 9) * 8 * ((3 * 5 * 2 + 6 + 5) * 7 + (6 * 2 * 7 * 7) + 8 + 8 + 6)
(3 * 6 + 7 + 6 + 3 + 7) * 4 * 7 * ((9 + 3 + 6) * 5 + 3 + (5 * 7 + 4 + 5 * 2) + 5) * 5 * 9
7 * ((7 + 6 + 2 + 9) + 3 * 4 + 9 + 2 + 2) + 2
3 * 8 * 2 * (6 * 2 * 2 + 4 * 8) + 8 + 7
8 + 3 + ((4 * 6 * 5 * 7) * 9 + 9 + 8 * 4)
(6 + 4 + (6 + 3 + 3 + 3) + 6) * 9
2 * 6 * (5 + 2) * (6 * 6 + 7 + 2)
4 + (8 + 2 + 4) * 4 * 5 + (9 * 5 + 9) + 8
6 * 3 * 9 + 6 + (2 * 4 * 3 + 2 * 5 * 3) * 6
2 + (8 + 5 * 7 + 4 + 7 * (2 + 9 * 6 * 5 * 3 + 3)) + 9 * 4
(9 + (2 + 6 + 7 * 4 * 8) + 9 * 4 + 9) * 3 + 8 * 8
2 * (9 * 5 * 9 + (8 * 9 + 5)) * 8 * 8
3 + 6 + 2 * 3 * 9 + 9
8 * 7 + 8 * (5 + 6) * 5 + 4
4 * (6 * 2 * (3 + 6 * 3) + 7 * 5 + 6)
(7 * 3 * 7 * 4 * 3 * 2) * (9 + 8 * 9) + 9 * (4 * 5 + 3 + 5 + 9)
9 + 8 * 3 * 4 + (9 + 8 * 9) + (4 * (6 * 4 * 9 * 5 + 6 + 4) + 8 + 7 + (4 * 9))
2 * (4 * (3 + 4 + 8 + 5) * 8 * 6)
8 * (8 * 2 * 6 + 6 + 6) * 8
(9 + 3 * 4 * 9 + 5 * 5) * 4
(9 * 9 * 4 * 4 + (6 * 2) + (3 + 3 * 2)) + 3 + ((2 * 3 + 8 * 4 * 8 * 2) + 2 + 3)
7 * (9 * (6 + 3 + 7)) * ((8 + 3 + 3 * 3 + 3 + 7) + 4 + 5 + 3 * (6 * 4 * 6 * 6 + 2 * 5) + (2 + 3 + 9 + 3 * 6 + 4)) + 6
8 + 6 * (3 + 5 + 6 + 9) * 4
(6 + 7 + 7 + 7 * 6 + 5) + 3 * 5 * 4 * 5 * 6
(2 * 9 + 9 + 3 * 7 + 6) + 3 + 7 + 4 + 9
(7 + (8 * 7 * 4 * 7 * 2 + 4) + (5 + 2 * 2 + 2 * 2 * 6)) * 3 * 4 + (4 * 2 + 4 * 6 + 3 * (8 * 6 + 4 * 3 * 7)) * 6
2 * (3 * (5 * 7 * 8 * 7 + 8) + (6 * 3 + 5) + 5 + 9)
8 * 4 + ((7 + 2 + 8) + (2 * 8 * 4 * 4 + 4 * 7) * 2) * 5 * 4
2 * 6 * 2
2 + (2 + 6 * 5 * 9) + ((8 + 2 * 6 * 5 * 5) * 7 + 3) + 3
((9 + 4 + 6 * 3 + 9) * 5) * (7 * 2 * 7 * 5 * 2) + 7 + 9 + 7
2 + 2 * 8 + 8
4 + 6 + 4 * 7
4 + 5 + 5 * 8 * (8 + 4 + (2 * 8 + 9 * 9 + 4 + 5))
(7 + 2 + (8 * 4 * 2 + 9 * 6) * 4) * 2 + (9 * 2 * 8 * 9 * 4)
4 + 9 + 6 * (4 * 6) + 7
6 + 4 + 8 * (7 * 9 + 7)
3 * 2 + 2 + 9 + (2 * 9 * 2 * 8 * 2 * (2 + 8 + 9 * 9 + 7)) * 7
9 + 9 + 5
(2 + 3 * 3) + 7
(9 * 7 + (8 + 8 * 6) + 2 * 7 * 3) + 6 * (5 * (2 + 9 + 9 + 6) + 6) * 8 * (4 + 4 * 6 * 5 + 5) * 8
7 + 4 + (4 * 2 + (2 * 7 + 2 + 4 + 2 + 5) + 9 * (9 * 7 + 7)) + 9 + 7 + (8 + 9 * 7 + 2 + (7 * 8 * 5 * 4 + 6 + 8))
(8 * 3) * (9 + 2 * 7 * 7 + 7) * 6 * 8 + 8 + (9 + 9 + 3 * 5 + 3)
4 * 5 + 2 + 4
((7 + 3 * 8 + 3) * 8 + 4 + 4 * 4 + 8) * 5 + 9 * (3 * 9 + 8 * 7 * 9 * 4) + 7
(8 + 6 + 7 + 9) + 8 * 6 * (5 + 2 + 3) * 8
(9 * 8 * (6 * 5) + 9) + 4 + 8 + 8 * 8
(4 + 5 * 9) * 7 + 7 * 6 + 8 * (2 + 5 * 8)
(4 + 3 + (6 + 2 + 2 + 6 * 6 * 2) * 4 * 2 * 9) + 3
8 * (5 + 8 + 2 * 7) + 2 + ((5 * 6 + 4 + 7 + 6 * 5) * 7 * 3 + 8)
2 + 5 + (6 * 3 + 2) + 8
2 + 9 * 6 * 5 + 5
3 * 3 + 7
(9 + 4 * 9) + 5 + (2 * 6 * (6 + 9) + 2 + 6) * 2 * ((9 * 6 + 7 * 3) + 5 * 2 + 3 * 9)
2 + 6 + ((6 * 9 * 9 * 8 * 9) + 9) * (5 + 2 * 5 * (3 + 5 * 8 * 5 * 2 * 9) * 8) * ((7 + 9 + 3 + 6 + 5) * 4 * 5)
3 * 9 * (5 + (3 * 4 * 2 + 8) + 5 * 3) + 4
9 * (9 * 3 + 3 + 4 + 7 + 4) + 3
2 * 9 + 3 * 9 + 5 + 9
(6 + (6 * 6 * 6 + 7 + 7 + 5) * 5) + 3 + 4 * 9
(2 * 8 * 8 * 6 * 8) + (5 + (5 * 6) * (8 + 5 + 7 * 7 + 2) + (6 * 4 + 7 + 5) + (3 + 5 + 8 + 6)) * 8 + ((8 * 3 + 3 * 7) * 4 + (3 * 8 * 4))
4 + ((9 * 2 * 9 * 9 * 7 + 5) * 6) * 7 * 7 + 5 * 8
(3 * 6 + 6 * (5 + 6 * 4 * 6 + 2 + 9)) * 9 * 9 + (2 * 7 * 2 * 5)
6 + 2 + (8 + 6)
6 + 3 * (3 + 3) + 8 * (9 * 6)
(2 * 8 + 4 + 5) + 7
7 + (2 * 9 * 5 * 6)
2 + 9 + ((7 + 3 + 3 * 9) * 6 + (5 + 2 + 8 + 5 * 6) * 7 + 8)
((4 + 9 + 8 * 6 + 8 * 9) + 8) * 6
9 * (9 * 3 + (3 * 8 * 8 * 7) * (4 + 8) + 8 * (5 * 3 + 2 + 4 * 3))
9 * (7 + (3 * 5 * 5 + 3 * 7)) + (5 * 3 + (4 * 7 * 7 + 8) * (3 * 7 * 5 * 6) * 8) * 5 + (2 * 7) * 4
4 * ((2 + 3 * 2 * 2 + 3) + 4 * 4 * 4 * 5 + (3 + 3 * 9 * 9 + 7)) * 9
8 + 7 * 4 * 3 * (5 + 9 + 8 * 7 * 3)
9 + (4 * (2 * 6 + 7) + 5 * 4 + 8 + (6 + 7 * 8 * 2)) + 9 + 5 + 8
3 * ((5 * 2 * 8 * 7 * 2 + 4) * (3 * 3 * 5 + 2) + 7 + 8) * 8 * 2 + 3
(8 * 2 + 5 + (4 * 9 * 7)) * 5 + (9 * 8) * (5 * (7 * 8 + 4 * 4) + 3)
((7 * 6 + 9 + 7 + 9 + 9) * 2 * 8 + (6 + 4 * 7 * 2 * 2) * 7 + (2 + 9 + 9 + 5 + 4)) * 5 * 2 + 7
7 + 2
4 * ((2 * 6) * 5 * 5 + 9 * (7 * 7 * 3 + 8)) + 2
6 * 8 + 5 + 4 * (5 + 8)
4 + 6 * (4 * 5 * 3 * 5 * 5 * 6) * 8 * 5
7 + ((6 * 6 + 2) + 7 * 7 + 7 * 9 * 6) + 6
(7 + 3 * (2 * 5 + 3 * 9)) + 2 * 2
6 + 3
2 * 8 * (3 + 8 * (4 + 6 * 4) * 8 * 2)
8 * ((4 + 8 * 5 + 7) + 6 + (4 + 8 + 4 + 9 * 7 + 9) + 2) + 5
(7 + 7 + 2 + 3 + 6 * 3) * ((7 + 5 + 8 + 5 * 6) + 4 + 6 * (3 + 3) * (4 + 6 * 9 * 6 + 4) * 7) * 3 * 9 + 5 + 4
5 * 7 * (5 * (2 + 5 * 7 + 7 * 5) + 9) + 3
8 + 2 * 4 * 3 + (8 + 3 + 4 + 3 + 2 * (4 * 5)) * 5
3 + (8 * 3) * 7 + 4 * 5 * 2
(7 * (7 * 2 * 7 * 3) * 3 + 2 + 7 * 6) * 4 * 6 + 6 + 6
(5 + 2 + 2 * 8 + (2 * 9 + 7 + 3) + 5) * 9
(6 + (7 + 8 * 2 + 9) + (7 * 3 * 4 * 5) + 6 * 3 * 4) + (3 * 2 * 5)
4 + 7 + (8 * 7 + (5 * 4))
8 * 2 * 5 * (9 + 4) + (3 * 7)
(3 + 4 + 8 + 6 + 2) * 9 + 6 + 6 + 3
(9 + (3 * 4 * 5 * 8 + 5) + (5 + 8)) + 5 * ((2 + 5) + (7 + 9) * 2) * (6 * 3) + 8
9 * (3 + 9 + 8 * (9 + 7 + 9 + 8 * 6) + 8 * 8) + 2
7 * ((9 * 2 + 8 * 4 + 7) * 4 + 3 + (5 * 3 * 5 + 3 + 4)) * 5 * 7
5 + 2 + (8 * 8) + 8
4 * (4 * 9 + (7 * 6 + 2 * 2 + 5 + 8) * 9 + 7) * 3 * 7
9 + 9 + (4 * (8 + 2 * 5 * 9 + 9 * 2) * 4 * 7) + (6 + 2) * ((8 * 7) + 9 * (7 + 7 * 7) + 2 + 5 + 2) + 6
9 + 4 + (3 * 9 * 3 * 9) + (3 + (2 + 9 + 9 + 8 + 7) + 5 + 9 * 3) * 7 * (4 + 7 * 8 + 7 * (4 * 2 + 5 * 8 + 9 + 8) + 3)
6 + (2 + 6) * (6 * 2 * 2 + 7) * 4 * 6
6 * (2 + (6 * 7 + 7 * 9) * 6) * 3 * 8 + 3
8 + (5 + 4 + 6)
9 + 2 * (5 * 5 * 5) * 2 * 5
4 * 4 * ((3 * 3) * (9 + 7 + 5) + 5 + 9) + 7 * (4 * 2 * 4) * 7
((4 * 5 * 7 * 7 + 3) * 9) * (6 * (7 * 8 * 7 + 3 + 7) + (4 * 2 + 3 + 3) * 4 + 6)
4 * 2 + 3 + 2 * (2 + 2 * 2 * 8)
4 + ((3 + 8) * 7 + 2 + (9 + 4 + 9 + 3 + 4 + 6) * (7 * 5 + 8)) + 4 + 5 * 8 + 2
(5 * 2) + 3 + (3 + 9) + 4 + 4 * 5
6 + (5 + 8 + 6 + (7 + 5 + 9)) + (3 * 5 + 8 * 3 * 4) * 4 + 4 + 5
((7 + 4 * 6) * 7 * 2 + 6) * 7 + 8
7 * 5
2 + 9 * 4 + 9 + 4 + (8 * 9)
5 + 5 * ((5 + 6) + 2 * 4) * (8 + 8 * 5) + 3
5 * 5
4 * 3 * 4 + 3 * (7 * 9) + 4
4 + 4 * (8 * 3 + 2 * 8 + 2) + 4 + 3
(8 + 5 + 4 * 4 + 6 * 9) + 8 * 6 * 3
8 + 6 + 3 + ((2 * 9 + 2) + (7 + 7 + 3)) + 6
(4 + 5 + 8) * 8 + (7 * 9 + 3 * 3 + 9 + (8 + 5 + 7 + 3 + 9)) + (3 * 3) * 8
(4 + 4 + 4 * 6 + 4 + 9) * 6 * ((8 + 6 + 3 + 2 * 8 + 2) * 4 + 6 * 8 * 7) * 5
6 * 6 + 3 * (2 + 7 + 3) * 4 + 5
((2 * 4 + 3 * 3 * 9 * 3) * 7) + 2 + 8 + 6 * 2
5 * (5 * 4) + 6
((7 + 6) + 7) + 3 * 2 * 9 * 6 + 4
6 + 8 + ((4 * 9 + 5) * 4) * (5 + 7 + 8 * 8 * 2) + 9
8 * 4 * 8 * 9 * 2
5 + 2 + (8 + 9 * 6 * 7 * 6) * 8
6 * (9 * 5) + (5 + 2 + 6 * 8) * 6 * 6 * (4 + 7 + 8 * 8)
2 * 3 * 6 * 8
3 + (4 + 5 * 8 * 8) * 2 * (9 * (5 * 3 + 8 + 6) * 2 + (5 * 7 * 3) * (4 + 5) + 7)
6 * (5 * 4) * 5 + 4 + (6 + 2) + 6
2 * (4 * (7 + 2 + 3 + 3 * 8) * (9 + 3 * 3) + (6 * 8 * 5 * 7) + (6 + 5) + 7) * (3 + 8 + (5 + 4 + 8 + 6) * 3 + 2 + 3) * 8 + (3 * 3) * 9
(5 + (4 * 8 * 7) + 7 * (7 * 9 + 2 + 5 + 6)) + 4
6 + ((3 + 9) * 4)
9 + ((4 + 4 + 5 * 6) * 8) * 2
(4 + 2) * 3 * 3 + 3
5 + 2 + (8 + 7 + 5)
6 + (7 + 4 * 9 * (6 + 8 * 8 + 3) * 8)
3 + 3 * 6 * 3 + (4 * (2 + 6 * 7) * 8)
7 * 5 + 8 * 7 + (9 + 4 * (7 + 6 + 2 * 7 * 9) + 9 + 9 * 4) + (3 + 6 + 5 * 6 * 4 + 9)
5 + (9 * (5 * 7 * 6 * 8) + (8 * 3) * 6 + (2 + 7 * 8)) * (9 + (5 + 7 * 2 * 6 + 9 + 7) + 4 + 8 * 3) * 7
(6 + 7 * 7 * 9 * 4) * 3 + (5 + 7) * 7 * 5 * 6
(9 + 6 * 5 + 3) * (2 + 6 + 6) + 4 * 6 + 3
5 + 8 + ((9 + 8) * (3 * 3 + 8 * 8) * 2 + 3) * 6 * 4
((3 + 9 * 2 * 8 * 7) * 7 * 8 + 3 + 7) * (8 * 9) * 5
5 + 3 + (4 * 4 * (8 * 8 + 4 * 4) + 4 + 5 * 6) * 4 + 2 + 7
5 + ((6 + 4 + 9 + 3) * 6 + (2 + 4 * 9 + 4 + 7 + 7)) + 4 + 9 * 8
2 + 4 * 7 * 5 * ((5 + 4 + 6) + 4 * 4)
8 * 6 + (4 + 8 * 5 * 6 + 4 * 9) * 9 + (2 + 8)
((8 + 3 * 2 * 6) * (7 + 8 * 8) * 9 + (2 + 5) + 4) * (2 * 9 * 5 + (9 * 4 * 2 + 7 + 8) * 2 + 2) * 6
4 * ((5 * 5 + 6 * 8 * 5 * 9) + 4 * (3 + 7 * 6 + 4) * 4) + 2 * ((9 + 8 + 9) + 9 * 2) * 8 + 4
2 * 8 * 6
(7 + (7 + 8 * 9) * 7 + (5 * 8 * 6 * 9 + 6 * 4)) + ((5 + 9 * 6 + 7) + 8 + 7 + (8 * 7 * 4 + 5 + 2) * 5 + (9 + 3 + 9 * 2 * 6 * 3)) * (4 + 5 * 4) + (8 * (5 * 5) * 9 * (3 * 3) + 5) * 4
(6 + 5 + 5 * 5) * 4 + ((8 + 6 + 8) + 5 + 5 + 7 * 9)
9 * (9 * 2 + 4 * 2 * 4)
4 + (6 + 2) * (4 + 2 + 2 * 5) + 2
5 * (6 + 8 + 5 * 2 * (7 * 2 + 2))
((7 + 5) * 2 * (3 * 5 * 9)) + 7 * (7 * 5 + 9 * 9 * (5 * 9))
7 + 5 * (2 * 3 * 4 + 9 * 7 * 9) + (2 + 8 + (3 * 6) * 7 * 7 * (5 * 8 * 7 + 8 * 6 * 9)) * 5
9 * 3 * ((9 + 7) * 7 * (2 * 9 + 5 * 9 * 7 + 8) + 6 * 6 + 6) * (9 + 7 + 6 + 9 * 7)
8 + 9 * 4 * ((7 * 3 + 6 + 6 * 7 + 9) + 8 * 5 * 2)
3 * (3 * (5 + 5 + 5 * 2))
4 * 4 * 8 * 6 + (3 + (5 + 5 + 5 + 6 * 6) * 8 * 9 + 9) + 3
(3 + 8) + 4
6 + 5 + 3 + ((2 + 5 + 6) + (9 + 5 * 4 * 9) + 6 * 7 + 5)
(6 * 8 * (2 + 2 + 8 + 8 + 2) + 7 + 6 * 3) + ((7 * 3 * 9) + 2 * 4 * 4 + 2) + 5
6 * ((6 * 8 + 3 * 9) + 2 * 9) + 3 * (6 + (3 * 7 * 4) + 4 * (6 * 8 * 3 * 7 + 3 * 8) * 6 * 7) * 6
5 * 8 * (7 * 7 + 4 * 7) + (2 * 4 + (3 * 2 * 4 * 9) + 4 + 2)
(3 * 8) + 3 + 6 + (3 * 3 + 9 * 6 * 8) + 3 + 5
((6 + 5 + 7 + 3) * 7 + 9 * 5) + 9 + 2
3 + 2 * ((9 + 9) * 8 + 5 * (6 + 4 * 6) + 4 + 7) + 9 + 4 * 4
5 * 6 + (5 + 6 * 8 * 5 * (8 * 2 + 7) * (7 * 6 * 2 * 2 * 7 * 6))
8 + 4 + 3 + (4 * 7) * 2 * (2 + (3 + 8 + 8 * 2 * 9))
7 + 5 + 3 + ((9 * 9 + 2 * 8 * 6 + 5) * 2) + (3 * 7 + 6 * 4 * (8 * 6 + 8 + 8 + 7 * 3) + (9 + 3 * 8 * 6 + 4)) * (9 * 6 * 9 * 6)
5 + (2 + (6 * 3 * 3 + 9 * 8 * 8) + 3) + 9 * 6 + 6
6 + (2 + (3 * 5 + 2 + 4)) + 7 * 9 * (8 * 9 * 6)
((4 * 4) + 2 + 7 + (5 + 3 * 2 + 9 + 8 + 4) * 5) * (2 * (6 + 8 * 9 + 7) * 9 + (7 + 5 * 7) * 8) + 6 * 3 * 3
(7 * 5) * (5 + 3 + 5)
(2 * 6 * 9 + 9 + 6) + 9 * 8
6 + (9 + 6 + 7 * 7 * 3 + (9 + 8 + 2 * 3 + 4 + 7)) + 9 * ((2 * 5 + 4 * 6 * 6 * 7) + (6 + 5 * 4 * 5 * 8) + 9) + ((5 + 7) * 5 * 4)
(7 * 2 + 7 * 8 * (7 + 6)) * 2 * 2 * 2
(6 + 5 * 2 + 3 + 5) * 8 + 4 + 2
3 + 4 + 3 + 7 * ((2 * 8 * 8 * 3 + 4 + 4) * 8 * 2 * 9 + 4 + 9)
7 + (5 * (7 * 9 + 7 + 2 * 7) + 9 + 3 + 5 + (8 * 9))
8 * (2 * (8 + 4 * 3) + 7 * 7 + 5 + 8) + 3 + 6 * 8
(8 + 3 * 5) * 6 + 2
3 + 8 * ((3 * 8) + 8 + 5)
5 * 9 * 5 + ((5 + 6 + 7) + 4) + ((5 + 8 + 3 + 4 + 3) * 8 * 3)
7 * (6 + 4 + (7 + 4 + 4 + 6) * 3 * 3 + 2) + 7 + 8 + 4 + 3
2 + ((3 + 8) + 2 + 9 + 4 * 3)
4 + 9 * 3 + 9
(7 + (7 * 4 + 7) * (6 * 6 * 9 * 4) * (2 + 5 + 8 + 3)) * 3
(8 + 2 + 4 * 7 * 3) + 2 * ((6 + 2 + 2) + 7 * 8 * 8 * 5 * 8) + 7 + (5 + 8 + 2 * (9 * 9) + 3)
8 * 2 + 6 * ((5 * 3) * 6 * 2 * (7 * 2 + 4 * 4 + 9)) + 4
4 * 3 * (8 + 7 * (2 + 4 + 7 + 4 + 3)) * 9
7 + (6 + (6 + 2) + 8 + 9 + 3) * (3 + 9 * (3 + 5 + 6 + 7 + 6) + (8 * 8 * 7) + 5 + 8) * 9 * 2 * 8
3 + 6 * (3 * 5) * (9 + 4 * 3) + (5 * 6 + 3) * 6
6 + (5 * 6 * 7 * (2 * 4) * 9) * 7 * 7 * 6 * ((2 * 6 * 2 + 3) + 7 + 9 * 2 + 6 * 6)
(4 * 4 * 2) * (4 + 3)
7 * 8 * 9 * 6 * 2 + (8 * (8 + 6 + 6) + 3 + 5 + 2)
(9 + 2 + (8 * 9 * 5 + 7) + 3) * 9 * (7 + 8 + 3 * 4 + 5) + 8 * (5 * 5 + (4 + 5 + 5 * 3 + 8) * 4) + (7 + 3 + 7 + 4)
8 * (2 * 2 + (8 + 7 * 9 * 2) * (8 * 2 + 5 + 4) * 9 + 8) * 6 + 3 + (4 * 3) * 7
(5 + 7 + 9 * 6 + 4) * 9
(3 + (5 + 3 + 4) * 3) + 3 + 8 * ((5 * 8 * 6) + (8 + 6 * 4 * 4 + 8) * 8 + 2 * 4 + 7) + (2 * (5 * 5 * 3) * 2 * 9)
(8 + 2 * 8) + 9 * 8 + 7
4 * (2 * 3 + 8 + 3 * 5) * ((3 * 3 * 8 + 7) * (7 + 5 + 8) * 6) + (9 * 5) + 8
7 * (6 * 3) + (7 + 8 + (2 + 5) * 3) + 7 + 6
5 + 9 * 2
9 * 2 + 8 * (3 * (6 * 7 * 7) * 8) + 9
((2 + 8 + 7 + 6) + 4 * (6 * 5 + 7 * 4 + 5) * 8) * 9 + (7 + 2 * 5 * 5 * 3 * 3) + 3
6 + 3 * 3 + (8 + 4)
(8 + (8 * 2) + 4) * 5 * 4 + 5
(5 * 5 * 5 * 8 + 8 + 7) + (6 + 9) + 4 * 3 + 4
(3 + (2 * 9 + 2 + 9 + 4) + 2 + (5 + 3 + 2)) * 5 * 3 + 5
6 + 3 * ((8 + 7 * 8 * 7 + 9) + 3 + 3 + 9 * (7 + 5 + 9) * 9) + 5 + (7 * 8 + (5 * 7 + 5 * 9 + 4))
(4 * 9) + 7 * 9
(3 + 7) + 2 + (5 * 2 * 4 * 6 + 5)
5 * 4 * 2 * ((9 * 4 * 7) * 2 * 3) + 3 + 6
(5 + 2 * 7 * 7) * (4 * (3 * 8 * 2) * 3 * (2 + 3 * 8)) + 4 * 6 + 8 + 7
7 * 4 + 2 + (6 + 2 * 5 * 2 + 6 * 9) + 7 * 2
8 + (4 * 4 * 7 * 9 + 5 + 6)
(4 + 7 + 8 * 6) + (3 * 5 + 4) + 4
6 + 2 + 3 + 2 * 3 + 9
6 + ((7 * 6 + 5) + 4 + 9 + (5 + 6 * 6 + 5 * 2 * 9) * (2 * 9)) + 9 + 5 * 2
8 + 3 * 4 * 3 * (3 * 9 * 2)
9 * (7 * 7 * 4) + 3 + (9 + (6 + 6 + 2 + 3 + 5 + 6) * (3 * 7 * 9 * 3) * 6 + 8 * 3)
5 * (9 * 2 + (6 + 2 * 5) + 5)
4 + 3 + 9 + 7 + 5 + (3 * (2 + 8 * 6) * 4 + 2)
3 * ((2 + 5 + 2 + 6) + 3 + 4 + (7 + 9) * (7 + 8 * 4 + 9) * 3) + 9 + (8 + 6)
8 + 4 * 4 + 7 + ((3 * 9 * 2 + 2 * 8) + (3 + 6 + 2 * 5 * 4 * 8) * 8 * 8)
4 + 4 * ((5 + 3 + 2 * 4 + 7 * 8) * (5 * 6 + 9) + 5 * 3)
7 * 8
3 + (5 * 3 + (5 * 9) * 2 + 4) + 2 + 3
6 + 8 + 3 + (2 * (6 + 5 + 3 * 8 + 9) * 3 * 9) * (6 * 8)
2 * 9 * 4 * (6 * 2 * 6 * 3 * 8) * 4
6 + (3 * 4 * 7 + 5 * 5 + 2) * 4 + (6 * 5 * (6 + 2 + 9 + 6 * 8) * 2 + 5) + 4
6 * ((6 + 4) + 9 * 5 * 3 + (9 * 5)) * (4 + 2 * 7 * (4 * 9 * 3 * 8) * 4) + 3
7 * 2 + 6 * (3 * 6 + 6 * 5) * 7 * 7
3 + 8 * (3 * 2 + (9 * 3 + 8 * 7 + 8 + 4) + 6 + 3 + 5) + (6 + 8 + 7 * 3) * 9 * (4 * 3 * 6 + (9 + 9 * 9 * 2 * 6) + 4 + (6 * 4 + 5 + 9 + 9 + 7))
4 * 8 * (8 * 5 + 3 * 6 * 7 + 9) * 9
8 + 4 * 9 * 4 * 8
(3 * 7 + (5 * 2 + 7 + 5 * 6) + 9 * 5) * 6 + 6 + 2 * 5
(8 + (4 * 8 * 8) * 7) + 4 * 8 * ((4 * 4 * 5) + 8 + 8 + 2) + ((6 * 8) * 8 * 8 * 8 * 2) + 8
4 * (6 + 2 + 6 + 9 + 7) * 4 + 7 + 9
5 + (2 + 6 * 8 + 5) + (6 + (8 + 9 + 5) * 4 * 3) * 4 * 5
9 * 6 + 5 * (4 + 2 + (7 + 9 * 6)) + 5 + 6
8 + ((8 + 3 + 5 + 3 * 8) + 5 + 3) * 4 * 3
2 + (3 + 5 * (8 * 2) + 8 + 5) * 8 + 4 + (8 + (9 + 8) + 8 * 9 * 7) + 9
7 * (4 * 2 + 5) * 8
3 + 5 + 7 + (6 + 2 * 7 * 7 * 5) + ((3 * 4 + 8 * 5 + 6) + 4 * 5)
(9 + 6) * 7 * (9 + 3 + 4 * (8 + 8 * 3 + 6 + 8 * 2))
2 + (6 + 5 + (7 * 5 + 5 + 9 + 7 + 6) + 2 + 2 * (8 + 8))
4 * 9 + 2 * 9 + 9 + 8
((2 * 7 * 5) * 8) * 8 + 4 + 8 * 4
7 * 5 + (8 + 9) * 3 * 6
8 + ((9 + 7 + 8) + (8 * 5) + 2 + 7 + 3 + 3) + 3
6 * (5 * (7 + 6 + 5 * 9 + 8 * 5) * 6) + 7 * 9 * ((2 + 5 * 8 * 2 + 3) * 3 + 7 + 5 * 3) * 2
3 * (3 + 5 + 4) * 6 * 9
6 + ((8 * 5 + 3 + 9) * (5 + 2 * 4 * 5 + 5) + (2 * 3) + 4 + 7) + 7 + 8 * 6 + 4
((9 * 2 + 8 * 9) * 4 + 5 * 3 * (8 * 2 * 7)) + (7 + 4 * 2 + 6) * 9
(6 * 5 + 7 * (5 * 5 + 4 + 7 * 7 + 6) + 2) * ((4 + 6 * 5 * 6 + 7) + 9 + 5) * 7 * (4 + 7 * 6 + 5) * 5
8 * (3 * 6 + 5) + 9 * (9 + 6 * (4 * 6) + (7 + 8 + 5 + 3)) * (3 * 7 * 8 * 2) + 9
8 * 3 + (5 + 3) + 8 * 4 + (4 + (7 + 9 * 5 * 3) * 6 + (7 + 5 + 4) + 8 * 8)
2 * 3 + ((8 * 8 * 8 + 4) * 7 * 7) * 5 * 8
(6 * (7 + 3 + 8 * 5 + 2) + 2 * 4 + 5 * 5) * 2 * (8 + 3 + 3 + (6 + 4) * (9 * 3)) + 6 * 9
8 * (7 * 6 * 8 * (3 * 6 + 4 + 6 + 2 + 2) * 6) + 8 * 8 * 6
(7 * 5 * 5) * 7 * ((3 + 6 + 3 + 4 + 2 * 2) * 8 * (7 + 9 * 9 * 9))
((5 + 7 * 9 * 2 + 7) + 7 + 5 * 6 + 3) * 6 * 2 * 3
5 + (5 * (9 + 9) + (7 * 7 * 4 + 2) * 3)
((8 * 8 + 2 + 8 * 9) * 5) + (2 + (9 * 6 + 9 + 7) * 7)
3 + ((7 * 4 * 4 * 5) * (8 + 3 * 4 + 4 * 9) * 5 * 3 + 7) + 4 * 2 * 7 * (3 * 6 * 6)
((9 * 7 + 6 + 7 * 9 + 2) * 4 + 3 + 4 + 4 * 6) * 8 + (4 + (8 + 2 * 6 * 4 * 3)) + (5 + 2 * 6 * (4 * 5 + 4 + 2)) + 4 * (6 + 3 * (2 * 2 * 6 * 3) + 8 + 4 * 5)
(5 * (7 + 9 + 5) + 6) * 5 * (6 + 7 * 9 + 6) * 9 * 5 + 9
(4 * 4 * 6 * (5 * 8 + 3 + 8) + 5) + (9 * (3 * 9 + 3 * 9) + 5 + 8 + 2 * 3) * 3 * 2 + 8 * 5
7 + 5 * (3 + 8 + 6 * 9 + 5 * 8) * ((9 * 9 * 4 * 9 + 3) * (6 + 9 * 5) * 8 + 4 + 6)
7 + 8 * ((3 + 7 * 5) + 3 * 2 + 2 * 3 + 7) * 9 + 8 * 7
3 + 4 + (5 * (9 + 9 + 3 * 6 * 4 * 3) * 4) * (4 * (7 * 6 * 3 + 5 * 2 * 9) + (7 * 3 + 5))
3 * (2 + 8 + 2) * 6 * (4 * 4)
2 + 2 + 3 * 5 + (3 + (6 + 2) + 9 + 7) * (3 * 3 + 5 + 8 + 7)
4 + 5 + 6 * 6 * 6 + (3 * 2 * 3 * (2 * 6 + 9) + 6 + (5 + 2 + 9 * 6))
((4 + 9) + 2) * 7 + 3 * 4
2 + 6 * ((2 * 7 * 8 * 3 + 8 + 4) + 9 + 6 + 9 * 2 + 9) + 7 * 6
9 * 7 + ((2 + 5 * 2 * 7 * 8) + 6 * 2 + 3 + 4) * 5 * (5 + 2 * 7) * 8
4 * (8 + 5 * 6 * 8) + 2 + 3 * 7
(2 * 2) * 9 * 6 * 3 + 9
9 + 2 * (7 + 5 + 7 + (3 + 7 * 2 * 2 + 3) * 3)
(2 + 5 * 9) * (4 * (9 + 2 * 4 + 3 + 2) * 2 + 8 + (6 * 7 + 9 + 5)) * 3 + 8
(6 * (6 + 7 * 7) + 7 * 6 * 7) + ((7 * 6 * 4) * 6 * (8 + 8 + 5 * 7 * 8 + 8) + (4 + 5 * 5 + 5 + 3 * 9) + 8)
9 * (6 * 9 * (8 * 2 * 6) + 4 * 4)
(5 + 5 * 5 + 8) * 6 + 5 * 2
4 + ((8 + 7) + 5) + 6
9 + 6 * (7 + 4 + (9 + 4 + 2 + 8)) + (8 * 5)
(3 * 8 + (6 + 9 * 8 + 6)) + 3 + 4 * 8 + 5 * 3
3 + 6 * (7 * 8 * 6 * (5 + 8 + 6) + 5 * 4) * ((8 + 9 + 6 * 8) * 9 + 6 * 6 + 2 * 2) * 4
7 * (6 * (5 * 5 + 9 * 7 + 5 + 9) + 8 + 9 + 2 * (6 * 7 * 7)) * 3
4 + 2 * (5 * (3 * 8 * 3) * (5 + 9 + 8 * 9) + 9 + 7) * 7 + 5
6 * 5 * (5 * 3 * 9 * 4 * 2) * 9 + 7 * 4
3 * (4 * (7 + 2) + 4 + 6 + (3 + 6 + 6 * 5)) + 5
7 * 7 * (5 + 9 + 9) * (8 * 6 * 5 + 4 * 2)
6 + (9 * 4) * ((4 * 2 * 4 * 4 * 6 * 7) + 9 + (2 * 9 + 9 + 9)) + 2 + 4 + 4
9 * (6 * 5 + 9 * 2 * 4) + 7 + 8 * 8 + 9
(6 + 7 + 8 * 2 * 4) * 5 + 2 + 2 * 5 + 6
8 * 7 * (9 * 2 * 4 + 3 + 4 * 4) + (8 * 8 + 9 + 6) + ((7 * 7 + 2 * 9) * (2 * 8 + 2) * 6 * 6) + 8";
        public override int Day => 19;
    }
}
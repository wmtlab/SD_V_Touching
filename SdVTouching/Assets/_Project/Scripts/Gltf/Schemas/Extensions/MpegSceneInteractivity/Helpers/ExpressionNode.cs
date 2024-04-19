using System;
using System.Collections.Generic;

namespace SdVTouching.Gltf
{
    public interface IExpressionNode
    {
        bool Evaluate();

        public static class Factory
        {
            public static IExpressionNode Create(string expression, Dictionary<int, Func<bool>> methods)
            {
                expression = expression.Replace(" ", "");
                Stack<IExpressionNode> nodeStack = new Stack<IExpressionNode>();
                Stack<char> operatorStack = new Stack<char>();
                int index = 0;
                while (index < expression.Length)
                {
                    char c = expression[index];
                    if (c == '#')
                    {
                        int conditionId = 0;
                        index++;
                        while (index < expression.Length && char.IsDigit(expression[index]))
                        {
                            conditionId = conditionId * 10 + (expression[index] - '0');
                            index++;
                        }
                        if (methods.TryGetValue(conditionId, out var method))
                        {
                            nodeStack.Push(new AtomicNode(conditionId, method));
                        }
                        else
                        {
                            throw new ArgumentException($"Method for condition {conditionId} is not set.");
                        }
                    }
                    else if (c == '(')
                    {
                        operatorStack.Push(c);
                        index++;
                    }
                    else if (c == ')')
                    {
                        while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                        {
                            char op = operatorStack.Pop();
                            if (op == '~')
                            {
                                IExpressionNode right = nodeStack.Pop();
                                nodeStack.Push(new OperatorNode(right, null, op));
                            }
                            else
                            {
                                IExpressionNode right = nodeStack.Pop();
                                IExpressionNode left = nodeStack.Pop();
                                nodeStack.Push(new OperatorNode(left, right, op));
                            }
                        }
                        operatorStack.Pop();
                        index++;
                    }
                    else if (c == '~' || c == '&' || c == '|')
                    {
                        while (operatorStack.Count > 0 && GetPriority(operatorStack.Peek()) >= GetPriority(c))
                        {
                            char op = operatorStack.Pop();
                            if (op == '~')
                            {
                                IExpressionNode right = nodeStack.Pop();
                                nodeStack.Push(new OperatorNode(right, null, op));
                            }
                            else
                            {
                                IExpressionNode right = nodeStack.Pop();
                                IExpressionNode left = nodeStack.Pop();
                                nodeStack.Push(new OperatorNode(left, right, op));
                            }
                        }
                        operatorStack.Push(c);
                        index++;
                    }

                }

                while (operatorStack.Count > 0)
                {
                    char op = operatorStack.Pop();
                    if (op == '~')
                    {
                        IExpressionNode right = nodeStack.Pop();
                        nodeStack.Push(new OperatorNode(right, null, op));
                    }
                    else
                    {
                        IExpressionNode right = nodeStack.Pop();
                        IExpressionNode left = nodeStack.Pop();
                        nodeStack.Push(new OperatorNode(left, right, op));
                    }
                }

                return nodeStack.Pop();
            }

            private static int GetPriority(char v)
            {
                return v switch
                {
                    '~' => 3,
                    '&' => 2,
                    '|' => 1,
                    _ => 0,
                };
            }
        }

    }

    public class AtomicNode : IExpressionNode
    {
        public int ConditionId { get; }
        public Func<bool> Method { get; set; }

        public AtomicNode(int conditionId, Func<bool> method)
        {
            ConditionId = conditionId;
            Method = method;
        }

        public bool Evaluate()
        {
            return Method?.Invoke() ?? throw new ArgumentException($"Method for condition {ConditionId} is not set.");
        }
    }

    public class OperatorNode : IExpressionNode
    {
        public IExpressionNode Left { get; }
        public IExpressionNode Right { get; }
        public char Operator { get; }

        public OperatorNode(IExpressionNode left, IExpressionNode right, char @operator)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }

        public bool Evaluate()
        {
            if (Operator == '&')
            {
                return Left.Evaluate() && Right.Evaluate();
            }
            else if (Operator == '|')
            {
                return Left.Evaluate() || Right.Evaluate();
            }
            else if (Operator == '~')
            {
                return !Left.Evaluate();
            }
            else
            {
                throw new ArgumentException($"Invalid operator: {Operator}");
            }
        }
    }
}
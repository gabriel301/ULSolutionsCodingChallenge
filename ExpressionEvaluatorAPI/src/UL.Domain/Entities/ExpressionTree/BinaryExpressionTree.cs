using System.Text.RegularExpressions;
using UL.Domain.Entities.Abstraction;
using UL.Domain.Enumeration;
using UL.Domain.Events.ExpressionTree.Created;
using UL.Domain.Events.ExpressionTree.Evaluated;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ExpressionTree;
using UL.Domain.Resources;
using UL.Domain.Services.Abstraction;
using UL.Shared.RegexPatterns;
using UL.Shared.Resources;

namespace UL.Domain.Entities.ExpressionTree;
public class BinaryExpressionTree : Entity, IExpressionTree, IDisposable
{

    private List<Node> _nodes = new List<Node>();
    private Node? _root;
    private bool disposed = false;
    private bool isEvaluated = false;
    public string Expression { get; private set; }
    public double Evaluation { get; private set; }


    private BinaryExpressionTree(string expression)
    {
        Validate(expression);
        Expression = expression;
        Build();

    }

    private void Build()
    {
        MatchCollection matches;

        Stack<string> expressionStack = new Stack<string>();
        Stack<Node> nodeStack = new Stack<Node>();

        matches = Regex.Matches(Expression, EvaluationPatterns.MATCH_ALL_SUM_OR_SUBTRACTION);

        if (matches.Count == 0)
        {
            matches = Regex.Matches(Expression, EvaluationPatterns.MATCH_ALL_MULTIPLICATION_OR_DIVISION);
        }

        var rootNode = matches.LastOrDefault();


        NodeTypeEnum nodeType = NodeTypeEnum.Operator;
        _root = new Node(rootNode!.Value, nodeType);
        _nodes.Add(_root);
        nodeStack.Push(_root);

        var expressionToEvaluate = Expression.Substring(0, rootNode.Index);


        var rigthSubstring = Expression.Substring(rootNode.Index + 1, Expression.Length - expressionToEvaluate.Length - 1);

        if (!rigthSubstring.Equals(string.Empty))
        {
            expressionStack.Push(rigthSubstring);
        }

        while (expressionStack.Count > 0)
        {

            if (expressionToEvaluate.Equals(string.Empty))
            {
                expressionToEvaluate = expressionStack.Pop();
            }
            nodeType = NodeTypeEnum.Operand;
            matches = Regex.Matches(expressionToEvaluate, EvaluationPatterns.CONTAINS_ONLY_DIGITS);

            if (matches.Count > 0)
            {
                rootNode = matches.LastOrDefault();
                Node node = new Node(rootNode!.Value, nodeType);
                _nodes.Add(node);
                var parentNode = nodeStack.Pop();
                node.SetParent(parentNode);
                if (!parentNode.HasLeftChild())
                {
                    parentNode.SetLeftChild(node);
                    nodeStack.Push(parentNode);
                }
                else if (!parentNode.HasRightChild())
                {
                    parentNode.SetRightChild(node);
                }

                expressionToEvaluate = string.Empty;
                continue;
            }

            if (matches.Count == 0)
            {
                matches = Regex.Matches(expressionToEvaluate, EvaluationPatterns.MATCH_ALL_SUM_OR_SUBTRACTION);
            }

            if (matches.Count == 0)
            {
                matches = Regex.Matches(expressionToEvaluate, EvaluationPatterns.MATCH_ALL_MULTIPLICATION_OR_DIVISION);
            }

            nodeType = NodeTypeEnum.Operator;

            rootNode = matches.LastOrDefault();
            Node operatorNode = new Node(rootNode!.Value, nodeType);
            _nodes.Add(operatorNode);
            var parentOperatorNode = nodeStack.Pop();

            operatorNode.SetParent(parentOperatorNode);
            if (!parentOperatorNode.HasLeftChild())
            {
                parentOperatorNode.SetLeftChild(operatorNode);
                nodeStack.Push(parentOperatorNode);
            }
            else if (!parentOperatorNode.HasRightChild())
            {
                parentOperatorNode.SetRightChild(operatorNode);
            }

            nodeStack.Push(operatorNode);

            var originalExpression = expressionToEvaluate;

            expressionToEvaluate = expressionToEvaluate.Substring(0, rootNode.Index);

            rigthSubstring = originalExpression.Substring(rootNode.Index + 1, originalExpression.Length - expressionToEvaluate.Length - 1);

            if (!rigthSubstring.Equals(string.Empty))
            {
                expressionStack.Push(rigthSubstring);
            }

        }


    }

    public static BinaryExpressionTree Create(string expression)
    {

        BinaryExpressionTree tree = new BinaryExpressionTree(expression);

        ExpressionTreeCreatedEventData eventData = new ExpressionTreeCreatedEventData(tree.Expression);

        ExpressionTreeCreatedEvent domainEvent = new ExpressionTreeCreatedEvent(eventData);

        tree.AddDomainEvent(domainEvent);

        return tree;

    }

    public double Evaluate(IOperationService operationService)
    {
        if (!isEvaluated)
        {
            Evaluation = DFS(operationService);
            isEvaluated = true;
            ExpressionTreeEvaluatedEventData eventData = new ExpressionTreeEvaluatedEventData(Expression, Evaluation);

            ExpressionTreeEvaluatedEvent domainEvent = new ExpressionTreeEvaluatedEvent(eventData);

            AddDomainEvent(domainEvent);
        }

        return Evaluation;

    }

    private double DFS(IOperationService operationService)
    {
        Stack<double> operandStack = new Stack<double>();
        Stack<string> operatorStack = new Stack<string>();
        HashSet<Node> visitedNodes = new HashSet<Node>();

        Node? currentNode = _root;

        do
        {

            if (currentNode!.Type.Equals(NodeTypeEnum.Operator) && !visitedNodes.Contains(currentNode))
            {
                operatorStack.Push(currentNode.Value);
                visitedNodes.Add(currentNode);
            }
            else if (!visitedNodes.Contains(currentNode))
            {
                operandStack.Push(double.Parse(currentNode.Value));
                visitedNodes.Add(currentNode);
            }

            if (currentNode.Type.Equals(NodeTypeEnum.Operator) && visitedNodes.Contains(currentNode.LeftChild!) && visitedNodes.Contains(currentNode.RightChild!) && operandStack.Count > 1)
            {
                double operand2 = operandStack.Pop();
                double operand1 = operandStack.Pop();
                string arithmeticOperator = operatorStack.Pop();
                double operationResult = operationService.Calculate(arithmeticOperator, operand1, operand2);

                if (double.IsInfinity(operationResult))
                {
                    throw new ExpressionTreeEvaluationException(Expression, ExpressionTreeEvaluationExceptionResource.Infinity_Value);
                }
                operandStack.Push(operationResult);
            }

            if (currentNode.LeftChild != null && !visitedNodes.Contains(currentNode.LeftChild))
            {
                currentNode = currentNode.LeftChild;
                continue;
            }

            if (currentNode.RightChild != null && !visitedNodes.Contains(currentNode.RightChild))
            {
                currentNode = currentNode.RightChild;
                continue;
            }

            if (currentNode.ParentNode != null)
            {
                currentNode = currentNode.ParentNode;
                continue;
            }
        } while (operandStack.Count > 1 || operatorStack.Count > 0);

        var result = operandStack.Pop();
        return result;
    }

    private void Validate(string expressionString)
    {
        if (expressionString is null || expressionString.Trim().Equals(string.Empty))
        {
            throw new DomainValidationException(ValidationResources.Null_Or_Empty_String);
        }

        if (Regex.Match(expressionString, ValidationPatterns.INVALID_CHARACTERS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Invalid_Characters);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONSECUTIVE_OPERATORS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Consecutive_Operators);
        }

        if (Regex.Match(expressionString, ValidationPatterns.STARTS_OR_ENDS_WITH_NON_DIGITS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Starts_Or_Ends_With_Operator);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONTAINS_ONLY_DIGITS).Success)
        {
            throw new DomainValidationException(expressionString, ValidationResources.Contains_Only_Digits);
        }
    }


    public int GetNodeCount()
    {
        return _nodes.Count;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                foreach (var item in _nodes)
                {
                    item.Dispose();
                }
                _nodes.Clear();
            }
            disposed = true;
        }

    }

    //Destructor
    ~BinaryExpressionTree()
    {
        Dispose(false);
    }
}

using UL.Domain.Entities.Abstraction;
using UL.Domain.Enumeration;
using UL.Domain.Events.ExpressionTree.Created;
using UL.Domain.Events.ExpressionTree.Evaluated;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ExpressionTree;
using UL.Domain.Resources;
using UL.Shared.RegexPatterns;
using UL.Resources.Validation;
using System.Text.RegularExpressions;
using UL.Domain.Entities.Expression;

namespace UL.Entities.Expression;
public class ExpressionTree : Entity, IExpressionTree, IDisposable
{

    private List<Node> _nodes = new List<Node>();
    private Node _root;
    private bool disposed = false;
    private bool isEvaluated = false;
    public string Expression { get; private set; }
    public double Evaluation { get; private set; }


    private ExpressionTree(string expression)
    {
        Validate(expression);
        Expression = expression;
        Build();

    }

    private void Build()
    {
        MatchCollection matches;

        Stack<String> expressionStack = new Stack<String>();
        Stack<Node> nodeStack = new Stack<Node>();

        matches = Regex.Matches(this.Expression, TreeBuildingPatterns.CONTAINS_ONLY_SUM_OR_SUBTRACTION);

        if (matches.Count == 0)
        {
            matches = Regex.Matches(this.Expression, TreeBuildingPatterns.CONTAINS_ONLY_MULTIPLICATION_OR_DIVISION);
        }

        var rootNode = matches.LastOrDefault();


        NodeTypeEnum nodeType = NodeTypeEnum.Operator;
        this._root = new Node(rootNode!.Value, nodeType);
        this._nodes.Add(this._root);
        nodeStack.Push(this._root);

        var expressionToEvaluate = this.Expression.Substring(0, rootNode.Index);


        var rigthSubstring = this.Expression.Substring(rootNode.Index + 1, this.Expression.Length - expressionToEvaluate.Length - 1);

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
            matches = Regex.Matches(expressionToEvaluate, TreeBuildingPatterns.CONTAINS_ONLY_DIGITS);

            if (matches.Count > 0)
            {
                rootNode = matches.LastOrDefault();
                Node node = new Node(rootNode!.Value, nodeType);
                this._nodes.Add(node);
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
                matches = Regex.Matches(expressionToEvaluate, TreeBuildingPatterns.CONTAINS_ONLY_SUM_OR_SUBTRACTION);
            }

            if (matches.Count == 0)
            {
                matches = Regex.Matches(expressionToEvaluate, TreeBuildingPatterns.CONTAINS_ONLY_MULTIPLICATION_OR_DIVISION);
            }

            nodeType = NodeTypeEnum.Operator;

            rootNode = matches.LastOrDefault();
            Node operatorNode = new Node(rootNode!.Value, nodeType);
            this._nodes.Add(operatorNode);
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

    public static ExpressionTree Create(string expression)
    {

        ExpressionTree tree = new ExpressionTree(expression);

        ExpressionTreeCreatedEventData eventData = new ExpressionTreeCreatedEventData(tree.Expression);

        ExpressionTreeCreatedEvent domainEvent = new ExpressionTreeCreatedEvent(eventData);

        tree.AddDomainEvent(domainEvent);

        return tree;

    }

    public double Evaluate()
    {
        if (!isEvaluated)
        {
            Evaluation = DFS();
            isEvaluated = true;
            ExpressionTreeEvaluatedEventData eventData = new ExpressionTreeEvaluatedEventData(this.Expression, this.Evaluation);

            ExpressionTreeEvaluatedEvent domainEvent = new ExpressionTreeEvaluatedEvent(eventData);

            this.AddDomainEvent(domainEvent);
        }

        return Evaluation;

    }

    private double DFS()
    {
        Stack<double> operandStack = new Stack<double>();
        Stack<string> operatorStack = new Stack<string>();
        HashSet<Node> visitedNodes = new HashSet<Node>();

        Node currentNode = _root;

        do
        {

            if (currentNode.Type.Equals(NodeTypeEnum.Operator) && !visitedNodes.Contains(currentNode))
            {
                operatorStack.Push(currentNode.Value);
                visitedNodes.Add(currentNode);
            }
            else if (!visitedNodes.Contains(currentNode))
            {
                operandStack.Push(Double.Parse(currentNode.Value));
                visitedNodes.Add(currentNode);
            }

            if (currentNode.Type.Equals(NodeTypeEnum.Operator) && visitedNodes.Contains(currentNode.LeftChild) && visitedNodes.Contains(currentNode.RightChild) && operandStack.Count > 1)
            {
                double operand2 = operandStack.Pop();
                double operand1 = operandStack.Pop();
                string arithmeticOperator = operatorStack.Pop();
                double operatonResult = PerformOperation(arithmeticOperator, operand1, operand2);

                if (double.IsInfinity(operatonResult))
                {
                    throw new ExpressionTreeEvaluationException(this.Expression, ExpressionTreeEvaluationExpectionResource.Infinity_Value);
                }
                operandStack.Push(operatonResult);
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

    private double PerformOperation(string arithmeticOperator, double operand1, double operand2) => arithmeticOperator switch
    {
        "+" => operand1 + operand2,
        "-" => operand1 - operand2,
        "*" => operand1 * operand2,
        "/" => operand1 / operand2,
        _ => 0.0d,
    };

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
    ~ExpressionTree()
    {
        Dispose(false);
    }
}

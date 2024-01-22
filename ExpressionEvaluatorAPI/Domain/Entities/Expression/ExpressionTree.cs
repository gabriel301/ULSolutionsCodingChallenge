using Domain.Enumeration;
using Domain.Exceptions;
using Domain.Interfaces;
using Shared.RegexPatterns;
using Shared.ReShared.Resources.Validationsoures.Validation;
using System.Text.RegularExpressions;

namespace Domain.Entities.Expression;
public class ExpressionTree : IExpressionTree, IDisposable
{

    private List<Node> _nodes = new List<Node>();
    private Node _root;
    private bool disposed = false;
    public string Expression { get; private set; }


    private ExpressionTree(string expression)
    {
        Validate(expression);
        Expression = expression;
        Build();

    }

    private void Build()
    {
        //Setting Root
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
                rootNode = matches.LastOrDefault(); //Se tiver mais em digito, pega todos?
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
            parentOperatorNode.SetLeftChild(operatorNode);
            _nodes.Add(operatorNode);
            nodeStack.Push(parentOperatorNode);
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

        return tree;

    }


    private void Validate(string expressionString)
    {
        if (expressionString is null || expressionString.Trim().Equals(string.Empty))
        {
            throw new DomainException(ValidationResources.Null_Or_Empty_String);
        }

        if (Regex.Match(expressionString, ValidationPatterns.INVALID_CHARACTERS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Invalid_Characters);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONSECUTIVE_OPERATORS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Consecutive_Operators);
        }

        if (Regex.Match(expressionString, ValidationPatterns.STARTS_OR_ENDS_WITH_NON_DIGITS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Starts_Or_Ends_With_Operator);
        }

        if (Regex.Match(expressionString, ValidationPatterns.CONTAINS_ONLY_DIGITS).Success)
        {
            throw new DomainException(expressionString, ValidationResources.Contains_Only_Digits);
        }
    }

    public double Evaluate()
    {
        throw new NotImplementedException();
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
                // Free any other managed objects here
                foreach (var item in _nodes)
                {
                    item.Clear();
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

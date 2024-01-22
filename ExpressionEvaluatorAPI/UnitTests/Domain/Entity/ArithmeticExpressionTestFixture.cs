using Domain.Interfaces;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Domain.Entity;
public class ArithmeticExpressionTestFixture
{

    public IExpressionTree GetExpressionTreeSubstitute() 
    {
        IExpressionTree expressionTree = Substitute.For<IExpressionTree>();
        expressionTree.Evaluate().Returns(0);
        return expressionTree;
    }
  
}

[CollectionDefinition(nameof(ArithmeticExpressionTestFixture))]
public class ExpressionTestFixtureCollection : ICollectionFixture<ArithmeticExpressionTestFixture>
{
}

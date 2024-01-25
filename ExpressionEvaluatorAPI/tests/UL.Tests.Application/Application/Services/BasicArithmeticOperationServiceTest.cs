using FluentAssertions;
using UL.Application.DomainServices;
using UL.Domain.Services.Abstraction;

namespace UL.Tests.Application.Application.Services;
public class BasicArithmeticOperationServiceTest
{
    private readonly IOperationService _operationService;

    public BasicArithmeticOperationServiceTest()
    {
        _operationService = new BasicArithmeticOperationService();
    }

    [Theory(DisplayName = nameof(Calculate))]
    [Trait("Application", "BasicArithmeticOpeationService")]
    [InlineData("+", 2.0, 1.0, 3.0)]
    [InlineData("+", 1.0, 2.0, 3.0)]
    [InlineData("-", 2.0, 1.0, 1.0)]
    [InlineData("-", 1.0, 2.0, -1.0)]
    [InlineData("*", 2.0, 1.0, 2.0)]
    [InlineData("*", 1.0, 2.0, 2.0)]
    [InlineData("/", 2.0, 1.0, 2.0)]
    [InlineData("/", 1.0, 2.0, 0.5)]
    [InlineData("/", 1.0, 0.0, double.PositiveInfinity)]

    public void Calculate(string mathOperator, double operaand1, double operand2, double expectedResult)
    {

        var result = _operationService.Calculate(mathOperator, operaand1, operand2);

        result.Should().Be(expectedResult);

    }
}

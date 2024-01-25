using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using UL.Tests.WebApi.Infrastructure;
using UL.WebApi;

namespace UL.Tests.WebApi.Controllers.Expression;
public class EvaluateArithmeticExpressionTest : CustomWebApplicationFactory
{
    #region Setup
    private readonly string _apiUrl;

    public EvaluateArithmeticExpressionTest(WebApplicationFactory<Program> factory) : base(factory)
    {
        _apiUrl = "api/v2/expression";
    }
    #endregion

    #region Theory Data
    public static TheoryData<string> InfinityValues =>
        new TheoryData<string>
        {
            "1/0",
            $"{double.MaxValue.ToString("0.#")}+{double.MaxValue.ToString("0.#")}"

        };

    #endregion

    #region Tests
    [Theory(DisplayName = nameof(IsNullOrEmptyString))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] //Spaces
    [InlineData("       ")] //Tabs
    public async Task IsNullOrEmptyString(string expression)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory(DisplayName = nameof(ContainsInvalidCharacters))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")] //Spaces
    [InlineData("       ")] //Tabs
    public async Task ContainsInvalidCharacters(string expression)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory(DisplayName = nameof(ContainsOnlyDigits))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("123")]
    [InlineData("12345")]
    public async Task ContainsOnlyDigits(string expression)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory(DisplayName = nameof(StartsOrEndsWithOperators))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [InlineData("+1+2")]
    [InlineData("-1+2-")]
    [InlineData("/1+1")]
    [InlineData("*1+2")]
    [InlineData("1+2-")]
    [InlineData("1+2*")]
    [InlineData("1-2/")]
    [InlineData("+1-2-")]
    [InlineData("-")]
    [InlineData("+")]
    [InlineData("/")]
    [InlineData("*")]
    public async Task StartsOrEndsWithOperators(string expression)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory(DisplayName = nameof(ContainsSequentialOperators))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [InlineData("++1+2")]
    [InlineData("--1+2")]
    [InlineData("//1+1")]
    [InlineData("**1+2")]
    [InlineData("1+-2")]
    [InlineData("1-*2")]
    [InlineData("1-*+/-2")]
    [InlineData("1-2+-")]
    public async Task ContainsSequentialOperators(string expression)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory(DisplayName = nameof(EvaluateExpressionInfinity))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [MemberData(nameof(InfinityValues))]
    public async Task EvaluateExpressionInfinity(string expression)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory(DisplayName = nameof(EvaluateExpression))]
    [Trait("WebApi", "EvaluateArithmeticExpression")]
    [InlineData("4+5*2", 14.0)]
    [InlineData("4+5/2", 6.5)]
    [InlineData("4+5/2-1", 5.5)]
    [InlineData("1+1+1", 3.0)]
    [InlineData("1+2/3+4*5", 21.6666666)]
    [InlineData("1/2+3*4", 12.5)]
    [InlineData("1/2+3+4*5", 23.5)]
    [InlineData("1/2/6/34554+243-908/3453-1344*465767+134565/23454*1245-13243-344565+566754/54365*2342-324344*4543+45345-56564", -2099822865.897338533165544)]
    [InlineData("1231/6546+657657-442*7867/234+65767-242+657-2342*675/342", 704356.93074383)]
    public async Task EvaluateExpression(string expression, double expectedResult)
    {
        var response = await HttpClient.PostAsJsonAsync(_apiUrl, expression);
        var result = Double.Parse(await response.Content.ReadAsStringAsync(), CultureInfo.InvariantCulture);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeApproximately(expectedResult, 0.0001);
    }

    #endregion
}

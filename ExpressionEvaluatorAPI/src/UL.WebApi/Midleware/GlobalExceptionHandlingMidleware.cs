using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UL.Application.Exceptions;
using UL.Domain.Exceptions;
using UL.Domain.Exceptions.ArithmeticExpression;
using UL.Domain.Exceptions.ExpressionTree;

namespace UL.WebApi.Midleware;

public class GlobalExceptionHandlingMidleware : IExceptionFilter
{

    private readonly ILogger<GlobalExceptionHandlingMidleware> _logger;

    public GlobalExceptionHandlingMidleware(ILogger<GlobalExceptionHandlingMidleware> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();

        var exception = context.Exception;

        if (exception is DomainValidationException)
        {
            details.Status = StatusCodes.Status400BadRequest;
            details.Detail = exception.Message;
        }
        else if (exception is ExpressionTreeEvaluationException or ArithmeticExpressionEvaluationException)
        {
            details.Status = StatusCodes.Status422UnprocessableEntity;
            details.Detail = exception.Message;
        }
        else if (exception is ApplicationValidationException)
        {
            details.Status = StatusCodes.Status400BadRequest;
            var casted = (ApplicationValidationException)exception;
            details.Extensions["errors"] = casted.Errors.Select(error => error.ErrorMessage).ToList();
        }
        else
        {
            details.Status = StatusCodes.Status500InternalServerError;
            details.Detail = "Unexpected Error";
        }

        context.HttpContext.Response.StatusCode = details.Status.Value!;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}

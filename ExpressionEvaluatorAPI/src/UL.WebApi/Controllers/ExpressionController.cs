using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UL.Application.Expression.Command;
using UL.WebApi.Versioning;

namespace UL.WebApi.Controllers;
[ApiController]
[ApiVersion(ApiVersions.v1)]
[Route("api/v{version:apiVersion}/expression")]

public class ExpressionController : ControllerBase
{

    private readonly ISender _sender;

    public ExpressionController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [MapToApiVersion(ApiVersions.v1)]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> EvaluateTreeExpression(CancellationToken cancellationToken,[FromBody] string expression) 
    {
        var command = new EvaluateTreeExpressionCommand(expression);
        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }
}

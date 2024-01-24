using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Domain.Exceptions;

namespace UL.Application.Behaviour.Logging;
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
{

    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;

        try
        {
            _logger.LogInformation("Executing request {RequestName}", requestName);

            var result = await next();

            _logger.LogInformation("Request {RequestName} processed successfully", requestName);

            return result;
        }
        catch (Exception exception) 
        {
            _logger.LogError(exception, "Request {RequestName} processing failed", requestName);
            throw;
        }

    }
}




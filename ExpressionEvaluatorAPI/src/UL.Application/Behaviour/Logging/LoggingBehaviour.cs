using MediatR;
using Microsoft.Extensions.Logging;
using UL.Application.Resources;

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
            _logger.LogInformation(LoggingTemplateResources.Executing_Request, requestName);

            var result = await next();

            _logger.LogInformation(LoggingTemplateResources.Request_Processed, requestName);

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, LoggingTemplateResources.Request_Falied, requestName, exception);
            throw;
        }

    }
}




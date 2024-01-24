using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.Application.Abstractions.Command;
using UL.Application.Exceptions;

namespace UL.Application.Behaviour.Validation;
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{

    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(!_validators.Any()) 
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errors = _validators.Select(validator => validator.Validate(context))
                                .Where(result => result.Errors.Any())
                                .SelectMany(result => result.Errors)
                                .Select(error => new ValidationError(error.PropertyName,error.ErrorMessage))
                                .ToList();

        if (errors.Any())
        {
            throw new ApplicationValidationException(errors);
        }

        return await next();

    }
}

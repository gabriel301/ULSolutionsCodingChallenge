using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UL.Application.Behaviour.Logging;
using UL.Application.Behaviour.Validation;
using UL.Application.DomainServices;
using UL.Domain.Services.Abstraction;

namespace UL.Application;

public static class DepencencyInjection
{
    public static IServiceCollection RegisterApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(builder =>
        {
            //Register Commands Handlers (CLasses that Implement IRequest and IRequestHandler Interfaces from MediatR)
            builder.RegisterServicesFromAssemblies(typeof(DepencencyInjection).Assembly);

            //Register Logging Pipeline Behaviour
            builder.AddOpenBehavior(typeof(LoggingBehaviour<,>));

            //Register Validation Pipeline Behaviour
            builder.AddOpenBehavior(typeof(ValidationBehaviour<,>));


        });

        //Add Validators (Classes that implement AbstractValidator Class from Fluent Validator)
        services.AddValidatorsFromAssembly(typeof(DepencencyInjection).Assembly);


        //Register Operation Service
        services.AddTransient<IOperationService, BasicArithmeticOperationService>();

        return services;
    }
}

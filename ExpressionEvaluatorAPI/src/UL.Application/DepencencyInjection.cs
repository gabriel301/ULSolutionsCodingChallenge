using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using UL.Application.Abstractions.Command;
using UL.Application.Behaviour.Logging;
using UL.Application.Behaviour.Validation;

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
        
        //Add Validators (Classed that implements AbstractValidator Class from Fluent Validator)
        services.AddValidatorsFromAssembly(typeof(DepencencyInjection).Assembly);

        return services;
    }
}

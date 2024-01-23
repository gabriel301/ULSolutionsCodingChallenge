using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Runtime.CompilerServices;
using UL.Application.Abstractions.Command;

namespace UL.Application;

public static class DepencencyInjection
{
    public static IServiceCollection RegisterApplicationDependencies(this IServiceCollection services, IConfiguration configurations) 
    {
        services.AddMediatR(builder => builder.RegisterServicesFromAssemblies(typeof(IBaseCommand).Assembly));

        return services;
    }
}

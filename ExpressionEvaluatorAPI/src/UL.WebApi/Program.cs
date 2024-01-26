
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Serilog;
using UL.Application;
using UL.WebApi.Midleware;
using UL.WebApi.Versioning;

namespace UL.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Removing server name from response header
        //Enforce TLS 3
        builder.WebHost.ConfigureKestrel(config =>
        {
            config.AddServerHeader = false;
            config.ConfigureHttpsDefaults(x => x.SslProtocols = System.Security.Authentication.SslProtocols.Tls13);
        });

        builder.Host.UseSerilog((context, loggerConfig) =>
        loggerConfig.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddControllers(options => options.Filters.Add(typeof(GlobalExceptionHandlingMidleware)));
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(2);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.UnsupportedApiVersionStatusCode = StatusCodes.Status404NotFound;
        })
       .AddMvc()
       .AddApiExplorer(options =>
       {
           options.GroupNameFormat = "'v'V";
           options.SubstituteApiVersionInUrl = true;
       });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v" + ApiVersions.v1, new OpenApiInfo()
            {
                Title = "Expression API v1",
                Version = $"v{ApiVersions.v1}"
            });
            options.SwaggerDoc("v" + ApiVersions.v2, new OpenApiInfo()
            {
                Title = "Expression API v2",
                Version = $"v{ApiVersions.v2}"
            });
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.CustomSchemaIds(x => x.FullName);
        });

        builder.Services.RegisterApplicationDependencies();


        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("fixed", configuration =>
            {
                configuration.QueueLimit = 0;
                configuration.PermitLimit = 200;
                //configuration.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                configuration.Window = TimeSpan.FromSeconds(1);
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v{ApiVersions.v1}/swagger.json", $"v{ApiVersions.v1}");
                options.SwaggerEndpoint($"/swagger/v{ApiVersions.v2}/swagger.json", $"v{ApiVersions.v2}");
            });
        }

        app.UseHttpsRedirection();

        app.UseSerilogRequestLogging();

        app.UseRateLimiter();

        app.MapControllers();

        app.Run();


    }
}


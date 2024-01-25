
using UL.Application;
using Serilog;
using UL.WebApi.Midleware;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;

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

        builder.Services.AddControllers( options => options.Filters.Add(typeof(GlobalExceptionHandlingMidleware)));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.RegisterApplicationDependencies();
        builder.Services.AddApiVersioning(options => 
        {
            options.DefaultApiVersion = new ApiVersion(1);
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

        builder.Services.AddRateLimiter( options => 
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("fixed", configuration => 
            {
                configuration.QueueLimit = 10;
                configuration.PermitLimit = 10;
                configuration.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                configuration.Window = TimeSpan.FromSeconds(1);
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseSerilogRequestLogging();

        app.UseRateLimiter();

        app.MapControllers();

        app.Run();
    }
}

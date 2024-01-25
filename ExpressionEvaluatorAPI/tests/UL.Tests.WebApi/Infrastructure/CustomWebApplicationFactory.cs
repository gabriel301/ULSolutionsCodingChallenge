using Microsoft.AspNetCore.Mvc.Testing;
using UL.WebApi;
using Xunit;

namespace UL.Tests.WebApi.Infrastructure;
public class CustomWebApplicationFactory : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient HttpClient;

    protected CustomWebApplicationFactory(WebApplicationFactory<Program> factory)
    {
        HttpClient = factory.CreateClient();
    }
}

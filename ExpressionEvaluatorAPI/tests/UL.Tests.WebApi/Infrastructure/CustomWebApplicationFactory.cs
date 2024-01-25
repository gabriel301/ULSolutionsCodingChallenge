using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UL.WebApi;
using Xunit;

namespace UL.Tests.WebApi.Infrastructure;
public class CustomWebApplicationFactory: IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient HttpClient;

    protected CustomWebApplicationFactory(WebApplicationFactory<Program> factory) 
    {
        HttpClient = factory.CreateClient();
    }
}

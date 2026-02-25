using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationInsightsRepro.IntegrationTests;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public delegate void RegisterCustomServicesHandler(IServiceCollection services);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Integration");

        // Set environment variables here - builder.ConfigureAppConfiguration is already too late
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", GlobalSetupFixture.DatabaseConnectionString);
    }
}


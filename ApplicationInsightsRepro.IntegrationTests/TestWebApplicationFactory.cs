using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace ApplicationInsightsRepro.IntegrationTests;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public delegate void RegisterCustomServicesHandler(IServiceCollection services);

    private readonly RegisterCustomServicesHandler? _registerCustomServicesHandler;
    private readonly bool _addTestAuthentication;

    public TestWebApplicationFactory(
        RegisterCustomServicesHandler? registerCustomServicesHandler,
        bool addTestAuthentication)
    {
        _registerCustomServicesHandler = registerCustomServicesHandler;
        _addTestAuthentication = addTestAuthentication;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Integration");

        // Set environment variables here - builder.ConfigureAppConfiguration is already too late
        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", GlobalSetupFixture.DatabaseConnectionString);

        var rabbitPort = GlobalSetupFixture.RabbitContainer.GetMappedPublicPort(RabbitMqBuilder.RabbitMqPort);
        Environment.SetEnvironmentVariable("RabbitMQ:Port", rabbitPort.ToString());

        var redisPort = GlobalSetupFixture.RedisContainer.GetMappedPublicPort(RedisBuilder.RedisPort);
        var redisConfiguration = $"localhost:{redisPort},abortConnect=true";
        Environment.SetEnvironmentVariable("Redis:ConfigString", redisConfiguration);

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            // Needed to overwrite the connection string in our azure pipeline
            configurationBuilder.AddEnvironmentVariables();

            configurationBuilder.AddInMemoryCollection([
                new KeyValuePair<string, string?>(
                    "Redis:ConfigString",
                    redisConfiguration)]);
        });
    }
}


using Microsoft.Data.SqlClient;
using Respawn;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace ApplicationInsightsRepro.IntegrationTests;

[SetUpFixture]
internal sealed class GlobalSetupFixture
{
    private static Respawner? s_respawner;

    public static MsSqlContainer DatabaseContainer { get; } = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .WithPortBinding(MsSqlBuilder.MsSqlPort, assignRandomHostPort: true)
        .Build();

    public static RabbitMqContainer RabbitContainer { get; } = new RabbitMqBuilder("rabbitmq:3.11")
        .WithUsername("guest")
        .WithPassword("guest")
        .WithPortBinding(RabbitMqBuilder.RabbitMqPort, assignRandomHostPort: true)
        .Build();

    public static RedisContainer RedisContainer { get; } = new RedisBuilder("redis:7.0")
        .WithPortBinding(RedisBuilder.RedisPort, assignRandomHostPort: true)
        .Build();

    public static string DatabaseConnectionString { get; private set; } = string.Empty;


    [OneTimeSetUp]
    public async Task SetUp()
    {
        await Task.WhenAll(
            DatabaseContainer.StartAsync(),
            RabbitContainer.StartAsync(),
            RedisContainer.StartAsync());

        DatabaseConnectionString = DatabaseContainer
            .GetConnectionString()
            .Replace("Database=master", $"Database=estore-{Guid.NewGuid()}");
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await DatabaseContainer.DisposeAsync();
        await RabbitContainer.DisposeAsync();
        await RedisContainer.DisposeAsync();
    }

    public static async Task RespawnDatabaseAsync()
    {
        using var connection = new SqlConnection(DatabaseConnectionString);

        try
        {
            await connection.OpenAsync();

            if (s_respawner is null)
            {
                s_respawner = await Respawner.CreateAsync(
                    connection,
                    new RespawnerOptions
                    {
                        TablesToIgnore = ["__EFMigrationsHistory"]
                    });
            }

            await s_respawner.ResetAsync(connection);
        }
        catch (Exception ex)
        {
            // Creation of the respawner can fail if the database has not been created yet
            TestContext.Out.WriteLine($"Failed to respawn database: {ex.Message}");
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}

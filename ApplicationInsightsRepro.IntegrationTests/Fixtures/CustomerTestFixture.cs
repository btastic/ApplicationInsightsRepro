using ApplicationInsightsRepro.Data;
using ApplicationInsightsRepro.IntegrationTests.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationInsightsRepro.IntegrationTests.Fixtures;

public class CustomerTestFixture : DefaultTestFixture
{
    public CustomerTestFixture()
    {
    }

    public async Task SeedTestData()
    {
        await GlobalSetupFixture.RespawnDatabaseAsync();

        // Seed customer data
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await CustomerSeeder.SeedCustomersAsync(context);
    }
}

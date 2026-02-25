using ApplicationInsightsRepro.Data;
using ApplicationInsightsRepro.IntegrationTests.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationInsightsRepro.IntegrationTests.Fixtures;

public class OrderTestFixture : DefaultTestFixture
{
    public OrderTestFixture()
    {
    }

    public async Task SeedTestData()
    {
        await GlobalSetupFixture.RespawnDatabaseAsync();
        
        // Seed customer data first, then orders
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await CustomerSeeder.SeedCustomersAsync(context);
        await OrderSeeder.SeedOrdersAsync(context);
    }
}

using ApplicationInsightsRepro.Data;
using ApplicationInsightsRepro.Models;

namespace ApplicationInsightsRepro.IntegrationTests.Data;

public static class CustomerSeeder
{
    public static async Task SeedCustomersAsync(ApplicationDbContext context)
    {
        var customers = new List<Customer>
        {
            new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "555-0101",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                PhoneNumber = "555-0102",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                PhoneNumber = "555-0103",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                FirstName = "Alice",
                LastName = "Williams",
                Email = "alice.williams@example.com",
                PhoneNumber = null,
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                FirstName = "Charlie",
                LastName = "Brown",
                Email = "charlie.brown@example.com",
                PhoneNumber = "555-0105",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();
    }
}

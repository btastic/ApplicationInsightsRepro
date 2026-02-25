using ApplicationInsightsRepro.Data;
using ApplicationInsightsRepro.Models;
using Microsoft.EntityFrameworkCore;

namespace ApplicationInsightsRepro.IntegrationTests.Data;

public static class OrderSeeder
{
    public static async Task SeedOrdersAsync(ApplicationDbContext context)
    {
        var customers = await context.Customers.Take(3).ToListAsync();
        
        if (customers.Count == 0)
        {
            return;
        }

        var orders = new List<Order>
        {
            new Order
            {
                CustomerId = customers[0].Id,
                OrderNumber = "ORD-001",
                OrderDate = DateTime.UtcNow.AddDays(-10),
                TotalAmount = 299.99m,
                Status = "Completed",
                Notes = "First order",
                CreatedAt = DateTime.UtcNow
            },
            new Order
            {
                CustomerId = customers[0].Id,
                OrderNumber = "ORD-002",
                OrderDate = DateTime.UtcNow.AddDays(-5),
                TotalAmount = 149.50m,
                Status = "Shipped",
                Notes = null,
                CreatedAt = DateTime.UtcNow
            },
            new Order
            {
                CustomerId = customers[1].Id,
                OrderNumber = "ORD-003",
                OrderDate = DateTime.UtcNow.AddDays(-3),
                TotalAmount = 599.99m,
                Status = "Processing",
                Notes = "Rush order",
                CreatedAt = DateTime.UtcNow
            },
            new Order
            {
                CustomerId = customers[1].Id,
                OrderNumber = "ORD-004",
                OrderDate = DateTime.UtcNow.AddDays(-2),
                TotalAmount = 89.99m,
                Status = "Pending",
                Notes = null,
                CreatedAt = DateTime.UtcNow
            },
            new Order
            {
                CustomerId = customers[2].Id,
                OrderNumber = "ORD-005",
                OrderDate = DateTime.UtcNow.AddDays(-1),
                TotalAmount = 1299.00m,
                Status = "Completed",
                Notes = "Large order with discount",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }
}

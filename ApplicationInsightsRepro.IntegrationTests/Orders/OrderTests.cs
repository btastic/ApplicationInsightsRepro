using System.Net;
using System.Net.Http.Json;
using ApplicationInsightsRepro.DTOs;
using ApplicationInsightsRepro.IntegrationTests.Fixtures;
using FluentAssertions;

namespace ApplicationInsightsRepro.IntegrationTests.Orders;

[TestFixture]
public sealed class OrderTests
{
    private readonly OrderTestFixture _fixture = new();

    [OneTimeSetUp]
    public async Task OneTimeSetUp() => await _fixture.SeedTestData();

    [Test]
    public async Task GetOrders_ReturnsAllOrders()
    {
        // Act
        var response = await _fixture.ServerClient.GetAsync("/api/orders");
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        orders.Should().NotBeNull();
        orders.Should().HaveCountGreaterOrEqualTo(5);
        orders![0].Customer.Should().NotBeNull();
    }

    [Test]
    public async Task GetOrder_WithValidId_ReturnsOrder()
    {
        // Arrange - Get an existing order
        var ordersResponse = await _fixture.ServerClient.GetAsync("/api/orders");
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await ordersResponse.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        orders.Should().NotBeNull().And.HaveCountGreaterThan(0);

        var orderId = orders![0].Id;

        // Act
        var response = await _fixture.ServerClient.GetAsync($"/api/orders/{orderId}");
        var order = await response.Content.ReadFromJsonAsync<OrderResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        order.Should().NotBeNull();
        order!.Id.Should().Be(orderId);
        order.OrderNumber.Should().NotBeNullOrEmpty();
        order.CustomerId.Should().BeGreaterThan(0);
        order.TotalAmount.Should().BeGreaterThan(0);
        order.Status.Should().NotBeNullOrEmpty();
        order.Customer.Should().NotBeNull();
    }

    [Test]
    public async Task GetOrder_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int orderId = 99999;

        // Act
        var response = await _fixture.ServerClient.GetAsync($"/api/orders/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetOrdersByCustomer_WithValidCustomerId_ReturnsOrders()
    {
        // Arrange - Get an existing customer with orders
        var ordersResponse = await _fixture.ServerClient.GetAsync("/api/orders");
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var allOrders = await ordersResponse.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        allOrders.Should().NotBeNull().And.HaveCountGreaterThan(0);

        var customerId = allOrders![0].CustomerId;

        // Act
        var response = await _fixture.ServerClient.GetAsync($"/api/orders/customer/{customerId}");
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        orders.Should().NotBeNull();
        orders.Should().HaveCountGreaterOrEqualTo(1);
        orders.Should().AllSatisfy(o => o.CustomerId.Should().Be(customerId));
    }

    [Test]
    public async Task GetOrdersByCustomer_WithInvalidCustomerId_ReturnsNotFound()
    {
        // Arrange
        const int customerId = 99999;

        // Act
        var response = await _fixture.ServerClient.GetAsync($"/api/orders/customer/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task CreateOrder_WithValidData_ReturnsCreatedOrder()
    {
        // Arrange - Get an existing customer first
        var customersResponse = await _fixture.ServerClient.GetAsync("/api/customers");
        customersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await customersResponse.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customers.Should().NotBeNull().And.HaveCountGreaterThan(0);

        var customerId = customers![0].Id;

        var newOrder = new CreateOrderDto(
            CustomerId: customerId,
            OrderNumber: $"ORD-{Guid.NewGuid().ToString()[..8]}",
            OrderDate: DateTime.UtcNow,
            TotalAmount: 499.99m,
            Status: "Pending",
            Notes: "Test order");

        // Act
        var response = await _fixture.ServerClient.PostAsJsonAsync("/api/orders", newOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created,
            because: await response.Content.ReadAsStringAsync());
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        createdOrder.Should().NotBeNull();
        createdOrder!.CustomerId.Should().Be(newOrder.CustomerId);
        createdOrder.OrderNumber.Should().Be(newOrder.OrderNumber);
        createdOrder.TotalAmount.Should().Be(newOrder.TotalAmount);
        createdOrder.Status.Should().Be(newOrder.Status);
        createdOrder.Notes.Should().Be(newOrder.Notes);
        createdOrder.Id.Should().BeGreaterThan(0);
        createdOrder.Customer.Should().NotBeNull();
    }

    [Test]
    public async Task CreateOrder_WithInvalidCustomerId_ReturnsBadRequest()
    {
        // Arrange
        var newOrder = new CreateOrderDto(
            CustomerId: 99999,
            OrderNumber: $"ORD-{Guid.NewGuid().ToString()[..8]}",
            OrderDate: DateTime.UtcNow,
            TotalAmount: 499.99m,
            Status: "Pending",
            Notes: null);

        // Act
        var response = await _fixture.ServerClient.PostAsJsonAsync("/api/orders", newOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateOrder_WithValidData_ReturnsNoContent()
    {
        // Arrange - Get an existing order
        var ordersResponse = await _fixture.ServerClient.GetAsync("/api/orders");
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await ordersResponse.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        orders.Should().NotBeNull().And.HaveCountGreaterThan(0);

        var existingOrder = orders![0];
        var orderId = existingOrder.Id;

        var updateOrder = new UpdateOrderDto(
            CustomerId: existingOrder.CustomerId,
            OrderNumber: existingOrder.OrderNumber,
            OrderDate: DateTime.UtcNow,
            TotalAmount: 399.99m,
            Status: "Shipped",
            Notes: "Updated order");

        // Act
        var response = await _fixture.ServerClient.PutAsJsonAsync($"/api/orders/{orderId}", updateOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await _fixture.ServerClient.GetAsync($"/api/orders/{orderId}");
        var order = await getResponse.Content.ReadFromJsonAsync<OrderResponseDto>();
        order.Should().NotBeNull();
        order!.TotalAmount.Should().Be(updateOrder.TotalAmount);
        order.Status.Should().Be(updateOrder.Status);
        order.Notes.Should().Be(updateOrder.Notes);
    }

    [Test]
    public async Task UpdateOrder_WithInvalidId_ReturnsNotFound()
    {
        // Arrange - Get an existing customer
        var customersResponse = await _fixture.ServerClient.GetAsync("/api/customers");
        customersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await customersResponse.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customers.Should().NotBeNull().And.HaveCountGreaterThan(0);

        const int orderId = 99999;
        var updateOrder = new UpdateOrderDto(
            CustomerId: customers![0].Id,
            OrderNumber: "ORD-999",
            OrderDate: DateTime.UtcNow,
            TotalAmount: 99.99m,
            Status: "Cancelled",
            Notes: null);

        // Act
        var response = await _fixture.ServerClient.PutAsJsonAsync($"/api/orders/{orderId}", updateOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateOrder_WithInvalidCustomerId_ReturnsBadRequest()
    {
        // Arrange - Get an existing order
        var ordersResponse = await _fixture.ServerClient.GetAsync("/api/orders");
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await ordersResponse.Content.ReadFromJsonAsync<List<OrderResponseDto>>();
        orders.Should().NotBeNull().And.HaveCountGreaterThan(0);

        var orderId = orders![0].Id;
        var updateOrder = new UpdateOrderDto(
            CustomerId: 99999,
            OrderNumber: orders[0].OrderNumber,
            OrderDate: DateTime.UtcNow,
            TotalAmount: 99.99m,
            Status: "Cancelled",
            Notes: null);

        // Act
        var response = await _fixture.ServerClient.PutAsJsonAsync($"/api/orders/{orderId}", updateOrder);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteOrder_WithValidId_ReturnsNoContent()
    {
        // Arrange - Get an existing customer first
        var customersResponse = await _fixture.ServerClient.GetAsync("/api/customers");
        customersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await customersResponse.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();
        customers.Should().NotBeNull().And.HaveCountGreaterThan(0);

        var customerId = customers![0].Id;

        // Create an order to delete
        var newOrder = new CreateOrderDto(
            CustomerId: customerId,
            OrderNumber: $"ORD-DELETE-{Guid.NewGuid().ToString()[..8]}",
            OrderDate: DateTime.UtcNow,
            TotalAmount: 50.00m,
            Status: "Pending",
            Notes: null);

        var createResponse = await _fixture.ServerClient.PostAsJsonAsync("/api/orders", newOrder);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, 
            because: await createResponse.Content.ReadAsStringAsync());
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>();

        // Act
        var deleteResponse = await _fixture.ServerClient.DeleteAsync($"/api/orders/{createdOrder!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the order is deleted
        var getResponse = await _fixture.ServerClient.GetAsync($"/api/orders/{createdOrder.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteOrder_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int orderId = 99999;

        // Act
        var response = await _fixture.ServerClient.DeleteAsync($"/api/orders/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

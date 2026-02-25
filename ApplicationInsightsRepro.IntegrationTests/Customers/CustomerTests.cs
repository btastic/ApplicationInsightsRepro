using System.Net;
using System.Net.Http.Json;
using ApplicationInsightsRepro.DTOs;
using ApplicationInsightsRepro.IntegrationTests.Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApplicationInsightsRepro.IntegrationTests.Customers;

[TestFixture]
public sealed class CustomerTests
{
    private readonly CustomerTestFixture _fixture = new();

    [OneTimeSetUp]
    public async Task OneTimeSetUp() => await _fixture.SeedTestData();

    [Test]
    public async Task GetCustomers_ReturnsAllCustomers()
    {
        // Act
        var response = await _fixture.ServerClient.GetAsync("/api/customers");
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerResponseDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        customers.Should().NotBeNull();
        customers.Should().HaveCountGreaterOrEqualTo(5);
    }

    [Test]
    public async Task GetCustomer_WithValidId_ReturnsCustomer()
    {
        // Arrange
        const int customerId = 1;

        // Act
        var response = await _fixture.ServerClient.GetAsync($"/api/customers/{customerId}");
        var customer = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(customerId);
        customer.FirstName.Should().NotBeNullOrEmpty();
        customer.LastName.Should().NotBeNullOrEmpty();
        customer.Email.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task GetCustomer_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int customerId = 99999;

        // Act
        var response = await _fixture.ServerClient.GetAsync($"/api/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task CreateCustomer_WithValidData_ReturnsCreatedCustomer()
    {
        // Arrange
        var newCustomer = new CreateCustomerDto(
            "Test",
            "User",
            $"test.user.{Guid.NewGuid()}@example.com",
            "555-0999");

        // Act
        var response = await _fixture.ServerClient.PostAsJsonAsync("/api/customers", newCustomer);
        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerResponseDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCustomer.Should().NotBeNull();
        createdCustomer!.FirstName.Should().Be(newCustomer.FirstName);
        createdCustomer.LastName.Should().Be(newCustomer.LastName);
        createdCustomer.Email.Should().Be(newCustomer.Email);
        createdCustomer.PhoneNumber.Should().Be(newCustomer.PhoneNumber);
        createdCustomer.Id.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task UpdateCustomer_WithValidData_ReturnsNoContent()
    {
        // Arrange
        const int customerId = 1;
        var updateCustomer = new UpdateCustomerDto(
            "Updated",
            "Name",
            "john.doe@example.com",
            "555-9999");

        // Act
        var response = await _fixture.ServerClient.PutAsJsonAsync($"/api/customers/{customerId}", updateCustomer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the update
        var getResponse = await _fixture.ServerClient.GetAsync($"/api/customers/{customerId}");
        var customer = await getResponse.Content.ReadFromJsonAsync<CustomerResponseDto>();
        customer.Should().NotBeNull();
        customer!.FirstName.Should().Be(updateCustomer.FirstName);
        customer.LastName.Should().Be(updateCustomer.LastName);
    }

    [Test]
    public async Task UpdateCustomer_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int customerId = 99999;
        var updateCustomer = new UpdateCustomerDto(
            "Updated",
            "Name",
            "updated@example.com",
            "555-9999");

        // Act
        var response = await _fixture.ServerClient.PutAsJsonAsync($"/api/customers/{customerId}", updateCustomer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteCustomer_WithValidId_ReturnsNoContent()
    {
        // Arrange - Create a customer to delete
        var newCustomer = new CreateCustomerDto(
            "ToDelete",
            "User",
            $"delete.user.{Guid.NewGuid()}@example.com",
            "555-0000");

        var createResponse = await _fixture.ServerClient.PostAsJsonAsync("/api/customers", newCustomer);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerResponseDto>();

        // Act
        var deleteResponse = await _fixture.ServerClient.DeleteAsync($"/api/customers/{createdCustomer!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify the customer is deleted
        var getResponse = await _fixture.ServerClient.GetAsync($"/api/customers/{createdCustomer.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteCustomer_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        const int customerId = 99999;

        // Act
        var response = await _fixture.ServerClient.DeleteAsync($"/api/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

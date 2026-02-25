namespace ApplicationInsightsRepro.DTOs;

public record CreateOrderDto(
    int CustomerId,
    string OrderNumber,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    string? Notes);

public record UpdateOrderDto(
    int CustomerId,
    string OrderNumber,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    string? Notes);

public record OrderResponseDto(
    int Id,
    int CustomerId,
    string OrderNumber,
    DateTime OrderDate,
    decimal TotalAmount,
    string Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    CustomerResponseDto? Customer);

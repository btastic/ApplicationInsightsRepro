namespace ApplicationInsightsRepro.DTOs;

public record CreateCustomerDto(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber);

public record UpdateCustomerDto(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber);

public record CustomerResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

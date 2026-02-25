using ApplicationInsightsRepro.Data;
using ApplicationInsightsRepro.DTOs;
using ApplicationInsightsRepro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicationInsightsRepro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ApplicationDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetCustomers()
    {
        var customers = await _context.Customers
            .Select(c => new CustomerResponseDto(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.PhoneNumber,
                c.CreatedAt,
                c.UpdatedAt))
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        var response = new CustomerResponseDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.CreatedAt,
            customer.UpdatedAt);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> CreateCustomer(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return BadRequest("A customer with this email already exists.");
        }

        var response = new CustomerResponseDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.PhoneNumber,
            customer.CreatedAt,
            customer.UpdatedAt);

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.Email = dto.Email;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error updating customer");
            return BadRequest("A customer with this email already exists.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

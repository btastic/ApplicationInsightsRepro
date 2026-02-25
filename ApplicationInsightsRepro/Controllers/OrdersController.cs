using ApplicationInsightsRepro.Data;
using ApplicationInsightsRepro.DTOs;
using ApplicationInsightsRepro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicationInsightsRepro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Select(o => new OrderResponseDto(
                o.Id,
                o.CustomerId,
                o.OrderNumber,
                o.OrderDate,
                o.TotalAmount,
                o.Status,
                o.Notes,
                o.CreatedAt,
                o.UpdatedAt,
                o.Customer != null ? new CustomerResponseDto(
                    o.Customer.Id,
                    o.Customer.FirstName,
                    o.Customer.LastName,
                    o.Customer.Email,
                    o.Customer.PhoneNumber,
                    o.Customer.CreatedAt,
                    o.Customer.UpdatedAt) : null))
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var response = new OrderResponseDto(
            order.Id,
            order.CustomerId,
            order.OrderNumber,
            order.OrderDate,
            order.TotalAmount,
            order.Status,
            order.Notes,
            order.CreatedAt,
            order.UpdatedAt,
            order.Customer != null ? new CustomerResponseDto(
                order.Customer.Id,
                order.Customer.FirstName,
                order.Customer.LastName,
                order.Customer.Email,
                order.Customer.PhoneNumber,
                order.Customer.CreatedAt,
                order.Customer.UpdatedAt) : null);

        return Ok(response);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrdersByCustomer(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
        {
            return NotFound("Customer not found");
        }

        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId == customerId)
            .Select(o => new OrderResponseDto(
                o.Id,
                o.CustomerId,
                o.OrderNumber,
                o.OrderDate,
                o.TotalAmount,
                o.Status,
                o.Notes,
                o.CreatedAt,
                o.UpdatedAt,
                o.Customer != null ? new CustomerResponseDto(
                    o.Customer.Id,
                    o.Customer.FirstName,
                    o.Customer.LastName,
                    o.Customer.Email,
                    o.Customer.PhoneNumber,
                    o.Customer.CreatedAt,
                    o.Customer.UpdatedAt) : null))
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto dto)
    {
        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found");
        }

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            OrderNumber = dto.OrderNumber,
            OrderDate = dto.OrderDate,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating order");
            return BadRequest("An order with this order number already exists.");
        }

        var response = new OrderResponseDto(
            order.Id,
            order.CustomerId,
            order.OrderNumber,
            order.OrderDate,
            order.TotalAmount,
            order.Status,
            order.Notes,
            order.CreatedAt,
            order.UpdatedAt,
            new CustomerResponseDto(
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.PhoneNumber,
                customer.CreatedAt,
                customer.UpdatedAt));

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto dto)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers.FindAsync(dto.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found");
        }

        order.CustomerId = dto.CustomerId;
        order.OrderNumber = dto.OrderNumber;
        order.OrderDate = dto.OrderDate;
        order.TotalAmount = dto.TotalAmount;
        order.Status = dto.Status;
        order.Notes = dto.Notes;
        order.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error updating order");
            return BadRequest("An order with this order number already exists.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

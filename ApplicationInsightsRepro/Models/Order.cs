namespace ApplicationInsightsRepro.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public required string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public required string Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Customer? Customer { get; set; }
}

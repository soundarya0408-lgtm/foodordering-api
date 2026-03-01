namespace FoodOrdering.Application.Customers;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? DOB { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

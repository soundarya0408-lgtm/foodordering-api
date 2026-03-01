namespace FoodOrdering.Application.Customers;

public class CreateCustomerRequest
{
    public string Name { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? DOB { get; set; }
}

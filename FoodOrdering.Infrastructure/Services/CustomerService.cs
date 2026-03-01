using FoodOrdering.Application.Customers;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly FoodOrderingDbContext _dbContext;

    public CustomerService(FoodOrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(request.MobileNumber))
            throw new ArgumentException("Mobile number is required");

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            MobileNumber = request.MobileNumber.Trim(),
            Email = request.Email?.Trim(),
            Address = request.Address?.Trim()
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            MobileNumber = customer.MobileNumber,
            Email = customer.Email,
            Address = customer.Address
        };
    }

    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync()
    {
        return await _dbContext.Customers
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                MobileNumber = c.MobileNumber,
                Email = c.Email,
                Address = c.Address,
                CreatedAt = c.CreatedAt,                
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateCustomerInfo(int customerId, CustomerDto customerDto)
    {
        var customer = await _dbContext.Customers.FindAsync(customerId);
        if (customer == null) return false;

        customer.Name = customerDto.Name;
        customer.MobileNumber = customerDto.MobileNumber;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCustomer(int customerId)
    {
        var customer = await _dbContext.Customers.FindAsync(customerId);
        if (customer == null) return false;

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}

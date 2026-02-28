using FoodOrdering.Application.Auth;
using FoodOrdering.Application.Customers;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodOrdering.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly FoodOrderingDbContext _dbContext;
    private readonly IConfiguration _config;

    public AuthService(FoodOrderingDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Find customer by mobile (case-insensitive)
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.MobileNumber == request.MobileNumber);

        if (customer == null)
            throw new UnauthorizedAccessException("Customer not found");

        // Generate JWT token
        var token = GenerateJwtToken(customer);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    private string GenerateJwtToken(Customer customer)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.Name),
            new Claim(ClaimTypes.MobilePhone, customer.MobileNumber)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

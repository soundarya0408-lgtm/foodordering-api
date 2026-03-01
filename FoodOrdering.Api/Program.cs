using FoodOrdering.Application.Auth;
using FoodOrdering.Application.Customers;
using FoodOrdering.Domain.Entities;
using FoodOrdering.Infrastructure.Interceptors;
using FoodOrdering.Infrastructure.Persistence;
using FoodOrdering.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FoodOrdering API", Version = "v1" });

    // JWT Authorization support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuditInterceptor>();

// DB Context

//Prod:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FoodOrderingDbContext>(options =>
    options.UseSqlite(connectionString)
    .AddInterceptors(new AuditInterceptor()));

//Local:
//var folder = Path.Combine(AppContext.BaseDirectory);
//if (!Directory.Exists(folder))
//    Directory.CreateDirectory(folder);
//var dbPath = Path.Combine(AppContext.BaseDirectory, "foodordering.db");
//builder.Services.AddDbContext<FoodOrderingDbContext>(options =>
//    options.UseSqlite($"Data Source={dbPath}")
//    .AddInterceptors(new AuditInterceptor()));



// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Authorization policies
builder.Services.AddAuthorization();
var app = builder.Build();

//FIX (Swagger always enabled)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodOrdering API V1");
    c.RoutePrefix = "swagger";
});


app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();     
app.UseAuthorization();
app.MapControllers();


// Auto apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FoodOrderingDbContext>();
    db.Database.Migrate();

    // Seed default customer if empty
    if (!db.Customers.Any())
    {
        db.Customers.AddRange(new Customer
        {
            Name = "Soundarya",
            MobileNumber = "9876543210",
            Email = "sound@example.com",
            Address = "Chennai",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        },
        new Customer
        {
            Name = "Shiranjeevi",
            MobileNumber = "9876543210",
            Email = "shira@example.com",
            Address = "Chennai",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System"
        });
        db.SaveChanges();
    }
}

app.Run();

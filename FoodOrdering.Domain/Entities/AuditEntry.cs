using System.ComponentModel.DataAnnotations;

namespace FoodOrdering.Domain.Entities;

public abstract class AuditEntity
{
    [MaxLength(200)]
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(200)]
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Backend.Core.Entities;

public class Category
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Display order for UI
    public int DisplayOrder { get; set; }
    
    // Icon or image URL for the category
    public string? IconUrl { get; set; }
}

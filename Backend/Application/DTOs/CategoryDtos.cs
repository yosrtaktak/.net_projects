using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int DisplayOrder { get; set; }
    public string? IconUrl { get; set; }
    public int VehicleCount { get; set; } // Number of vehicles in this category
}

public class CreateCategoryDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [Range(0, 1000)]
    public int DisplayOrder { get; set; } = 0;
    
    public string? IconUrl { get; set; }
}

public class UpdateCategoryDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
    
    [Range(0, 1000)]
    public int DisplayOrder { get; set; }
    
    public string? IconUrl { get; set; }
}

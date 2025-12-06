using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int DisplayOrder { get; set; }
    public string? IconUrl { get; set; }
    public int VehicleCount { get; set; }
}

public class CreateCategoryModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    public int DisplayOrder { get; set; } = 0;
    
    public string? IconUrl { get; set; }
}

public class UpdateCategoryModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
    
    [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    public int DisplayOrder { get; set; }
    
    public string? IconUrl { get; set; }
}

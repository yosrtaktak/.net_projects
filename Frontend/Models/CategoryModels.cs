using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Frontend.Models;

public class CategoryModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updatedAt")]
    public DateTime? UpdatedAt { get; set; }
    
    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
    
    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }
    
    [JsonPropertyName("vehicleCount")]
    public int VehicleCount { get; set; }
}

public class CreateCategoryModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
    
    [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; } = 0;
    
    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }
}

public class UpdateCategoryModel
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [Range(0, 1000, ErrorMessage = "Display order must be between 0 and 1000")]
    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
    
    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }
}

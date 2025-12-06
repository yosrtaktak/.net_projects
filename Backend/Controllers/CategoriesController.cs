using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.DTOs;
using Backend.Core.Entities;
using Backend.Core.Interfaces;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery] bool activeOnly = false)
    {
        var categories = activeOnly 
            ? await _categoryRepository.GetActiveAsync()
            : await _categoryRepository.GetAllAsync();

        var categoryDtos = new List<CategoryDto>();
        foreach (var category in categories)
        {
            var vehicleCount = await _categoryRepository.GetVehicleCountAsync(category.Id);
            categoryDtos.Add(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                DisplayOrder = category.DisplayOrder,
                IconUrl = category.IconUrl,
                VehicleCount = vehicleCount
            });
        }

        return Ok(categoryDtos);
    }

    // GET: api/categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        var vehicleCount = await _categoryRepository.GetVehicleCountAsync(category.Id);

        var categoryDto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            DisplayOrder = category.DisplayOrder,
            IconUrl = category.IconUrl,
            VehicleCount = vehicleCount
        };

        return Ok(categoryDto);
    }

    // POST: api/categories
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        // Check if category name already exists
        if (await _categoryRepository.NameExistsAsync(dto.Name))
        {
            return BadRequest(new { message = "A category with this name already exists" });
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder,
            IconUrl = dto.IconUrl,
            CreatedAt = DateTime.UtcNow
        };

        var createdCategory = await _categoryRepository.CreateAsync(category);

        var categoryDto = new CategoryDto
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name,
            Description = createdCategory.Description,
            IsActive = createdCategory.IsActive,
            CreatedAt = createdCategory.CreatedAt,
            UpdatedAt = createdCategory.UpdatedAt,
            DisplayOrder = createdCategory.DisplayOrder,
            IconUrl = createdCategory.IconUrl,
            VehicleCount = 0
        };

        return CreatedAtAction(nameof(GetCategory), new { id = categoryDto.Id }, categoryDto);
    }

    // PUT: api/categories/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        // Check if new name conflicts with existing category
        if (await _categoryRepository.NameExistsAsync(dto.Name, id))
        {
            return BadRequest(new { message = "A category with this name already exists" });
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.DisplayOrder = dto.DisplayOrder;
        category.IconUrl = dto.IconUrl;

        var updatedCategory = await _categoryRepository.UpdateAsync(category);
        var vehicleCount = await _categoryRepository.GetVehicleCountAsync(updatedCategory.Id);

        var categoryDto = new CategoryDto
        {
            Id = updatedCategory.Id,
            Name = updatedCategory.Name,
            Description = updatedCategory.Description,
            IsActive = updatedCategory.IsActive,
            CreatedAt = updatedCategory.CreatedAt,
            UpdatedAt = updatedCategory.UpdatedAt,
            DisplayOrder = updatedCategory.DisplayOrder,
            IconUrl = updatedCategory.IconUrl,
            VehicleCount = vehicleCount
        };

        return Ok(categoryDto);
    }

    // DELETE: api/categories/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        
        if (category == null)
            return NotFound(new { message = "Category not found" });

        var vehicleCount = await _categoryRepository.GetVehicleCountAsync(id);
        if (vehicleCount > 0)
        {
            return BadRequest(new { message = $"Cannot delete category. It has {vehicleCount} vehicle(s) associated with it." });
        }

        var deleted = await _categoryRepository.DeleteAsync(id);

        if (!deleted)
            return BadRequest(new { message = "Failed to delete category" });

        return NoContent();
    }

    // PATCH: api/categories/5/toggle
    [HttpPatch("{id}/toggle")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> ToggleActive(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        category.IsActive = !category.IsActive;
        var updatedCategory = await _categoryRepository.UpdateAsync(category);
        var vehicleCount = await _categoryRepository.GetVehicleCountAsync(updatedCategory.Id);

        var categoryDto = new CategoryDto
        {
            Id = updatedCategory.Id,
            Name = updatedCategory.Name,
            Description = updatedCategory.Description,
            IsActive = updatedCategory.IsActive,
            CreatedAt = updatedCategory.CreatedAt,
            UpdatedAt = updatedCategory.UpdatedAt,
            DisplayOrder = updatedCategory.DisplayOrder,
            IconUrl = updatedCategory.IconUrl,
            VehicleCount = vehicleCount
        };

        return Ok(categoryDto);
    }
}

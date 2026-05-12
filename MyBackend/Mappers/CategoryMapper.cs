using MyBackend.DTOs.CategoryDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Models;

namespace MyBackend.Mappers;

public class CategoryMapper : ICategoryMapper
{
    public CategoryDto? ToDto(Category? category)
    {
        if (category is null) return null;
        
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }

    public Category ToEntity(CreateCategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }
    
    public void UpdateEntity(UpdateCategoryDto dto, Category entity)
    {
        entity.Name = dto.Name ?? entity.Name;
        entity.Description = dto.Description ?? entity.Description;
    }
}
using MyBackend.DTOs.CategoryDtos;
using MyBackend.Models;

namespace MyBackend.Mappers.Interfaces;

public interface ICategoryMapper
{
    CategoryDto? ToDto(Category? category);
    Category ToEntity(CreateCategoryDto dto);
    void UpdateEntity(UpdateCategoryDto dto, Category category);
}
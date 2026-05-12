using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.DTOs.CategoryDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Services.Interfaces;

namespace MyBackend.Services;

public class CategoryService(AppDbContext _context, ICategoryMapper _mapper) : ICategoryService
{
    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        return categories.Select(c => _mapper.ToDto(c)).ToList();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null) 
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        
        return _mapper.ToDto(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = _mapper.ToEntity(dto);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return _mapper.ToDto(category);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");;

        _mapper.UpdateEntity(dto, category);
        await _context.SaveChangesAsync();
        return _mapper.ToDto(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
            throw new KeyNotFoundException($"Category with ID {id} not found.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
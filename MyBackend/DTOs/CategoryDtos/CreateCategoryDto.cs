namespace MyBackend.DTOs.CategoryDtos;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
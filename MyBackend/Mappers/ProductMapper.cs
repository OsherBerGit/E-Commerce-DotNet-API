using MyBackend.DTOs;
using MyBackend.DTOs.ProductDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Models;

namespace MyBackend.Mappers;

public class ProductMapper : IProductMapper
{
    public ProductDto? ToDto(Product? product)
    {
        if (product is null) return null;
        
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl
        };
    }

    public Product ToEntity(CreateProductDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Quantity = dto.Quantity,
            CategoryId = dto.CategoryId ?? 0,
            ImageUrl = dto.ImageUrl 
        };
    }

    public void UpdateEntity(UpdateProductDto dto, Product entity)
    {
        entity.Name = dto.Name ?? entity.Name;
        entity.Description = dto.Description ?? entity.Description;
        entity.Price = dto.Price ?? entity.Price;
        entity.Quantity = dto.Quantity ?? entity.Quantity;
        entity.CategoryId = dto.CategoryId ?? entity.CategoryId;
        entity.ImageUrl = dto.ImageUrl ?? entity.ImageUrl;
    }
}
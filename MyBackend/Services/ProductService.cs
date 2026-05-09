using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.DTOs;
using MyBackend.DTOs.ProductDtos;
using MyBackend.Exceptions;
using MyBackend.Mappers;
using MyBackend.Mappers.Interfaces;
using MyBackend.Services.Interfaces;

namespace MyBackend.Services;

public class ProductService(AppDbContext _context, IProductMapper _mapper, IPhotoService _photoService) : IProductService
{
    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync();
        
        return products.Select(p => _mapper.ToDto(p)!).ToList();
    }
    
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
        
        return _mapper.ToDto(product);
    }
    
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var existingProduct = await _context.Products.AnyAsync(p => p.Name == dto.Name);
        if (existingProduct)
            throw new ProductAlreadyExistsException("Product with this name already exists.");
        
        var product = _mapper.ToEntity(dto);
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.ToDto(product)!;
    }
    
    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return null;
        
        _mapper.UpdateEntity(dto, product);

        await _context.SaveChangesAsync();

        return _mapper.ToDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return false;
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<ProductDto?> UpdateProductQuantityAsync(int id, int delta)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
            return null;
        
        if (product.Quantity + delta < 0)
            throw new InvalidOperationException("Quantity cannot be negative!");
        
        product.Quantity += delta; 

        await _context.SaveChangesAsync();
        
        return _mapper.ToDto(product);
    }
    
    public async Task<ProductDto> AddPhotoToProductAsync(int productId, IFormFile file)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null) 
            throw new KeyNotFoundException("Product not found");
        
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error is not null) 
            throw new Exception($"Cloudinary error: {result.Error.Message}");
        
        product.ImageUrl = result.SecureUrl.AbsoluteUri;
        product.PublicId = result.PublicId;

        await _context.SaveChangesAsync();
        
        return _mapper.ToDto(product); 
    }
}
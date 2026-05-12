using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBackend.DTOs;
using MyBackend.DTOs.ProductDtos;
using MyBackend.Services;
using MyBackend.Services.Interfaces;

namespace MyBackend.Controllers;

[ApiController]
[Route("api/products")]
[EnableRateLimiting("PublicApiPolicy")]
public class ProductController(IProductService _productService) : ControllerBase
{
    [NonAction]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts() => Ok(await _productService.GetAllProductsAsync());
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto?>> CreateProduct(CreateProductDto dto)
    {
        var newProduct = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto?>> UpdateProduct(int id, UpdateProductDto dto)
    {
        var updatedProduct = await _productService.UpdateProductAsync(id, dto);
        return Ok(updatedProduct);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
    
    [HttpPatch("{id}/quantity/{delta}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProductQuantity(int id, int delta)
    {
        var updatedProduct = await _productService.UpdateProductQuantityAsync(id, delta);
        return Ok(updatedProduct); 
    }
    
    [HttpPost("{productId}/add-photo")]
    public async Task<IActionResult> AddPhoto(int productId, IFormFile file)
    {
        var updatedProduct = await _productService.AddPhotoToProductAsync(productId, file);
        return Ok(updatedProduct);
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts([FromQuery] int? categoryId)
    {
        return Ok(await _productService.GetAllProductsAsync(categoryId));
    }
}
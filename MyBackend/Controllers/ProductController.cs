using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBackend.DTOs;
using MyBackend.DTOs.ProductDtos;
using MyBackend.Services;

namespace MyBackend.Controllers;

[ApiController]
[Route("api/products")]
[EnableRateLimiting("PublicApiPolicy")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
    {
        var products = await productService.GetAllProductsAsync();
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto?>> CreateProduct(CreateProductDto dto)
    {
        var newProduct = await productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto?>> UpdateProduct(int id, UpdateProductDto dto)
    {
        var updatedProduct = await productService.UpdateProductAsync(id, dto);
        return Ok(updatedProduct);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await productService.DeleteProductAsync(id);
        return NoContent();
    }
    
    [HttpPatch("{id}/quantity/{delta}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProductQuantity(int id, int delta)
    {
        var updatedProduct = await productService.UpdateProductQuantityAsync(id, delta);
        return Ok(updatedProduct); 
    }
}
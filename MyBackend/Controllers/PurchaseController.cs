using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBackend.DTOs.PurchaseDtos;
using MyBackend.Extensions;
using MyBackend.Services;

namespace MyBackend.Controllers;

[ApiController]
[Route("api/purchases")]
[Authorize]
public class PurchaseController(IPurchaseService purchaseService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseDto>> GetPurchaseById(int id)
    {
        var purchase = await purchaseService.GetPurchaseByIdAsync(id);
        return Ok(purchase);
    }
    
    [HttpPost]
    public async Task<ActionResult<PurchaseDto>> CreatePurchase([FromBody] CreatePurchaseDto dto)
    {
        int userId = User.GetUserId();
        var newPurchase = await purchaseService.CreatePurchaseAsync(userId, dto);
        return CreatedAtAction(nameof(GetPurchaseById), new { id = newPurchase.Id }, newPurchase);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<PurchaseDto>>> GetPurchasesByUserId(int userId)
    {
        var purchases = await purchaseService.GetPurchasesByUserIdAsync(userId);
        return Ok(purchases);
    }
    
    [HttpGet("my-purchases")]
    public async Task<ActionResult<List<PurchaseDto>>> GetMyPurchases()
    {
        int userId = User.GetUserId();
        var purchases = await purchaseService.GetPurchasesByUserIdAsync(userId);
        return Ok(purchases);
    }

    [NonAction]
    public Task<ActionResult<List<PurchaseDto>>> GetAllPurchases() => throw new NotImplementedException();

    [NonAction]
    public Task<IActionResult> DeletePurchase(int id) => throw new NotImplementedException();
}


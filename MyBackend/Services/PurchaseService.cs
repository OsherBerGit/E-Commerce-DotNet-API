using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.DTOs.PurchaseDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Services.Interfaces;

namespace MyBackend.Services;

public class PurchaseService(AppDbContext _context, IPurchaseMapper _mapper) : IPurchaseService
{
    public async Task<List<PurchaseDto>> GetAllPurchasesAsync() { throw new NotImplementedException(); }

    public async Task<PurchaseDto?> GetPurchaseByIdAsync(int id)
    {
        var purchase = await _context.Purchases
            .AsNoTracking()
            .Include(p => p.PurchaseProducts)
            .ThenInclude(pp => pp.Product)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (purchase is null)
            throw new KeyNotFoundException($"Purchase with ID {id} not found.");

        return _mapper.ToDto(purchase);
    }

    public async Task<List<PurchaseDto>> GetPurchasesByUserIdAsync(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        
        var purchases = await _context.Purchases
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Include(p => p.PurchaseProducts)
            .ThenInclude(pp => pp.Product)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
        
        return purchases.Select(p => _mapper.ToDto(p)!).ToList();
    }
    
    public async Task<PurchaseDto?> CreatePurchaseAsync(int userId, CreatePurchaseDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        
        var purchase = _mapper.ToEntity(dto);
        purchase.UserId = userId;
        purchase.Date = DateTime.UtcNow;

        foreach (var item in purchase.PurchaseProducts)
        {
            var existingProduct = await _context.Products.FindAsync(item.ProductId);
            if (existingProduct is null)
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");
            
            if (existingProduct.Quantity < item.Quantity)
                throw new InvalidOperationException($"Product '{existingProduct.Name}' has only {existingProduct.Quantity} left in stock.");
            
            existingProduct.Quantity -= item.Quantity;
            item.Product = existingProduct;
        }
        
        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();
        return _mapper.ToDto(purchase);
    }

    public async Task<bool> DeletePurchaseAsync(int id) => throw new NotImplementedException();
}
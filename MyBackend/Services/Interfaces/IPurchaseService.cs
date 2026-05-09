using MyBackend.DTOs.PurchaseDtos;

namespace MyBackend.Services.Interfaces;

public interface IPurchaseService
{
    Task<PurchaseDto?> CreatePurchaseAsync(int userId, CreatePurchaseDto dto);
    Task<List<PurchaseDto>> GetAllPurchasesAsync();
    Task<PurchaseDto?> GetPurchaseByIdAsync(int id);
    Task<bool> DeletePurchaseAsync(int id);
    Task<List<PurchaseDto>> GetPurchasesByUserIdAsync(int userId);
}
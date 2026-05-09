using MyBackend.DTOs.PurchaseDtos;
using MyBackend.Models;

namespace MyBackend.Mappers.Interfaces;

public interface IPurchaseMapper
{
    PurchaseDto? ToDto(Purchase? purchase);
    Purchase ToEntity(CreatePurchaseDto dto);
}
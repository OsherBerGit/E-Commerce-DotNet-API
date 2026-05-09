using MyBackend.DTOs.PurchaseDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Models;

namespace MyBackend.Mappers;

public class PurchaseMapper : IPurchaseMapper
{
    public PurchaseDto? ToDto(Purchase? purchase)
    {
        if (purchase is null) return null;
        
        var items = purchase.PurchaseProducts.Select(pp => new PurchaseItemDto
        {
            ProductId = pp.ProductId,
            ProductName = pp.Product.Name,
            Price = pp.Product.Price,
            Quantity = pp.Quantity,
            Subtotal = pp.Product.Price * pp.Quantity
        }).ToList();

        return new PurchaseDto
        {
            Id = purchase.Id,
            UserId = purchase.UserId,
            Date = purchase.Date,
            Items = items,
            Total = items.Sum(i => i.Subtotal)
        };
    }

    public Purchase ToEntity(CreatePurchaseDto dto)
    {
        return new Purchase
        {
            Date = DateTime.UtcNow,
            PurchaseProducts = dto.Items.Select(item => new PurchaseProduct
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}


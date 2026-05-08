using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyBackend.Data;
using MyBackend.DTOs;
using MyBackend.DTOs.PurchaseDtos;
using MyBackend.Mappers;
using MyBackend.Models;
using MyBackend.Services;

namespace MyBackend.Tests.Services;

public class PurchaseServiceTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IPurchaseMapper> _purchaseMapperMock;
    private readonly AppDbContext _context;
    private readonly PurchaseService _purchaseService;

    public PurchaseServiceTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _purchaseMapperMock = new Mock<IPurchaseMapper>();
        
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        
        _purchaseService = new PurchaseService(_context, _purchaseMapperMock.Object);
    }

    [Fact]
    public async Task CreatePurchaseAsync_ShouldThrowException_WhenProductIsOutOfStock()
    {
        // Arrange
        int userId = 1;
        var productId = 10;
        
        var dummyUser = new User { Id = userId, Username = "buyer", PasswordHash = "hash", Email = "buyer@test.com" };
        _context.Users.Add(dummyUser);
        
        var dummyProduct = new Product { Id = productId, Name = "Test Product", Price = 100, Quantity = 0 };
        _context.Products.Add(dummyProduct);
    
        await _context.SaveChangesAsync();
        
        var createPurchaseDto = new CreatePurchaseDto { Items = new List<CreatePurchaseItemDto> { new CreatePurchaseItemDto { ProductId = productId, Quantity = 1 } } };
        
        var fakeMappedPurchase = new Purchase { PurchaseProducts = new List<PurchaseProduct> { new PurchaseProduct { ProductId = productId, Quantity = 1 } } };
        
        _purchaseMapperMock.Setup(m => m.ToEntity(createPurchaseDto)).Returns(fakeMappedPurchase);

        // Act
        var act = async () => await _purchaseService.CreatePurchaseAsync(userId, createPurchaseDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage($"Product with ID {productId} has only 0 left.");
        
        _context.Purchases.Should().BeEmpty();
    }
    
    [Fact]
    public async Task CreatePurchaseAsync_ShouldCreatePurchaseAndReduceStock_WhenDataIsValid()
    {
        // Arrange
        int userId = 1;
        var productId = 10;
        var initialQuantity = 5;
        var quantityToBuy = 2;
    
        // 1. Add dummy user
        var dummyUser = new User { Id = userId, Username = "buyer", PasswordHash = "hash", Email = "buyer@test.com" };
        _context.Users.Add(dummyUser);

        // 2. Add product with enough stock
        var dummyProduct = new Product { Id = productId, Name = "Test Product", Price = 100, Quantity = initialQuantity };
        _context.Products.Add(dummyProduct);
    
        await _context.SaveChangesAsync();

        // 3. Prepare the request DTO
        var createPurchaseDto = new CreatePurchaseDto 
        { 
            Items = new List<CreatePurchaseItemDto> { new CreatePurchaseItemDto { ProductId = productId, Quantity = quantityToBuy } } 
        };

        // 4. Setup Mapper to return a valid Purchase entity
        var expectedPurchaseEntity = new Purchase { PurchaseProducts = new List<PurchaseProduct> { new PurchaseProduct { ProductId = productId, Quantity = quantityToBuy } } };
    
        _purchaseMapperMock.Setup(m => m.ToEntity(createPurchaseDto)).Returns(expectedPurchaseEntity);

        // Setup Mapper to return a DTO at the end of the service execution
        _purchaseMapperMock.Setup(m => m.ToDto(It.IsAny<Purchase>())).Returns(new PurchaseDto { Id = 1 });

        // Act
        var result = await _purchaseService.CreatePurchaseAsync(userId, createPurchaseDto);

        // Assert
        result.Should().NotBeNull();
    
        // Verify the purchase was saved to the database
        _context.Purchases.Should().HaveCount(1);
    
        // Verify the business logic: Stock must be reduced!
        var updatedProduct = await _context.Products.FindAsync(productId);
        updatedProduct!.Quantity.Should().Be(initialQuantity - quantityToBuy);
    }
}
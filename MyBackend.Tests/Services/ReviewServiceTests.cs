using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyBackend.Data;
using MyBackend.DTOs.ReviewDtos;
using MyBackend.Mappers;
using MyBackend.Models;
using MyBackend.Services;

namespace MyBackend.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IReviewMapper> _reviewMapperMock;
    private readonly AppDbContext _context;
    private readonly ReviewService _reviewService;
    
    public ReviewServiceTests()
    {
        // 1. Initialize Mocks
        _reviewMapperMock = new Mock<IReviewMapper>();

        // 2. Setup InMemory Database (Unique instance per test)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        // 3. Create the System Under Test (SUT)
        _reviewService = new ReviewService(_context, _reviewMapperMock.Object);
    }
    
    [Fact]
    public async Task CreateReviewAsync_ShouldThrowException_WhenUserAlreadyReviewedProduct()
    {
        // Arrange
        int userId = 1;
        int productId = 10;

        // Add a dummy user
        _context.Users.Add(new User { Id = userId, Username = "reviewer", PasswordHash = "hash", Email = "test@test.com" });
        
        // Add a dummy product
        _context.Products.Add(new Product { Id = productId, Name = "Laptop", Price = 1000, Quantity = 5 });
        
        // Add an EXISTING review for this specific user and product
        _context.ProductReviews.Add(new ProductReview 
        { 
            Id = 1, 
            UserId = userId, 
            ProductId = productId, 
            Rating = 5, 
            Comment = "Amazing!" 
        });
        
        await _context.SaveChangesAsync();

        var createReviewDto = new CreateReviewDto { ProductId = productId, Rating = 4, Comment = "Trying to review again" };

        // Act
        var act = async () => await _reviewService.CreateReviewAsync(userId, createReviewDto);

        // Assert
        // We expect the specific exception written in your service logic
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User has already reviewed this product");
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldCreateReview_WhenDataIsValid()
    {
        // Arrange
        int userId = 1;
        int productId = 10;

        // Add a dummy user and product (NO existing reviews this time)
        _context.Users.Add(new User { Id = userId, Username = "reviewer", PasswordHash = "hash", Email = "test@test.com" });
        _context.Products.Add(new Product { Id = productId, Name = "Laptop", Price = 1000, Quantity = 5 });
        await _context.SaveChangesAsync();

        var createReviewDto = new CreateReviewDto { ProductId = productId, Rating = 5, Comment = "Perfect!" };

        // Setup the Mapper Mock to return a valid ProductReview entity instead of null
        var mappedEntity = new ProductReview { ProductId = productId, Rating = 5, Comment = "Perfect!" };
        _reviewMapperMock.Setup(m => m.ToEntity(createReviewDto)).Returns(mappedEntity);

        // Setup the Mapper Mock to return a DTO at the end
        _reviewMapperMock.Setup(m => m.ToDto(It.IsAny<ProductReview>()))
            .Returns(new ReviewDto { Id = 1, ProductId = productId, Rating = 5, Comment = "Perfect!" });

        // Act
        var result = await _reviewService.CreateReviewAsync(userId, createReviewDto);

        // Assert
        result.Should().NotBeNull();
        result!.Rating.Should().Be(5);
        
        // Verify the review was successfully saved to the database
        _context.ProductReviews.Should().HaveCount(1);
    }
}
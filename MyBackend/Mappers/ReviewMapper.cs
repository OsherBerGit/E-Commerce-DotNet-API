using MyBackend.DTOs.ReviewDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Models;

namespace MyBackend.Mappers;

public class ReviewMapper : IReviewMapper
{
    public ReviewDto? ToDto(ProductReview? review)
    {
        if (review is null) return null;
        
        return new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            Username = review.User?.Username ?? "Unknown User"
        };
    }

    public ProductReview ToEntity(CreateReviewDto dto)
    {
        return new ProductReview
        {
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };
    }

    public void UpdateEntity(UpdateReviewDto dto, ProductReview review)
    {
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.UpdatedAt = DateTime.UtcNow;
    }
}
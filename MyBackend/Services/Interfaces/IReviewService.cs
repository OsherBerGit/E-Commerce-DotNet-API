using MyBackend.DTOs.ReviewDtos;

namespace MyBackend.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewDto?> CreateReviewAsync(int userId, CreateReviewDto dto);
    Task<List<ReviewDto>> GetAllReviewsAsync();
    Task<ReviewDto?> GetReviewByIdAsync(int id);
    Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);
    Task<List<ReviewDto>> GetReviewsByUserIdAsync(int userId);
    Task<ReviewDto?> UpdateReviewAsync(int userId, int id, UpdateReviewDto dto);
    Task<bool> DeleteReviewAsync(int userId, int id);
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyBackend.DTOs.ReviewDtos;
using MyBackend.Extensions;
using MyBackend.Services;
using MyBackend.Services.Interfaces;

namespace MyBackend.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewController(IReviewService _reviewService) : ControllerBase
{
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ReviewDto>> GetReviewById(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        return Ok(review);
    }
    
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto dto)
    {
        var newReview = await _reviewService.CreateReviewAsync(User.GetUserId(), dto);
        return CreatedAtAction(nameof(GetReviewById), new { id = newReview.Id }, newReview);
    }
    
    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetReviewsByProductId(int productId)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
        return Ok(reviews);
    }

    [HttpGet("my-reviews")]
    public async Task<ActionResult<List<ReviewDto>>> GetReviewsByUserId()
    {
        var reviews = await _reviewService.GetReviewsByUserIdAsync(User.GetUserId());
        return Ok(reviews);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ReviewDto?>> UpdateReview(int id, UpdateReviewDto dto)
    {
        var updatedReview = await _reviewService.UpdateReviewAsync(User.GetUserId(), id, dto);
        return Ok(updatedReview);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteReviewAsync(User.GetUserId(), id);
        return NoContent();
    }
    
    [NonAction]
    public Task<ActionResult<List<ReviewDto>>> GetAllReviews() => throw new NotImplementedException();
}
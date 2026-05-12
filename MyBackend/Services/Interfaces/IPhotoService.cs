using CloudinaryDotNet.Actions;
using MyBackend.DTOs;

namespace MyBackend.Services.Interfaces;

public interface IPhotoService
{
    Task<PhotoResponseDto> AddPhotoAsync(IFormFile file);
    Task DeletePhotoAsync(string publicId);
}
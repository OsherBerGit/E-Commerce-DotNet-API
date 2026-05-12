using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using MyBackend.DTOs;
using MyBackend.Services.Interfaces;
using MyBackend.Settings;

namespace MyBackend.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    
    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }
    
    public async Task<PhotoResponseDto> AddPhotoAsync(IFormFile file)
    {
        if (file.Length == 0) 
            throw new ArgumentException("File is empty.");
        
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
        };
        
        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null)
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

        return new PhotoResponseDto(result.SecureUrl.AbsoluteUri, result.PublicId);;
    }
    
    public async Task DeletePhotoAsync(string publicId)
    {
        var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
        if (result.Error != null)
            throw new Exception($"Cloudinary deletion failed: {result.Error.Message}");
    }
}
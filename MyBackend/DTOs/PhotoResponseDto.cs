namespace MyBackend.DTOs;

public class PhotoResponseDto
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    
    public PhotoResponseDto(string url, string publicId)
    {
        Url = url;
        this.PublicId = publicId;
    }
}
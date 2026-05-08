using System.ComponentModel.DataAnnotations;

namespace MyBackend.Models;

public class BlacklistedToken
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public DateTime Expiration { get; set; }
}
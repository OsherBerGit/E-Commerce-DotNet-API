using System.ComponentModel.DataAnnotations;

namespace MyBackend.DTOs;

public class AuthenticationRequest
{
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
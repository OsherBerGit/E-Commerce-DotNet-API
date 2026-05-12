using MyBackend.Models;

namespace MyBackend.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
    public RefreshToken GenerateRefreshToken(User user);
}
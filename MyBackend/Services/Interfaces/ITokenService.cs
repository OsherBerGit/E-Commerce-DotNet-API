using MyBackend.Models;

namespace MyBackend.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
    public RefreshToken GenerateRefreshToken(User user);
}
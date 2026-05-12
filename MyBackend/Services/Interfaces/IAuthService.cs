using MyBackend.DTOs;
using MyBackend.DTOs.UserDtos;
using MyBackend.Models;

namespace MyBackend.Services.Interfaces;

public interface IAuthService
{
    Task<User> RegisterUserAsync(CreateUserDto request);
    Task<AuthenticationResponse> LoginUserAsync(AuthenticationRequest request);
    Task<AuthenticationResponse> RefreshTokenAsync();
    Task RevokeTokenAsync();
    Task<AuthenticationResponse> LoginWithGoogleAsync(string idToken);
}
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.DTOs;
using MyBackend.DTOs.UserDtos;
using MyBackend.Exceptions;
using MyBackend.Models;

namespace MyBackend.Services;

public class AuthService(AppDbContext context, ITokenService _tokenService, IHttpContextAccessor _httpContextAccessor) : IAuthService
{
    public async Task<User?> RegisterUserAsync(CreateUserDto request)
    {
        if (await context.Users.AnyAsync(u => u.Username == request.Username))
            throw new UserAlreadyExistsException("Username is already taken");
        
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        return user;
    }

    public async Task<AuthenticationResponse?> LoginUserAsync(AuthenticationRequest request)
    {
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");
        
        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);
        
        user.RefreshTokens.Add(refreshToken); 
        await context.SaveChangesAsync();
        
        SetRefreshTokenCookie(refreshToken);
        
        return new AuthenticationResponse() { AccessToken = accessToken, };
    }

    public async Task<AuthenticationResponse?> RefreshTokenAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(token))
            throw new UnauthorizedAccessException("Refresh token is missing");
        
        var user = await context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user is null)
            throw new UnauthorizedAccessException("Invalid token");
        
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        
        if (!refreshToken.IsActive)
            throw new UnauthorizedAccessException("Token is expired or revoked");
        
        refreshToken.Revoked = DateTime.UtcNow;
        
        var newAccessToken = _tokenService.CreateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user);

        user.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync();

        SetRefreshTokenCookie(newRefreshToken);

        return new AuthenticationResponse { AccessToken = newAccessToken };
    }

    public async Task<bool> RevokeTokenAsync()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        
        if (string.IsNullOrEmpty(token)) return false;
        
        var user = await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
        
        if (user is null) return false;
        
        var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == token);
        if (refreshToken is null || !refreshToken.IsActive) return false;
        
        refreshToken.Revoked = DateTime.UtcNow;
        await context.SaveChangesAsync();
        
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken");
        
        return true;
    }
    
    private void SetRefreshTokenCookie(RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshToken.Expires,
            Path = "/api/auth"
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }
}
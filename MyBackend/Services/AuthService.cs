using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.DTOs;
using MyBackend.DTOs.UserDtos;
using MyBackend.Exceptions;
using MyBackend.Models;
using MyBackend.Services.Interfaces;

namespace MyBackend.Services;

public class AuthService(AppDbContext context, ITokenService _tokenService, IHttpContextAccessor _httpContextAccessor, ITokenBlacklistService _tokenBlacklistService, IFirebaseAuthService _firebaseAuthService) : IAuthService
{
    public async Task<User> RegisterUserAsync(CreateUserDto request)
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

    public async Task<AuthenticationResponse> LoginUserAsync(AuthenticationRequest request)
    {
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");
        
        var accessToken = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);
        
        user.RefreshTokens.Add(refreshToken); 
        await context.SaveChangesAsync();
        
        SetRefreshTokenCookie(refreshToken);
        
        return new AuthenticationResponse() { AccessToken = accessToken, };
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync()
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
        
        var newAccessToken = _tokenService.GenerateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user);

        user.RefreshTokens.Add(newRefreshToken);
        await context.SaveChangesAsync();

        SetRefreshTokenCookie(newRefreshToken);

        return new AuthenticationResponse { AccessToken = newAccessToken };
    }

    public async Task RevokeTokenAsync()
    {
        var refreshTokenString = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        var accessToken = authHeader?.Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(accessToken))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(accessToken);
                var expiry = jwtToken.ValidTo;
                
                await _tokenBlacklistService.BlacklistTokenAsync(accessToken, expiry);
            }
            catch (Exception) { }
        }
        
        if (string.IsNullOrEmpty(refreshTokenString)) return;
    
        var user = await context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshTokenString));
    
        if (user is null) return;
        
        var refreshTokenEntity = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshTokenString);
        if (refreshTokenEntity is null || !refreshTokenEntity.IsActive) return;
    
        refreshTokenEntity.Revoked = DateTime.UtcNow;
        await context.SaveChangesAsync();
        
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken");
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
    
    public async Task<AuthenticationResponse> LoginWithGoogleAsync(string idToken)
    {
        string email = await _firebaseAuthService.VerifyTokenAsync(idToken);
    
        var user = await context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email);
    
        if (user is null)
        {
            user = new User
            {
                Email = email,
                Username = email,
                PasswordHash = "EXTERNAL_AUTH"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        
        var accessToken = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        user.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();
    
        SetRefreshTokenCookie(refreshToken);

        return new AuthenticationResponse { AccessToken = accessToken };
    }
}
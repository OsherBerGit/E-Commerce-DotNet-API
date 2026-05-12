using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyBackend.DTOs;
using MyBackend.DTOs.UserDtos;
using MyBackend.Services;
using MyBackend.Services.Interfaces;

namespace MyBackend.Controllers;

[ApiController]
[EnableRateLimiting("AuthPolicy")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserDto request)
    {
        var user = await _authService.RegisterUserAsync(request);
        return Ok(new { message = "User registered successfully", userId = user.Id });
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest authenticationRequest)
    {
        var result = await _authService.LoginUserAsync(authenticationRequest);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthenticationResponse>> RefreshToken()
    {
        var result = await _authService.RefreshTokenAsync();
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.RevokeTokenAsync(); 
        return NoContent();
    }
    
    [HttpPost("google-login")]
    public async Task<ActionResult<AuthenticationResponse>> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        var response = await _authService.LoginWithGoogleAsync(dto.IdToken);
        return Ok(response);
    }
}
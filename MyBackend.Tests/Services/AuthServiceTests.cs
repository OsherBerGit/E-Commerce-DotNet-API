using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyBackend.Data;
using MyBackend.DTOs;
using MyBackend.Models;
using MyBackend.Services;
using MyBackend.Services.Interfaces;

namespace MyBackend.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextMock;
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    
    public AuthServiceTests()
    {
        // 1. Mock Dependencies
        _tokenServiceMock = new Mock<ITokenService>();
        _httpContextMock = new Mock<IHttpContextAccessor>();

        // 2. Setup In-Memory Database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        
        // 3. Create Service
        _authService = new AuthService(_context, _tokenServiceMock.Object, _httpContextMock.Object);
    }

    [Fact]
    public async Task LoginUserAsync_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var user = new User 
        { 
            Username = username, 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword") 
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new AuthenticationRequest { Username = username, Password = password };

        // Act
        var act = async () => await _authService.LoginUserAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }
    
    [Fact]
    public async Task LoginUserAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        // Arrange
        var username = "osher";
        var password = "password123";
        var user = new User 
        { 
            Id = 1, 
            Username = username, 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Roles = new List<Role> { new Role { Rolename = "User" } }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");
    
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken(It.IsAny<User>()))
            .Returns(new RefreshToken { Token = "fake-refresh", Expires = DateTime.UtcNow.AddDays(7) });

        var loginRequest = new AuthenticationRequest { Username = username, Password = password };

        // Act
        var result = await _authService.LoginUserAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("fake-jwt-token");
        
        _tokenServiceMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Once);
    }
}
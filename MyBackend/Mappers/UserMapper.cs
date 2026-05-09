using MyBackend.DTOs.UserDtos;
using MyBackend.Mappers.Interfaces;
using MyBackend.Models;

namespace MyBackend.Mappers;

public class UserMapper : IUserMapper
{
    public UserDto? ToDto(User? user)
    { 
        if (user is null) return null;
        
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = user.Roles?
                .Select(ur => ur.Rolename)
                .ToList() ?? new List<string>()
        };
    }

    public User ToEntity(CreateUserDto dto, string hashedPassword)
    {
        if (dto is null)
            throw new ArgumentNullException(nameof(dto), "Cannot convert null DTO to User entity");
        
        return new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = hashedPassword
        };
    }
}
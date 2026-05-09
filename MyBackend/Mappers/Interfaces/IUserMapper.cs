using MyBackend.DTOs.UserDtos;
using MyBackend.Models;

namespace MyBackend.Mappers.Interfaces;

public interface IUserMapper
{
    UserDto? ToDto(User? user);
    User ToEntity(CreateUserDto dto, string hashedPassword);
}
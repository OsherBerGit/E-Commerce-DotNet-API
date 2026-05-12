using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.DTOs.UserDtos;
using MyBackend.Exceptions;
using MyBackend.Mappers.Interfaces;
using MyBackend.Models;
using MyBackend.Services.Interfaces;

namespace MyBackend.Services;

public class UserService(AppDbContext _context, IUserMapper _mapper) : IUserService
{
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .Select(u => _mapper.ToDto(u)!)
            .ToListAsync();
    }
    
    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user is null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        return _mapper.ToDto(user);
    }
    
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Username == dto.Username);
        if (existingUser)
            throw new UserAlreadyExistsException("Username is already taken.");
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        
        var user = _mapper.ToEntity(dto, hashedPassword);

        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Rolename == "User");
        if (defaultRole is not null)
            user.Roles = new List<Role> { defaultRole };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return _mapper.ToDto(user)!;
    }
    
    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user is null)
            throw new KeyNotFoundException($"User with ID {id} not found.");
        
        user.Email = dto.Email ?? user.Email;

        await _context.SaveChangesAsync();
        return _mapper.ToDto(user);
    }
    
    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {id} not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}

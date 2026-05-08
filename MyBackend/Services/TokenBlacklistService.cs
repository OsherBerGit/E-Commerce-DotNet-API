using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Models;

namespace MyBackend.Services;

public class TokenBlacklistService(AppDbContext _context) : ITokenBlacklistService
{
    public async Task BlacklistTokenAsync(string token, DateTime expiry)
    {
        _context.BlacklistedTokens.Add(new BlacklistedToken { Token = token, Expiration = expiry });
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _context.BlacklistedTokens.AnyAsync(t => t.Token == token);
    }
}
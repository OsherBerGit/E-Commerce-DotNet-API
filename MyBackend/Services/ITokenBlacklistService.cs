namespace MyBackend.Services;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string token, DateTime expiresAt);
    Task<bool> IsTokenBlacklistedAsync(string token);
}
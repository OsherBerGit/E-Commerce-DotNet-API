namespace MyBackend.Services.Interfaces;

public interface IFirebaseAuthService
{
    Task<string> VerifyTokenAsync(string idToken);
}
using FirebaseAdmin.Auth;
using MyBackend.Services.Interfaces;

namespace MyBackend.Services;

public class FirebaseAuthService : IFirebaseAuthService
{
    public async Task<string> VerifyTokenAsync(string idToken)
    {
        try
        { 
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken); 
            return decodedToken.Claims["email"].ToString()!;
        }
        catch (Exception e)
        {
            throw new UnauthorizedAccessException("Invalid or expired Firebase token.");
        }
    }
}
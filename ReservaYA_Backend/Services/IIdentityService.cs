using ReservaYA_Backend.ResponseModels;

namespace ReservaYA_Backend.Services
{
    public interface IIdentityService
    {
        Task<string> RegisterAsync(string userId);
        Task<AuthenticationResult> LoginAsync(string userName, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refresToken);
        Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);
    }
}

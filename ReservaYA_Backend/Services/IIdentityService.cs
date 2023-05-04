using ReservaYA_Backend.ResponseModels;

namespace ReservaYA_Backend.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string mail, string password);
        Task<AuthenticationResult> LoginAsync(string userName, string password);
        Task<bool> ChangePassword(string mail, string oldPassword, string newPassword);
    }
}

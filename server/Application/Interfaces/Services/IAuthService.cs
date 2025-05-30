using Application.DTO;
using Persistence.Models;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterUserRequest registerUser, CancellationToken cancellationToken);
    Task<User> LoginAsync(LoginUserRequest loginUserRequest, CancellationToken cancellationToken);
    Task ChangePasswordAsync(string username, string currentPassword, string newPassword, CancellationToken cancellationToken);
    Task<User> GoogleSignInAsync(GoogleSignInRequest googleSignInRequest, CancellationToken cancellationToken);
}
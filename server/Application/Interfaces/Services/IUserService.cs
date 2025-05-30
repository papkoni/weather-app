using Application.DTO;
using Persistence.Models;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<string> GetUserEmailAsync(string username, CancellationToken cancellationToken);
    Task UpdateUsernameAsync(string email, string newUsername, CancellationToken cancellationToken);
    Task<List<UsersDto>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Guid> GetUserIdAsync(string username, CancellationToken cancellationToken);
    Task SetSelectedCityAsync(Guid userId, int cityId, CancellationToken cancellationToken);
    Task AddCityToUserAsync(Guid userId, int cityId, string cityName, CancellationToken cancellationToken);
    Task<List<UserCity>> GetAddedCitiesAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteCityFromUserAsync(Guid userId, int cityId, CancellationToken cancellationToken);
    Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}
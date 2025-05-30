using Persistence.Models;

namespace Persistence.Interfaces.Repositories;

public interface IUserCityRepository
{
    Task<UserCity?> GetSCityByNameAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(UserCity userCity, CancellationToken cancellationToken);
    Task<List<UserCity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<UserCity>> GetAllAsync(CancellationToken cancellationToken);
    void Update(UserCity userCity);
    void Delete(UserCity userCity);
    Task<UserCity?> GetGetByUserIdAndCityIdAsync(Guid userId, int cityId, CancellationToken cancellationToken);
    Task<UserCity?> GetSelectedCityAsync(Guid userId, CancellationToken cancellationToken);
}
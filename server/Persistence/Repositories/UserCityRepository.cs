using Microsoft.EntityFrameworkCore;
using Persistence.Interfaces.Repositories;
using Persistence.Models;

namespace Persistence.Repositories;

public class UserCityRepository(AppDbContext context): IUserCityRepository
{
    public async Task AddAsync(UserCity userCity, CancellationToken cancellationToken)
    {
        await context.UserCities.AddAsync(userCity, cancellationToken);
    }
    
    // public async Task<UserCity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    // {
    //     return await context.UserCities
    //         .Where(uc => uc.UserId == userId)
    //         .Select(uc => new 
    //         {
    //             CityName = uc.CityName,
    //             IsSelected = uc.IsSelected
    //         })
    //         .ToListAsync(cancellationToken);
    // }

    public async Task<UserCity?> GetGetByUserIdAndCityIdAsync(Guid userId, int cityId, CancellationToken cancellationToken)
    {
        return await context.UserCities.
            FirstOrDefaultAsync(
                us => us.UserId == userId 
                                      && 
                                      us.CityId == cityId, 
                cancellationToken);
    }
    
    public async Task<List<UserCity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.UserCities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public void Update(UserCity userCity)
    {
        context.UserCities.Update(userCity);
    }
    
    public void Delete(UserCity userCity)
    {
        context.UserCities.Remove(userCity);
    }
    
    public async Task<List<UserCity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.UserCities
            .AsNoTracking()
            .Where(uc => uc.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<UserCity?> GetSelectedCityAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.UserCities
            .AsNoTracking()
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.IsSelected == "1", cancellationToken);
    }
    
    public async Task<UserCity?> GetSCityByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await context.UserCities
            .AsNoTracking()
            .FirstOrDefaultAsync(uc => uc.CityName == name, cancellationToken);
    }
}
using Microsoft.EntityFrameworkCore;
using Persistence.Models;

namespace Persistence.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserCity> UserCities { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
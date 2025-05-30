using Microsoft.EntityFrameworkCore;
using Persistence.Interfaces.Repositories;
using Persistence.Models;

namespace Persistence.Repositories;

public class UserRepository(AppDbContext context): IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }
    
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return  await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
    public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return  await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == name, cancellationToken);
    }
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
    
    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    
    public void Update(User user)
    {
        context.Users.Update(user);
    }
    
    public void Delete(User user)
    {
        context.Users.Remove(user);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Persistence.Interfaces;
using Persistence.Interfaces.Repositories;
using Persistence.Repositories;

namespace Application.Extensions;

public static class PersistenceExtension
{
    public static void AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserCityRepository, UserCityRepository>();
        services.AddDbContext<IAppDbContext, AppDbContext>(
            options =>
            {
                options.UseNpgsql(configuration.GetConnectionString(nameof(AppDbContext)));
            }
        );
    }
}
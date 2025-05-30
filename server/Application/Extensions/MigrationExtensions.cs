using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Microsoft.AspNetCore.Builder;
namespace Application.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateAsyncScope();

        using AppDbContext libraryDbContext =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();

        libraryDbContext.Database.Migrate();
    }
}
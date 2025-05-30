using API.Middleware;
using Application.Extensions;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddHttpContextAccessor(); 

        builder.Services.AddControllers();
        
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddPersistence(builder.Configuration);      
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        app.UseMiddleware<ExceptionMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.ApplyMigrations();

        app.UseAuthorization();

        app.MapControllers();
        
        app.Run();
    }
}
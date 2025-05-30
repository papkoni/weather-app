using Application.Exceptions;
using Application.Interfaces.Services;

namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
//[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        this.weatherService = weatherService;
    }

    [HttpGet("/weatherChart")]
    public async Task<IActionResult> GetWeatherChart([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var weatherChart = await weatherService.GetWeatherChartAsync(userId, cancellationToken);
        return Ok(new { message = "Success", data = weatherChart });
    }
    
    [HttpGet("/weather")]
    public async Task<IActionResult> GetWeather([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var weather = await weatherService.GetWeatherDataAsync(userId, cancellationToken);
        
        return Ok(weather);
    }
    
    [HttpGet("/dailyWeather")]
    public async Task<IActionResult> GetDailyWeather([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var weather = await weatherService.GetDailyWeatherAsync(userId, cancellationToken);
        
        return Ok(weather);
    }
}

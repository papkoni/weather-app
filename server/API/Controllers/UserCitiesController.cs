using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Persistence.Interfaces.Repositories;

namespace API.Controllers;

[ApiController]
//[Route("api/user-cities/")]
public class UserCitiesController(
    IUserService userService,
    IWeatherService weatherService,
    IUserCityRepository userCityRepository): ControllerBase
{
    [HttpPost("/addCityToUser")]
    public async Task<IActionResult> AddCityToUser([FromBody] AddCityDto dto, CancellationToken cancellationToken)
    {
        await userService.AddCityToUserAsync(dto.UserId, dto.CityId, dto.CityName, cancellationToken);
        return Ok(new { message = "City added successfully" });
    }

    [HttpGet("getAddedCities/{userId:guid}")]
    public async Task<IActionResult> GetAddedCities(Guid userId, CancellationToken cancellationToken)
    {
        var cities = await userService.GetAddedCitiesAsync(userId, cancellationToken);
        return Ok(cities);
    }

    [HttpDelete("/deleteCityFromUser")]
    public async Task<IActionResult> DeleteCity([FromBody] DeleteCityDto dto, CancellationToken cancellationToken)
    {
        await userService.DeleteCityFromUserAsync(dto.UserId, dto.CityId, cancellationToken);
        return Ok(new { message = "City deleted successfully" });
    }

    [HttpPost("/setSelectedCity")]
    public async Task<IActionResult> SetSelectedCity([FromBody] SelectCityDto dto, CancellationToken cancellationToken)
    {
        await userService.SetSelectedCityAsync(dto.UserId, dto.CityId, cancellationToken);
        return Ok(new { message = "Selected city updated successfully" });
    }
    
    [HttpGet("/userCities")]
    public async Task<IActionResult> GetUserCities([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var userCities = await userService.GetAddedCitiesAsync(userId, cancellationToken);

        var tasks = userCities.Select(async city =>
        {
            var weatherInfo = await weatherService.GetCurrentWeatherAsync(city.CityName, city.CityId, cancellationToken);
            weatherInfo.IsSelected = city.IsSelected;
            return weatherInfo;
        });

        var weatherData = await Task.WhenAll(tasks);

        return Ok(new { message = "Success", data = weatherData });
    }
    
    [HttpGet("/guestWeather")]
    public async Task<IActionResult> GetGuest([FromQuery] string cityName, CancellationToken cancellationToken)
    {
        var weatherInfo = await weatherService.GetDailyWeatherByCityAsync(cityName, cancellationToken);
        
        return Ok(weatherInfo);
    }
}

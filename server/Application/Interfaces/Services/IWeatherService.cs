using Application.DTO.Weather;

namespace Application.Interfaces.Services;

public interface IWeatherService
{
    Task<List<DailyWeatherResponse>> GetDailyWeatherByCityAsync(string cityName, CancellationToken cancellationToken);
    Task<WeatherInfoDto> GetCurrentWeatherAsync(string cityName, int cityId, CancellationToken cancellationToken);
    Task<WeatherChartResponse> GetWeatherChartAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<WeatherModel>> GetWeatherDataAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<DailyWeatherResponse>> GetDailyWeatherAsync(Guid userId, CancellationToken cancellationToken);
}
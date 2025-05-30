namespace Application.DTO.Weather;

public class WeatherForecastDto
{
    public string CityName { get; set; }
    public List<DailyForecastDto> DailyForecasts { get; set; }
}
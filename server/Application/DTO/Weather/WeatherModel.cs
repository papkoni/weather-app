namespace Application.DTO.Weather;

public class WeatherModel
{
    public string Name { get; set; }
    public List<DailyForecastDto> WeeklyWeather { get; set; }
}


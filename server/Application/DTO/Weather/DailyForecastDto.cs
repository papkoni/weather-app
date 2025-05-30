namespace Application.DTO.Weather;

public class DailyForecastDto
{
    public string MainImg { get; set; }
    public string MainTemp { get; set; }
    public string MainWind { get; set; }
    public string MainHumidity { get; set; }
    public HourlyForecast AllTime { get; set; }
}
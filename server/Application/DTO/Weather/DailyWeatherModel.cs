namespace Application.DTO.Weather;

public class DailyWeatherModel
{
    public string Date { get; set; }
    public string AvgTemp { get; set; }
    public string Condition { get; set; }
    public string Icon { get; set; }
    public string WindKph { get; set; }
    public string Humidity { get; set; }
}
namespace Application.DTO.Weather;

public class HourlyForecast
{
    public List<string> Img { get; set; }
    public List<string> Temps { get; set; }
    public List<string> Hour { get; set; }
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.DTO.Weather;

public class DailyWeatherResponse
{
    [JsonPropertyName("week_weather")]
    public List<WeekWeather> WeekWeather { get; set; }

    [JsonPropertyName("day_weather")]
    public List<DayWeather> DayWeather { get; set; }
}

public class WeekWeather
{
    [JsonPropertyName("main_img")]
    public string MainImg { get; set; }

    [JsonPropertyName("main_temp")]
    public string MainTemp { get; set; }
}

public class DayWeather
{
    [JsonPropertyName("all_time")]
    public HourlyWeather AllTime { get; set; }
}

public class HourlyWeather
{
    [JsonPropertyName("img")]
    public List<string> Img { get; set; }

    [JsonPropertyName("temps")]
    public List<string> Temps { get; set; }

    [JsonPropertyName("hour")]
    public List<string> Hour { get; set; }
}

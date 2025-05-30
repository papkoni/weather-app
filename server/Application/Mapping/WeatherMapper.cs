using Application.DTO.Weather;

namespace Application.Mapping;

public static class WeatherMapper
{
    public static List<WeatherModel> MapToModel(string cityName, WeatherApiResponse2 api)
    {
        var model = new WeatherModel
        {
            Name = cityName,
            WeeklyWeather = api.forecast.forecastday.Select(day => new DailyForecastDto
            {
                MainImg = day.day.condition.Icon,
                MainTemp = day.day.avgtemp_c.ToString("0.0"),
                MainWind = day.day.maxwind_kph.ToString("0.0"),
                MainHumidity = day.day.avghumidity.ToString("0.0"),
                AllTime = new HourlyForecast
                {
                    Img = day.hour.Select(h => h.condition.Icon).ToList(),
                    Temps = day.hour.Select(h => h.temp_c.ToString("0.0")).ToList(),
                    Hour = day.hour.Select(h => DateTime.Parse(h.time).ToString("HH:mm")).ToList()
                }
            }).ToList()
        };

        return new List<WeatherModel> { model };
    }
}

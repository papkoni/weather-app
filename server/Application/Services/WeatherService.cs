using System.Net.Http.Json;
using System.Text.Json;
using Application.DTO.Weather;
using Application.Exceptions;
using Application.Interfaces.Services;
using Application.Mapping;
using Persistence.Interfaces.Repositories;

namespace Application.Services;

public class WeatherService(HttpClient httpClient, IUserCityRepository userCityRepository): IWeatherService
{
    private readonly string _apiKey = "4f432461d44a47c085f92809251005"; 

    public async Task<WeatherInfoDto> GetCurrentWeatherAsync(string cityName, int cityId, CancellationToken cancellationToken)
    {
        var url = $"https://api.weatherapi.com/v1/current.json?key={_apiKey}&q={cityName}&aqi=no";

        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Weather API error: {response.StatusCode}");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        Console.WriteLine(json);

        return new WeatherInfoDto
        {
            
            CityId = cityId,
            CityName = weatherData.Location.Name,
            Coordinates = new CoordinatesDto
            (
                weatherData.Location.Lon,
                weatherData.Location.Lat
            ),
            Weather = new WeatherDto
            (
                weatherData.Current.Temp_C,
                weatherData.Current.Condition.Text,
                weatherData.Current.Condition.Icon
            )
        };
    } 
    
    
    public async Task<WeatherChartResponse> GetWeatherChartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var city = await userCityRepository.GetSelectedCityAsync(userId, cancellationToken);
        if (city == null)
            throw new KeyNotFoundException("City not found");

        var uri = $"http://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={city.CityName}&days=7&aqi=no&alerts=no";
        var response = await httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch weather");

        var json = await response.Content.ReadAsStringAsync();
        var apiData = JsonSerializer.Deserialize<WeatherApiResponse2>(json);

        var daily = apiData.forecast.forecastday.Select(day => new DailyForecast
        {
            Date = day.date,
            Temperature = new TemperatureData
            {
                Day = day.day.avgtemp_c,
                Min = day.day.mintemp_c,
                Max = day.day.maxtemp_c,
                Night = GetNightTemp(day.hour)
            },
            WindSpeed = day.day.maxwind_kph,
            Humidity = day.day.avghumidity,
            Cloudiness = day.day.daily_will_it_be_sunny == 1 ? 0 : 100, // approximation
            Pressure = 1013 // WeatherAPI doesn’t return daily pressure, optional mock
        }).ToList();

        return new WeatherChartResponse
        {
            CityName = city.CityName,
            Daily = daily
        };
    }

    private double GetNightTemp(List<Hour> hours)
    {
        // Approximate night temperature between 00:00 - 06:00
        return hours
            .Where(h => {
                var hour = DateTime.Parse(h.time).Hour;
                return hour >= 0 && hour <= 6;
            })
            .Average(h => h.temp_c);
    }

   

    public async Task<List<WeatherModel>> GetWeatherDataAsync(Guid userId, CancellationToken cancellationToken)
    {
        var city = await userCityRepository.GetSelectedCityAsync(userId, cancellationToken);
        if (city == null) throw new Exception("Selected city not found");

        var uri = $"http://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={city.CityName}&days=3&aqi=no&alerts=no";
        var response = await httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error fetching weather data");
        }

        var json = await response.Content.ReadAsStringAsync();
        var apiResult = JsonSerializer.Deserialize<WeatherApiResponse2>(json);

        return WeatherMapper.MapToModel(city.CityName, apiResult);
    }

    public async Task<List<DailyWeatherResponse>> GetDailyWeatherAsync(Guid userId, CancellationToken cancellationToken)
    {
        var city = await userCityRepository.GetSelectedCityAsync(userId, cancellationToken);
        if (city == null)
            throw new KeyNotFoundException("City not found");

        var uri = $"http://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={city.CityName}&days=7&aqi=no&alerts=no";
        var response = await httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch weather");

        var json = await response.Content.ReadAsStringAsync();
        var apiData = JsonSerializer.Deserialize<WeatherApiResponse2>(json);

        var weekWeatherList = apiData.forecast.forecastday.Select(day => new WeekWeather
        {
            MainImg = day.day.condition.Icon,
            MainTemp = day.day.avgtemp_c.ToString("0.0")
        }).ToList();

        var firstDay = apiData.forecast.forecastday[0];

        var dayWeatherList = new List<DayWeather>
        {
            new DayWeather
            {
                AllTime = new HourlyWeather
                {
                    Img = firstDay.hour.Select(h => h.condition.Icon).ToList(),
                    Temps = firstDay.hour.Select(h => h.temp_c.ToString("0.0")).ToList(),
                    Hour = firstDay.hour.Select(h => DateTime.Parse(h.time).ToString("HH:mm")).ToList()
                }
            }
        };

        return new List<DailyWeatherResponse>
        {
            new DailyWeatherResponse
            {
                WeekWeather = weekWeatherList,
                DayWeather = dayWeatherList
            }
        };
    }
    
    public async Task<List<DailyWeatherResponse>> GetDailyWeatherByCityAsync(string cityName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("City name cannot be empty", nameof(cityName));

        var uri = $"http://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={cityName}&days=7&aqi=no&alerts=no";
        var response = await httpClient.GetAsync(uri, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch weather");

        var json = await response.Content.ReadAsStringAsync();
        var apiData = JsonSerializer.Deserialize<WeatherApiResponse2>(json);

        var weekWeatherList = apiData.forecast.forecastday.Select(day => new WeekWeather
        {
            MainImg = day.day.condition.Icon,
            MainTemp = day.day.avgtemp_c.ToString("0.0")
        }).ToList();

        var firstDay = apiData.forecast.forecastday[0];

        var dayWeatherList = new List<DayWeather>
        {
            new DayWeather
            {
                AllTime = new HourlyWeather
                {
                    Img = firstDay.hour.Select(h => h.condition.Icon).ToList(),
                    Temps = firstDay.hour.Select(h => h.temp_c.ToString("0.0")).ToList(),
                    Hour = firstDay.hour.Select(h => DateTime.Parse(h.time).ToString("HH:mm")).ToList()
                }
            }
        };

        return new List<DailyWeatherResponse>
        {
            new DailyWeatherResponse
            {
                WeekWeather = weekWeatherList,
                DayWeather = dayWeatherList
            }
        };
    }
}
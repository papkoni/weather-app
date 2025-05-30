namespace Application.DTO.Weather;

public record WeatherApiResponse(
    Location Location, 
    CurrentWeather Current);

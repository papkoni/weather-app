namespace Application.DTO.Weather;

public record CurrentWeather(
    float Temp_C, 
    Condition Condition,
    double Wind_Kph,
    int Humidity);

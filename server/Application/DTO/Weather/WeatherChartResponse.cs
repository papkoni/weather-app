namespace Application.DTO.Weather;

public class WeatherChartResponse
{
    public string CityName { get; set; }
    public List<DailyForecast> Daily { get; set; }
}


public class DailyForecast
{
    public string Date { get; set; }
    public TemperatureData Temperature { get; set; }
    public double WindSpeed { get; set; }
    public double Humidity { get; set; }
    public double Cloudiness { get; set; }
    public double Pressure { get; set; }
}

public class TemperatureData
{
    public double Day { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Night { get; set; }
}
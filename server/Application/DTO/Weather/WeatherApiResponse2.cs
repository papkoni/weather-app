namespace Application.DTO.Weather;

public class WeatherApiResponse2
{
    public Forecast forecast { get; set; }
}

public class Forecast
{
    public List<ForecastDay2> forecastday { get; set; }
}

public class ForecastDay2
{
    public string date { get; set; }
    public Day day { get; set; }
    public List<Hour> hour { get; set; }
}

public class Day
{
    public double avgtemp_c { get; set; }
    public double maxwind_kph { get; set; }
    public double avghumidity { get; set; }
    public Condition condition { get; set; }
    
    public double maxtemp_c { get; set; }
    public double mintemp_c { get; set; }
    public int daily_will_it_be_sunny { get; set; }
}

public class Hour
{
    public string time { get; set; }
    public double temp_c { get; set; }
    public Condition condition { get; set; }
}


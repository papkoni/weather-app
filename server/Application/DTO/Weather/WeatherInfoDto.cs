namespace Application.DTO.Weather;

public class WeatherInfoDto
{
    public string CityName { get; set; }
    public long CityId { get; set; }
    public string IsSelected { get; set; }
    public CoordinatesDto Coordinates { get; set; }
    public WeatherDto Weather { get; set; }
}
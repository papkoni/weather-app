namespace Application.DTO.Weather;

public record Location(
    string Name,
    double Lat,
    double Lon, 
    long Localtime_Epoch);

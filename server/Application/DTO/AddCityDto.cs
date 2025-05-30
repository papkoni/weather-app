namespace Application.DTO;

public record AddCityDto(
    Guid UserId, 
    int CityId, 
    string CityName);

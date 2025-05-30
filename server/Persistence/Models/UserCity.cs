namespace Persistence.Models;

public class UserCity
{
    public UserCity(Guid userId, int cityId, string cityName, string isSelected = "0")
    {
        UserId = userId;
        CityId = cityId;
        CityName = cityName;
        IsSelected = isSelected;
    }
    
    public Guid UserId { get; set; }
    public int CityId { get; set; }
    public string CityName { get; set; }
    public string IsSelected { get; set; }
    public virtual User User { get; set; }
}
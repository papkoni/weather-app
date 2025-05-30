using Application.DTO;
using Application.Exceptions;
using Application.Interfaces.Services;
using MapsterMapper;
using Persistence.Interfaces;
using Persistence.Interfaces.Repositories;
using Persistence.Models;

namespace Application.Services;

public class UserService( 
    IUserRepository userRepository,
    IAppDbContext appDbContext,
    IUserCityRepository userCityRepository,
    IMapper mapper): IUserService
{
    public async Task<string> GetUserEmailAsync(string username, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByNameAsync(username, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return user.Email;
    }
    
    public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return user;
    }
    
    public async Task UpdateUsernameAsync(string email, string newUsername, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.Username = newUsername;
        
        userRepository.Update(user);
        
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<UsersDto>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return mapper.Map<List<UsersDto>>(users);;
    }
    
    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException("User not found");
        
        userRepository.Delete(user);
        
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<Guid> GetUserIdAsync(string username, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByNameAsync(username, cancellationToken);
        if (user == null)
            throw new NotFoundException("User not found");

        return user.Id;
    }
    
    public async Task SetSelectedCityAsync(Guid userId, int cityId, CancellationToken cancellationToken)
    {
        // Получаем все города пользователя
        var userCities = await userCityRepository.GetByUserIdAsync(userId, cancellationToken);

        if (!userCities.Any())
        {
            throw new NotFoundException("No cities found for user");
        }

        // Сбросить выбор у всех
        foreach (var city in userCities)
        {
            city.IsSelected = "0";
            userCityRepository.Update(city);

        }

        // Установить выбранный
        var selectedCity =  userCities.FirstOrDefault(c => c.CityId == cityId);
        if (selectedCity == null)
        {
            throw new NotFoundException("Selected city not found for this user");
        }

        selectedCity.IsSelected = "1";
        userCityRepository.Update(selectedCity);
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddCityToUserAsync(Guid userId, int cityId, string cityName, CancellationToken cancellationToken)
    {
        var existing = await userCityRepository.GetGetByUserIdAndCityIdAsync(userId, cityId, cancellationToken);
        if (existing != null)
        {
            throw new BadRequestException("City already added");
        }

        var userCity = new UserCity
        (
            userId,
            cityId,
            cityName
        );

        await userCityRepository.AddAsync(userCity, cancellationToken);
        
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<UserCity>> GetAddedCitiesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await userCityRepository.GetByUserIdAsync(userId, cancellationToken);
    }
    
    public async Task DeleteCityFromUserAsync(Guid userId, int cityId, CancellationToken cancellationToken)
    {
        var userCity = await userCityRepository.GetGetByUserIdAndCityIdAsync(userId, cityId, cancellationToken);
        if (userCity == null)
        {
            throw new NotFoundException("City not found for the user");
        }

        userCityRepository.Delete(userCity);
        
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
}
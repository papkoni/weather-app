using Application.DTO;
using Application.Exceptions;
using Application.Interfaces.Auth;
using Application.Interfaces.Services;
using Persistence.Interfaces;
using Persistence.Interfaces.Repositories;
using Persistence.Models;

namespace Application.Services;

public class AuthService(
    IPasswordHasher passwordHasher, 
    IUserRepository userRepository,
    IAppDbContext appDbContext): IAuthService
{
    public async Task<User> RegisterAsync(RegisterUserRequest registerUser, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByNameAsync(registerUser.Name, cancellationToken);
        if (existingUser != null)
        {
            throw new BadRequestException($"A user '{registerUser.Name}' already exists");
        }
        
        var hashedPassword = passwordHasher.Generate(registerUser.Password);
        
        var user = new User(Guid.NewGuid(), registerUser.Name, hashedPassword, registerUser.Email);
        
        await userRepository.AddAsync(user, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        return user; 
    }

    public async Task<User> LoginAsync(LoginUserRequest loginUserRequest, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByNameAsync(loginUserRequest.Name, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        
        var isPasswordValid = passwordHasher.Verify(loginUserRequest.Password, user.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid password or email");
        }
        
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        return user;
    }
    
    public async Task ChangePasswordAsync(string username, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByNameAsync(username, cancellationToken);
        if (user == null)
            throw new NotFoundException("User not found");

        var isPasswordValid = passwordHasher.Verify(currentPassword, user.Password);
        if (!isPasswordValid)
            throw new UnauthorizedException("Current password is incorrect");

        var hashedNewPassword = passwordHasher.Generate(newPassword);

        user.Password = hashedNewPassword;
        
        userRepository.Update(user);
    
        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> GoogleSignInAsync(GoogleSignInRequest googleSignInRequest, CancellationToken cancellationToken)
    {
        try
        {
            // Проверяем, существует ли пользователь с таким email
            var existingUser = await userRepository.GetByEmailAsync(googleSignInRequest.Email, cancellationToken);
            
            if (existingUser != null)
            {
                // Пользователь существует
                if (existingUser.GoogleId != null)
                {
                    // Пользователь уже связан с Google аккаунтом
                    return existingUser;
                }
                else
                {
                    // Связываем существующий аккаунт с Google
                    existingUser.GoogleId = googleSignInRequest.Sub;
                    existingUser.IsGoogleAccount = true;
                    
                    userRepository.Update(existingUser);
                    await appDbContext.SaveChangesAsync(cancellationToken);
                    
                    return existingUser;
                }
            }
            else
            {
                // Создаем нового пользователя с Google аккаунтом
                var newUser = new User(
                    Guid.NewGuid(),
                    googleSignInRequest.Username,
                    passwordHasher.Generate(googleSignInRequest.Sub), // Пароль не требуется для Google аккаунта
                    googleSignInRequest.Email,
                    googleSignInRequest.Sub,
                    true,
                    "User"
                );
                
                await userRepository.AddAsync(newUser, cancellationToken);
                await appDbContext.SaveChangesAsync(cancellationToken);
                
                return newUser;
            }
        }
        catch (Exception ex)
        {
            throw new UnauthorizedException("Google sign-in failed: " + ex.Message);
        }
    }
}
using Application.DTO;
using Application.Interfaces.Services;

namespace API.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
//[Route("api/auth/")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(new { message = "Registration successful", userId = user.Id });
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _authService.LoginAsync(request, cancellationToken);
        return Ok(new { message = "Login successful", data = new { name = user.Username, email = user.Email, password = user.Password, role = user.Role }});
    }

    [HttpPost("/changePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await _authService.ChangePasswordAsync(request.Username, request.CurrentPassword, request.NewPassword, cancellationToken);
        return Ok(new { message = "Password changed successfully" });
    }

    [HttpPost("/googleSignIn")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequest request, CancellationToken cancellationToken)
    {
        var user = await _authService.GoogleSignInAsync(request, cancellationToken);
        
        if (user.GoogleId != null && user.IsGoogleAccount)
        {
            return Ok(new { 
                message = "User logged in successfully with Google", 
                userId = user.Id, 
                username = user.Username, 
                email = user.Email 
            });
        }
        
        return BadRequest(new { message = "Google sign-in failed" });
    }
}

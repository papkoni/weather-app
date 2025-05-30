using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
//[Route("api/user/")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("/getUserEmail")]
    public async Task<IActionResult> GetEmail(string username, CancellationToken cancellationToken)
    {
        var email = await userService.GetUserEmailAsync(username, cancellationToken);
        return Ok(new { email });
    }

    [HttpPut("/updateUser")]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto, CancellationToken cancellationToken)
    {
        await userService.UpdateUsernameAsync(dto.Email, dto.Name, cancellationToken);
        return Ok(new { message = "Username updated successfully" });
    }

    [HttpGet("/users")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userService.GetAllUsersAsync(cancellationToken);
        return Ok(users); 
    }

    [HttpDelete("/users/{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        await userService.DeleteUserAsync(userId, cancellationToken);
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpGet("/getUserId")]
    public async Task<IActionResult> GetUserId([FromQuery] string username, CancellationToken cancellationToken)
    {
        var id = await userService.GetUserIdAsync(username, cancellationToken);
        return Ok(new { userId = id });
    }
}

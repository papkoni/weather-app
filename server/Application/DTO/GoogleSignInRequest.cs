using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record GoogleSignInRequest(
    [Required] string Username,
    [Required] string Email,
    [Required] string Sub); // Sub - это Google ID пользователя 
using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record RegisterUserRequest(
        [Required]string Email,
        [Required] string Password,
        [Required] string Name);
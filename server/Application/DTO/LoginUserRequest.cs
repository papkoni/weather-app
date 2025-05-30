using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public record LoginUserRequest(
    [Required]string Name,
    [Required] string Password);
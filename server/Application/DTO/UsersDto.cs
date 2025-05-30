namespace Application.DTO;

public record UsersDto(
    Guid Id,
    string Username,
    string Email);
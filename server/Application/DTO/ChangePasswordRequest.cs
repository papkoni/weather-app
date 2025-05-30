namespace Application.DTO;

public record ChangePasswordRequest(
    string Username,
    string CurrentPassword,
    string NewPassword);
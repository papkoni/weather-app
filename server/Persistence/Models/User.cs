using System;

namespace Persistence.Models;

public class User
{
    public User(Guid id, string username, string password, string email, string googleId = null, bool isGoogleAccount = false, string role = "User")
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        GoogleId = googleId;
        IsGoogleAccount = isGoogleAccount;
        Role = role;
    }
    
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string? GoogleId { get; set; }
    public bool IsGoogleAccount { get; set; } = false;
    public string Role { get; set; }
}
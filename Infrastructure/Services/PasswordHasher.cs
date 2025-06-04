namespace Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
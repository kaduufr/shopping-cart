namespace Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(UserEntity user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
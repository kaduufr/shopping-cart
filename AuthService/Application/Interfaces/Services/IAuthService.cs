namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResultDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResultDto> LoginAsync(LoginDto loginData);
}

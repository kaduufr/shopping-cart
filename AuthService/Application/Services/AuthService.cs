namespace Application.Services;

public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);

        var user = new UserEntity
        {
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            IsActive = true,
            Name = registerDto.Name
        };
        var userCreated = await _userRepository.CreateAsync(user);
        
        if (userCreated == null)
        {
            throw new InvalidOperationException("Erro ao criar usuário");
        }
        return new AuthResultDto
        {
            UserId = userCreated.Id,
        };
    }

    public Task<AuthResultDto> LoginAsync(LoginDto loginData)
    {
        throw new NotImplementedException();
    }
}
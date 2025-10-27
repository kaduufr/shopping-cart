using Application.Interfaces.Services;

namespace Application.Services;

public class AuthService: IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }
    
    public async Task<AuthResultDto> RegisterAsync(RegisterDto registerDto)
    {
        
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("E-mail já está em uso");
        
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
            throw new InvalidOperationException("Erro ao criar usuário");
        
        // Gerando tokens JWT
        string token = _tokenService.GenerateToken(userCreated);
        string refreshToken = _tokenService.GenerateRefreshToken();
        
        return new AuthResultDto
        {
            UserId = userCreated.Id,
            Token = token,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos");
        }
        
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Usuário inativo");
        }
        
        bool isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password);
        
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("E-mail ou senha inválidos");
        }
        
        string token = _tokenService.GenerateToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken();
        
        return new AuthResultDto
        {
            UserId = user.Id,
            Token = token,
            RefreshToken = refreshToken
        };
    }
}


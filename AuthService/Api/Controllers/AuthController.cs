namespace Api.Controllers;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDto> _registerDtoValidator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IValidator<RegisterDto> registerDtoValidator, 
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _registerDtoValidator = registerDtoValidator;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Credenciais inválidas");
        }

        if (request.Username == "usuario" && request.Password == "senha")
        {
            // Criar um token ou objeto de resposta
            var response = new
            {
                Token = "seu-token-jwt-aqui",
                ExpiresIn = 3600, // segundos
                UserName = request.Username
            };

            return Ok(response);
        }

        return Unauthorized("Usuário ou senha incorretos");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var validation = _registerDtoValidator.Validate(registerDto);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
        }

        var result = await _authService.RegisterAsync(registerDto);
        return Ok(result);
    }
}
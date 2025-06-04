namespace Api.Controllers;

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
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (Exception e)
        {
            return e is UnauthorizedAccessException ? Unauthorized(e.Message) : StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var validation = _registerDtoValidator.Validate(registerDto);
            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);
        }
        catch (Exception e)
        {
            return e is InvalidOperationException ? BadRequest(e.Message) : StatusCode(500, "Erro interno do servidor");
        }
    }
}
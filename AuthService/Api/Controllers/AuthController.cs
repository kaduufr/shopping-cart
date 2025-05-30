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
}
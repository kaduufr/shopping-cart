namespace Application.Dtos;

public class AuthResultDto
{
    public string Token { get; set; }
    public string? RefreshToken { get; set; }
    public string UserId { get; set; }
}

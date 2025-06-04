namespace Api.Builders;

public static class ServicesBuilder
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

        builder
            .Services
            .AddControllers();
            
        // Configuração do PostgreSQL
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Database=authservice;Username=postgres;Password=postgres";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
            
        // Registro de repositórios
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        
        // Registro de serviços
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        
        // Configuração de autenticação JWT
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? "your-default-secret-key-with-at-least-32-chars"))
            };
        });
        
        // Adicionando serviço de autorização
        builder.Services.AddAuthorization();
        
        return builder;
    }
}


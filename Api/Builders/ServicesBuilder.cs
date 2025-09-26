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
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               "Host=localhost;Database=shopping_cart;Username=postgres;Password=postgres";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        builder.Services.AddSingleton<ICartRepository, RedisCartRepository>();

        // Registro de repositórios
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        // Registro de serviços
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICartService, CartService>();

        return builder;
    }
}


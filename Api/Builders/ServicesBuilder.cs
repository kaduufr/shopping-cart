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

        // Registro de repositórios
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICartRepository, RedisCartRepository>();

        // Registro de serviços
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICartService, CartService>();

        // Redis
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer
            .Connect(builder.Configuration.GetConnectionString("Redis") ??
                     throw new InvalidOperationException(
                         "Redis connection string is not configured.")));

        // Kafka
        var producerConfig = new ProducerConfig { BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers") };
        builder.Services.AddSingleton<IProducer<Null, string>>(new ProducerBuilder<Null, string>(producerConfig).Build());
        builder.Services.AddSingleton<IEventPublisher, KafkaEventPublisher>();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers"),
            GroupId = "shopping-cart-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        builder.Services.AddSingleton(new ConsumerBuilder<Ignore, string>(consumerConfig).Build());
        builder.Services.AddHostedService<CartEventsConsumer>();

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


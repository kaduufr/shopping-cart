using StackExchange.Redis;

namespace Api.Builders;

public static class RedisBuilder
{
    public static WebApplicationBuilder UseRedis(this WebApplicationBuilder builder)
    {
        // Redis
        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"));
        return builder;
    }
}
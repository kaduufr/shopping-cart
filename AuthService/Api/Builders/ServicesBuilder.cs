namespace Api.Builders;

public static class ServicesBuilder
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        return builder;
    }
}
namespace Api.Builders;

public static class OpenApiBuilder
{
    public static WebApplicationBuilder AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        // Add OpenAPI services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure Swagger UI
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "v1" });
        });

        return builder;
    }
    
    public static WebApplication UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
            });
        }

        return app;
    }
}
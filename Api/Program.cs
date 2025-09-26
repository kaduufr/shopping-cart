var builder = WebApplication
    .CreateBuilder(args)
    .AddOpenApi()
    .UseRedis()     // Primeiro o Redis
    .AddServices()  // Depois os serviços que dependem do Redis
    .UseAuth()
    .UseKafka();    // Por último o Kafka que depende dos serviços

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseOpenApi();

app.Run();

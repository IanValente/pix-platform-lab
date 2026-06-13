using SettlementService.Infrastructure.Adapter.In;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// A MÁGICA: Inicia nosso listener do RabbitMQ em background
builder.Services.AddHostedService<PixCreatedEventConsumer>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
using SettlementService.Application.Port.In;
using SettlementService.Application.Port.Out;
using SettlementService.Application.UseCase;
using SettlementService.Infrastructure.Adapter.In;
using SettlementService.Infrastructure.Adapter.Out;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// === INJEÇÃO DE DEPENDÊNCIA (Mapeando Interface -> Classe Real) ===

// Singleton: Só queremos 1 banco em memória vivo o tempo todo (se fosse Scoped, ele apagaria a cada Pix)
builder.Services.AddSingleton<ISaveSettlementPort, InMemorySettlementAdapter>();

// Scoped: Criado, usado e destruído a cada processamento (padrão ouro para UseCases)
builder.Services.AddScoped<IProcessSettlementUseCase, ProcessSettlementService>();

// Registra o Background Worker que consome o RabbitMQ
builder.Services.AddHostedService<PixCreatedEventConsumer>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
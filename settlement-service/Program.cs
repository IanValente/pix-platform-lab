using Microsoft.EntityFrameworkCore;
using SettlementService.Application.Port.In;
using SettlementService.Application.Port.Out;
using SettlementService.Application.UseCase;
using SettlementService.Infrastructure.Adapter.In;
using SettlementService.Infrastructure.Adapter.In.Web.Exceptions;
using SettlementService.Infrastructure.Adapter.Out.Database; // Novo import

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// === REGISTRO DO TRATADOR DE ERROS (NOVO) ===
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // Cadastra nossa classe
builder.Services.AddProblemDetails(); // Ensina o .NET a formatar JSON de erro padronizado

// === INJEÇÃO DO BANCO DE DADOS ===
// Ensina o EF Core a usar o SQL Server com a credencial do appsettings.json
builder.Services.AddDbContext<SettlementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// === INJEÇÃO DE DEPENDÊNCIA (Substituição de Liskov em Ação) ===
// ANTES (comentado mentalmente): builder.Services.AddSingleton<ISaveSettlementPort, InMemorySettlementAdapter>();
// AGORA: Registramos o banco de dados real. Como conexões de banco de dados não podem ser Singleton para não travar, usamos AddScoped.
builder.Services.AddScoped<ISaveSettlementPort, SqlServerSettlementAdapter>();

builder.Services.AddScoped<IProcessSettlementUseCase, ProcessSettlementService>();
builder.Services.AddScoped<IGetSettlementQuery, GetSettlementService>();
builder.Services.AddHostedService<PixCreatedEventConsumer>();

// 1. Cria a regra ensinando o .NET a aceitar o Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SettlementService.Infrastructure.Adapter.Out.Database.SettlementDbContext>();
    // O comando Migrate lê a pasta Migrations e cria/atualiza as tabelas no SQL Server
    dbContext.Database.Migrate(); 
}
app.UseCors("PermitirAngular");
// === ATIVAÇÃO DO PIPELINE (NOVO) ===
app.UseExceptionHandler(); // Diz para o servidor: "Se der erro, use o tratador que cadastrei acima"
app.UseAuthorization();
app.MapControllers();

app.Run();
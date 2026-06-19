using SettlementService.Application.Port.Out;
using SettlementService.Domain.Model;

namespace SettlementService.Infrastructure.Adapter.Out.Database;

public class SqlServerSettlementAdapter : ISaveSettlementPort
{
    private readonly SettlementDbContext _context;

    // Injeção de dependência via construtor (Igual faríamos no Java com o JpaRepository)
    public SqlServerSettlementAdapter(SettlementDbContext context)
    {
        _context = context;
    }

    public Settlement Save(Settlement settlement)
    {
        // 1. Prepara o objeto para inserção (Equivalente ao entityManager.persist())
        _context.Settlements.Add(settlement);
        
        // 2. Comita a transação no banco (O EF Core controla a transação automaticamente aqui)
        _context.SaveChanges();
        
        System.Console.WriteLine($"[SQL Server] Liquidação {settlement.Id} gravada com sucesso!");
        
        return settlement;
    }
}
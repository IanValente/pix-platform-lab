using System.Collections.Generic;
using SettlementService.Application.Port.Out;
using SettlementService.Domain.Model;

namespace SettlementService.Infrastructure.Adapter.Out;

public class InMemorySettlementAdapter : ISaveSettlementPort
{
    // Simulando uma tabela de banco de dados na memória RAM
    private readonly List<Settlement> _database = new();

    public Settlement Save(Settlement settlement)
    {
        _database.Add(settlement);
        
        // Em um sistema corporativo real, aqui teríamos o log de persistência
        System.Console.WriteLine($"[Banco de Dados] Liquidação {settlement.Id} salva com status {settlement.Status}");
        
        return settlement;
    }
}
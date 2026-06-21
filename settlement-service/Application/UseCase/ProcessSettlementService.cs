using SettlementService.Application.Port.In;
using SettlementService.Application.Port.Out;
using SettlementService.Domain.Model;

namespace SettlementService.Application.UseCase;

// A classe implementa a Porta de Entrada e consome a Porta de Saída
public class ProcessSettlementService : IProcessSettlementUseCase
{
    private readonly ISaveSettlementPort _saveSettlementPort;

    // Injeção de dependência via construtor (O padrão ouro do SOLID)
    public ProcessSettlementService(ISaveSettlementPort saveSettlementPort)
    {
        _saveSettlementPort = saveSettlementPort;
    }

    public Settlement Execute(string pixTransactionId, decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("O valor do Pix deve ser maior que zero.");
        }
        // 1. Instancia a regra de negócio
        var settlement = new Settlement(pixTransactionId, amount);

        // 2. Executa o comportamento do domínio
        settlement.Process();

        // 3. Manda a infraestrutura salvar o resultado
        return _saveSettlementPort.Save(settlement);
    }
}
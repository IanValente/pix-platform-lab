using System;

namespace SettlementService.Domain.Model;

public class Settlement
{
    public Guid Id { get; private set; }
    public string PixTransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public SettlementStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; } // Nullable, pois só é preenchido ao processar

    // Construtor para criar uma nova intenção de liquidação
    public Settlement(string pixTransactionId, decimal amount)
    {
        Id = Guid.NewGuid();
        PixTransactionId = pixTransactionId;
        Amount = amount;
        Status = SettlementStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    // Regra de Negócio: O comportamento de processar a liquidação pertence à Entidade
    public void Process()
    {
        if (Status != SettlementStatus.Pending)
        {
            throw new InvalidOperationException("Apenas liquidações pendentes podem ser processadas.");
        }

        // Aqui entrariam validações financeiras reais (ex: taxa, limite, etc)
        
        Status = SettlementStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
    }
}
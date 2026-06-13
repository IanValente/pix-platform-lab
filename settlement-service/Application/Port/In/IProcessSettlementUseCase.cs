using SettlementService.Domain.Model;

namespace SettlementService.Application.Port.In;

public interface IProcessSettlementUseCase
{
    // Recebemos o ID original do Pix e o valor para liquidar
    Settlement Execute(string pixTransactionId, decimal amount);
}
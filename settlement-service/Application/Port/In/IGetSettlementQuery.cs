using System;
using System.Threading.Tasks;
using SettlementService.Domain.Model;

namespace SettlementService.Application.Port.In;

public interface IGetSettlementQuery
{
    // Usamos Task para assincronismo e '?' para indicar que pode voltar nulo
    Task<Settlement?> ExecuteAsync(Guid id);
}
using SettlementService.Domain.Model;

namespace SettlementService.Application.Port.Out;

public interface ISaveSettlementPort
{
    // O contrato diz: "Alguém precisa salvar isso, não me importa como."
    Settlement Save(Settlement settlement);
}
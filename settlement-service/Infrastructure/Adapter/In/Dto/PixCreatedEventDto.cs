namespace SettlementService.Infrastructure.Adapter.In.Dto;

// As propriedades devem bater com o JSON gerado pelo Java (id, key, amount)
public record PixCreatedEventDto(string id, string key, decimal amount);
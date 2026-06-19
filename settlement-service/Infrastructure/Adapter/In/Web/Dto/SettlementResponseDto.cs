using System;

namespace SettlementService.Infrastructure.Adapter.In.Web.Dto;

public record SettlementResponseDto(Guid Id, string PixTransactionId, decimal Amount, string Status, DateTime? ProcessedAt);
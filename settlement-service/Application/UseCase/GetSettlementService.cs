using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SettlementService.Application.Port.In;
using SettlementService.Domain.Model;
using SettlementService.Infrastructure.Adapter.Out.Database;

namespace SettlementService.Application.UseCase;

public class GetSettlementService : IGetSettlementQuery
{
    private readonly SettlementDbContext _context;

    public GetSettlementService(SettlementDbContext context)
    {
        _context = context;
    }

    public async Task<Settlement?> ExecuteAsync(Guid id)
    {
        // Vai no banco de forma assíncrona (sem travar a CPU)
        return await _context.Settlements.FirstOrDefaultAsync(s => s.PixTransactionId == id.ToString());
    }
}
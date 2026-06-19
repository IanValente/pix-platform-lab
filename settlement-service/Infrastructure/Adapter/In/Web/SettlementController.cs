using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SettlementService.Application.Port.In;
using SettlementService.Infrastructure.Adapter.In.Web.Dto;

namespace SettlementService.Infrastructure.Adapter.In.Web;

[ApiController]
[Route("api/v1/[controller]")] // [controller] é um atalho dinâmico que pega o nome da classe
public class SettlementController : ControllerBase
{
    private readonly IGetSettlementQuery _query;

    public SettlementController(IGetSettlementQuery query)
    {
        _query = query;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var settlement = await _query.ExecuteAsync(id);

        if (settlement == null)
        {
            return NotFound(); // Equivalente ao ResponseEntity.notFound()
        }

        var responseDto = new SettlementResponseDto(
            settlement.Id, 
            settlement.PixTransactionId, 
            settlement.Amount, 
            settlement.Status.ToString(), 
            settlement.ProcessedAt
        );

        return Ok(responseDto); // Equivalente ao ResponseEntity.ok()
    }
}
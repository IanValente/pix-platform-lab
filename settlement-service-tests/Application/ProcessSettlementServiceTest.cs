using System;
using System.Threading.Tasks;
using NSubstitute; // O equivalente ao import do Mockito
using SettlementService.Application.Port.Out;
using SettlementService.Application.UseCase;
using SettlementService.Domain.Model;
using Xunit; // O equivalente ao import do JUnit

namespace SettlementService.Tests.Application;

public class ProcessSettlementServiceTest
{
    // Variáveis globais do teste
    private readonly ISaveSettlementPort _savePortMock;
    private readonly ProcessSettlementService _sut; // SUT = System Under Test (Convenção de mercado)

    // O Construtor no xUnit atua como o @BeforeEach do JUnit. Ele roda zumbado antes de cada [Fact].
    public ProcessSettlementServiceTest()
    {
        // Mockito.mock(ISaveSettlementPort.class)
        _savePortMock = Substitute.For<ISaveSettlementPort>();
        
        // Injetamos o dublê no nosso serviço real
        _sut = new ProcessSettlementService(_savePortMock);
    }

    [Fact] 
    // 1. Removido o 'async Task' e substituído por 'void'
    public void Execute_ValidPixData_ShouldProcessAndSave()
    {
        // 1. ARRANGE
        var pixId = Guid.NewGuid().ToString();
        var amount = 150.50m; 

        // 2. Removido o 'Task.FromResult'. Agora o Mock devolve o objeto diretamente.
        _savePortMock.Save(Arg.Any<Settlement>())
                     .Returns(callInfo => callInfo.Arg<Settlement>());

        // 2. ACT
        // 3. Removido o 'await'
        var result = _sut.Execute(pixId, amount);

        // 3. ASSERT
        Assert.NotNull(result); 
        Assert.Equal(pixId, result.PixTransactionId); 
        Assert.Equal(amount, result.Amount);
        Assert.Equal(SettlementStatus.Processed, result.Status);
        
        // 4. Removido o 'await' antes do Received
        _savePortMock.Received(1).Save(Arg.Any<Settlement>());
    }

    [Fact]
    public void Execute_AmountIsZeroOrNegative_ShouldThrowArgumentException()
    {
        // 1. ARRANGE (Preparando o cenário ruim)
        var pixId = Guid.NewGuid().ToString();
        var invalidAmount = 0m; // Um Pix de zero reais!

        // 2. ACT & ASSERT (Ação e Validação acontecem juntas no teste de exceção)
        // Pedimos ao xUnit para capturar a exceção que DEVE ser lançada
        var exception = Assert.Throws<ArgumentException>(() => 
        {
            _sut.Execute(pixId, invalidAmount);
        });

        // Verificamos se a mensagem de erro é a que o negócio exige
        Assert.Contains("O valor do Pix deve ser maior que zero", exception.Message);

        // BEHAVIOR VERIFICATION: Garantimos que o Mock do banco NUNCA foi chamado
        // O equivalente ao verify(port, never()).save(any()) do Mockito
        _savePortMock.DidNotReceive().Save(Arg.Any<Settlement>());
    }
}
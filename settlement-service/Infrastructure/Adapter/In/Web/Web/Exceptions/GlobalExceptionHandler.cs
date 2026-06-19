using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SettlementService.Infrastructure.Adapter.In.Web.Exceptions;

// A interface IExceptionHandler é a grande novidade do .NET 8 para erros globais
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    // Esse método será chamado magicamente se qualquer Controller estourar um erro
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. Logamos o erro verdadeiro (com o Stack Trace) no nosso console/Kibana para os desenvolvedores
        _logger.LogError(exception, $"Erro não tratado: {exception.Message}");

        // 2. Definimos o status HTTP padrão e a mensagem base
        var statusCode = StatusCodes.Status500InternalServerError;
        var title = "Erro Interno no Servidor";
        var detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

        // 3. Verificamos se é um erro da nossa regra de negócio (Equivalente ao @ExceptionHandler(InvalidOperationException.class) do Java)
        if (exception is InvalidOperationException businessException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            title = "Violação de Regra de Negócio";
            detail = businessException.Message;
        }

        // 4. Montamos o JSON de erro oficial da Microsoft (ProblemDetails)
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path // Mostra em qual URL deu erro
        };

        httpContext.Response.StatusCode = statusCode;
        
        // 5. Devolvemos o JSON para o cliente
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornar 'true' diz para o .NET: "Eu cuidei desse erro, não precisa fazer mais nada"
        return true; 
    }
}
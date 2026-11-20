using GestaoLogistica.Application.Comum;

namespace GestaoLogistica.API.Middlewares;

// Captura qualquer erro não tratado e devolve no padrão ResultWrapper (nunca uma página de erro)
public class TratamentoErrosMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<TratamentoErrosMiddleware> _logger;

    public TratamentoErrosMiddleware(RequestDelegate proximo, ILogger<TratamentoErrosMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (Exception erro)
        {
            _logger.LogError(erro, "Erro não tratado na requisição {Caminho}", contexto.Request.Path);

            var resposta = ResultWrapper<object>.Falha(
                "Não foi possível realizar a operação.",
                new List<string> { "Erro interno no servidor. Tente novamente mais tarde." },
                500);

            contexto.Response.StatusCode = resposta.CodigoHttp;
            await contexto.Response.WriteAsJsonAsync(resposta);
        }
    }
}

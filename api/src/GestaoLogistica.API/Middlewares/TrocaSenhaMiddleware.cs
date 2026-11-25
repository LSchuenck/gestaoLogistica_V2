using GestaoLogistica.Application.Comum;

namespace GestaoLogistica.API.Middlewares;

// Quem está com a senha temporária só pode trocar a senha
public class TrocaSenhaMiddleware
{
    private const string EnderecoTrocarSenha = "/api/autenticacao/senha";
    private readonly RequestDelegate _proximo;

    public TrocaSenhaMiddleware(RequestDelegate proximo)
    {
        _proximo = proximo;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        var precisaTrocar = contexto.User.HasClaim("TrocarSenha", "True");
        var estaTrocando = contexto.Request.Path.StartsWithSegments(EnderecoTrocarSenha);

        if (precisaTrocar && !estaTrocando)
        {
            contexto.Response.StatusCode = 403;
            await contexto.Response.WriteAsJsonAsync(ResultWrapper<object>.Falha("Troque a senha temporária para continuar.", null, 403));
            return;
        }

        await _proximo(contexto);
    }
}

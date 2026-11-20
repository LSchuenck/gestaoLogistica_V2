using System.Security.Claims;
using GestaoLogistica.Application.Comum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

// Base de todos os controllers: exige token JWT válido e padroniza a resposta HTTP
[ApiController]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    // Devolve o ResultWrapper com o código HTTP definido pelo serviço (200, 201, 400, 404...)
    protected IActionResult Responder<T>(ResultWrapper<T> resultado)
    {
        return StatusCode(resultado.CodigoHttp, resultado);
    }

    // Id do usuário que está logado (lido de dentro do token JWT)
    protected int IdUsuarioLogado()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}

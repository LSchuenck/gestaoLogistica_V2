using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/autenticacao")]
public class AutenticacaoController : ApiControllerBase
{
    private readonly IServicoAutenticacao _servico;

    public AutenticacaoController(IServicoAutenticacao servico)
    {
        _servico = servico;
    }

    // POST api/autenticacao/login  (público: é aqui que o usuário recebe o token)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Entrar(LoginDto dados) => Responder(await _servico.Entrar(dados));
}

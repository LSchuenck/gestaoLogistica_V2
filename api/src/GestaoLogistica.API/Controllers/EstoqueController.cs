using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/estoque")]
public class EstoqueController : ApiControllerBase
{
    private readonly IServicoEstoque _servico;

    public EstoqueController(IServicoEstoque servico)
    {
        _servico = servico;
    }

    // Saldo de cada produto em cada armazém
    [HttpGet]
    public async Task<IActionResult> ListarSaldos() => Responder(await _servico.ListarSaldos());

    // Últimas movimentações (histórico)
    [HttpGet("movimentacoes")]
    public async Task<IActionResult> ListarMovimentacoes() => Responder(await _servico.ListarUltimasMovimentacoes());

    // Registra uma ENTRADA ou SAIDA. O usuário responsável é o dono do token.
    [HttpPost("movimentacoes")]
    public async Task<IActionResult> RegistrarMovimentacao(RegistrarMovimentacaoDto dados)
        => Responder(await _servico.RegistrarMovimentacao(dados, IdUsuarioLogado()));
}

using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/transportadoras")]
public class TransportadorasController : ApiControllerBase
{
    private readonly IServicoTransportadora _servico;

    public TransportadorasController(IServicoTransportadora servico)
    {
        _servico = servico;
    }

    // Esta consulta usa CACHE (ver ServicoTransportadora)
    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id) => Responder(await _servico.BuscarPorId(id));

    [HttpPost]
    public async Task<IActionResult> Cadastrar(SalvarTransportadoraDto dados) => Responder(await _servico.Cadastrar(dados));

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, SalvarTransportadoraDto dados) => Responder(await _servico.Atualizar(id, dados));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlternarStatus(int id) => Responder(await _servico.AlternarStatus(id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id) => Responder(await _servico.Excluir(id));
}

using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/veiculos")]
[Authorize(Roles = PerfisUsuario.AdministradorOuGerente)]
public class VeiculosController : ApiControllerBase
{
    private readonly IServicoVeiculo _servico;

    public VeiculosController(IServicoVeiculo servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id) => Responder(await _servico.BuscarPorId(id));

    [HttpPost]
    public async Task<IActionResult> Cadastrar(SalvarVeiculoDto dados) => Responder(await _servico.Cadastrar(dados));

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, SalvarVeiculoDto dados) => Responder(await _servico.Atualizar(id, dados));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlterarStatus(int id, AlterarStatusVeiculoDto dados) => Responder(await _servico.AlterarStatus(id, dados.Status));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id) => Responder(await _servico.Excluir(id));
}

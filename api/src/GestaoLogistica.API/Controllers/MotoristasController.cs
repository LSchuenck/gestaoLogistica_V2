using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

// A lista é liberada para todos porque a tela de Viagens usa; o resto é só para ADMIN e GERENTE
[Route("api/motoristas")]
public class MotoristasController : ApiControllerBase
{
    private readonly IServicoMotorista _servico;

    public MotoristasController(IServicoMotorista servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpGet("{id}")]
    [Authorize(Roles = PerfisUsuario.AdministradorOuGerente)]
    public async Task<IActionResult> BuscarPorId(int id) => Responder(await _servico.BuscarPorId(id));

    [HttpPost]
    [Authorize(Roles = PerfisUsuario.AdministradorOuGerente)]
    public async Task<IActionResult> Cadastrar(SalvarMotoristaDto dados) => Responder(await _servico.Cadastrar(dados));

    [HttpPut("{id}")]
    [Authorize(Roles = PerfisUsuario.AdministradorOuGerente)]
    public async Task<IActionResult> Atualizar(int id, SalvarMotoristaDto dados) => Responder(await _servico.Atualizar(id, dados));

    [HttpDelete("{id}")]
    [Authorize(Roles = PerfisUsuario.AdministradorOuGerente)]
    public async Task<IActionResult> Excluir(int id) => Responder(await _servico.Excluir(id));
}

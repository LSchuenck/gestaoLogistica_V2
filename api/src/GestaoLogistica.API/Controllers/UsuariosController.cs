using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Constantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

// Gestão de usuários: somente o perfil ADMIN tem acesso
[Route("api/usuarios")]
[Authorize(Roles = PerfisUsuario.Administrador)]
public class UsuariosController : ApiControllerBase
{
    private readonly IServicoUsuario _servico;

    public UsuariosController(IServicoUsuario servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id) => Responder(await _servico.BuscarPorId(id));

    [HttpPost]
    public async Task<IActionResult> Cadastrar(SalvarUsuarioDto dados) => Responder(await _servico.Cadastrar(dados));

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, SalvarUsuarioDto dados) => Responder(await _servico.Atualizar(id, dados));

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlternarStatus(int id) => Responder(await _servico.AlternarStatus(id, IdUsuarioLogado()));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id) => Responder(await _servico.Excluir(id, IdUsuarioLogado()));
}

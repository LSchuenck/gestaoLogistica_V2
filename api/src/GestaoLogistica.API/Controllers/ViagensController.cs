using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/viagens")]
public class ViagensController : ApiControllerBase
{
    private readonly IServicoViagem _servico;

    public ViagensController(IServicoViagem servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpPost]
    public async Task<IActionResult> Iniciar(IniciarViagemDto dados) => Responder(await _servico.Iniciar(dados));

    [HttpPatch("{id}/avancar")]
    public async Task<IActionResult> Avancar(int id) => Responder(await _servico.Avancar(id));

    [HttpPatch("{id}/concluir")]
    public async Task<IActionResult> Concluir(int id) => Responder(await _servico.Concluir(id));

    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id) => Responder(await _servico.Cancelar(id));
}

using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/clientes")]
public class ClientesController : ApiControllerBase
{
    private readonly IServicoCliente _servico;

    public ClientesController(IServicoCliente servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id) => Responder(await _servico.BuscarPorId(id));

    [HttpPost]
    public async Task<IActionResult> Cadastrar(SalvarClienteDto dados) => Responder(await _servico.Cadastrar(dados));

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, SalvarClienteDto dados) => Responder(await _servico.Atualizar(id, dados));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id) => Responder(await _servico.Excluir(id));
}

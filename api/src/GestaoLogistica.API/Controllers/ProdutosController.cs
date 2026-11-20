using GestaoLogistica.Application.DTOs;
using GestaoLogistica.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistica.API.Controllers;

[Route("api/produtos")]
public class ProdutosController : ApiControllerBase
{
    private readonly IServicoProduto _servico;

    public ProdutosController(IServicoProduto servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar() => Responder(await _servico.Listar());

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id) => Responder(await _servico.BuscarPorId(id));

    [HttpPost]
    public async Task<IActionResult> Cadastrar(SalvarProdutoDto dados) => Responder(await _servico.Cadastrar(dados));

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, SalvarProdutoDto dados) => Responder(await _servico.Atualizar(id, dados));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id) => Responder(await _servico.Excluir(id));
}

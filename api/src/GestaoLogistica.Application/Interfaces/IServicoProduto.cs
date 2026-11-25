using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoProduto
{
    Task<ResultWrapper<List<ProdutoDto>>> Listar();
    Task<ResultWrapper<ProdutoDto>> BuscarPorId(int id);
    Task<ResultWrapper<ProdutoDto>> Cadastrar(SalvarProdutoDto dados);
    Task<ResultWrapper<ProdutoDto>> Atualizar(int id, SalvarProdutoDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

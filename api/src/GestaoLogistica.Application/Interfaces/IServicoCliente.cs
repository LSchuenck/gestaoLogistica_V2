using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoCliente
{
    Task<ResultWrapper<List<ClienteDto>>> Listar();
    Task<ResultWrapper<ClienteDto>> BuscarPorId(int id);
    Task<ResultWrapper<ClienteDto>> Cadastrar(SalvarClienteDto dados);
    Task<ResultWrapper<ClienteDto>> Atualizar(int id, SalvarClienteDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

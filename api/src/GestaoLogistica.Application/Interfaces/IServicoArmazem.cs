using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoArmazem
{
    Task<ResultWrapper<List<ArmazemDto>>> Listar();
    Task<ResultWrapper<ArmazemDto>> BuscarPorId(int id);
    Task<ResultWrapper<ArmazemDto>> Cadastrar(SalvarArmazemDto dados);
    Task<ResultWrapper<ArmazemDto>> Atualizar(int id, SalvarArmazemDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

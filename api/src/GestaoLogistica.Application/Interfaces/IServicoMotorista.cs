using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoMotorista
{
    Task<ResultWrapper<List<MotoristaDto>>> Listar();
    Task<ResultWrapper<MotoristaDto>> BuscarPorId(int id);
    Task<ResultWrapper<MotoristaDto>> Cadastrar(SalvarMotoristaDto dados);
    Task<ResultWrapper<MotoristaDto>> Atualizar(int id, SalvarMotoristaDto dados);
    Task<ResultWrapper<object>> Excluir(int id);
}

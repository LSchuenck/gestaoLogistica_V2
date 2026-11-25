using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoVeiculo
{
    Task<ResultWrapper<List<VeiculoDto>>> Listar();
    Task<ResultWrapper<VeiculoDto>> BuscarPorId(int id);
    Task<ResultWrapper<VeiculoDto>> Cadastrar(SalvarVeiculoDto dados);
    Task<ResultWrapper<VeiculoDto>> Atualizar(int id, SalvarVeiculoDto dados);
    Task<ResultWrapper<VeiculoDto>> AlterarStatus(int id, string novoStatus);
    Task<ResultWrapper<object>> Excluir(int id);
}

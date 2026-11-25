using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoTransportadora
{
    Task<ResultWrapper<List<TransportadoraDto>>> Listar();
    Task<ResultWrapper<TransportadoraDto>> BuscarPorId(int id);
    Task<ResultWrapper<TransportadoraDto>> Cadastrar(SalvarTransportadoraDto dados);
    Task<ResultWrapper<TransportadoraDto>> Atualizar(int id, SalvarTransportadoraDto dados);
    Task<ResultWrapper<TransportadoraDto>> AlternarStatus(int id);
    Task<ResultWrapper<object>> Excluir(int id);
}

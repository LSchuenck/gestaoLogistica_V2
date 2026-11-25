using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoViagem
{
    Task<ResultWrapper<List<ViagemDto>>> Listar();
    Task<ResultWrapper<ViagemDto>> Iniciar(IniciarViagemDto dados);
    Task<ResultWrapper<ViagemDto>> Avancar(int id);
    Task<ResultWrapper<ViagemDto>> Concluir(int id);
    Task<ResultWrapper<ViagemDto>> Cancelar(int id);
}

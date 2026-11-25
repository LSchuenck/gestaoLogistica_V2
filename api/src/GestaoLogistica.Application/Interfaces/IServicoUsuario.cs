using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoUsuario
{
    Task<ResultWrapper<List<UsuarioDto>>> Listar();
    Task<ResultWrapper<UsuarioDto>> BuscarPorId(int id);
    Task<ResultWrapper<UsuarioDto>> Cadastrar(SalvarUsuarioDto dados);
    Task<ResultWrapper<UsuarioDto>> Atualizar(int id, SalvarUsuarioDto dados);
    Task<ResultWrapper<UsuarioDto>> AlternarStatus(int id, int idUsuarioLogado);
    Task<ResultWrapper<object>> Excluir(int id, int idUsuarioLogado);
}

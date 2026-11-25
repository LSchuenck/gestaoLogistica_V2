using GestaoLogistica.Application.Comum;
using GestaoLogistica.Application.DTOs;

namespace GestaoLogistica.Application.Interfaces;

public interface IServicoEstoque
{
    Task<ResultWrapper<List<SaldoEstoqueDto>>> ListarSaldos();
    Task<ResultWrapper<List<MovimentacaoDto>>> ListarUltimasMovimentacoes();
    Task<ResultWrapper<SaldoEstoqueDto>> RegistrarMovimentacao(RegistrarMovimentacaoDto dados, int idUsuarioLogado);
}

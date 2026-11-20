using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioEstoque
{
    Task<List<Estoque>> ListarSaldos();
    Task<Estoque?> BuscarSaldo(int idProduto, int idArmazem);
    Task<List<MovimentacaoEstoque>> ListarUltimasMovimentacoes(int quantidade);

    // Grava o novo saldo e o histórico juntos (ou grava os dois, ou nenhum)
    Task SalvarMovimentacao(Estoque estoque, MovimentacaoEstoque movimentacao);
}

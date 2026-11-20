using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioProduto : IRepositorioBase<Produto>
{
    // Verifica se o produto tem saldo em algum armazém ou movimentações registradas
    Task<bool> PossuiEstoqueOuMovimentacoes(int idProduto);
}

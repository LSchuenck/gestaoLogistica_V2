using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioArmazem : IRepositorioBase<Armazem>
{
    // Verifica se o armazém tem produtos com saldo ou movimentações registradas
    Task<bool> PossuiEstoqueOuMovimentacoes(int idArmazem);
}

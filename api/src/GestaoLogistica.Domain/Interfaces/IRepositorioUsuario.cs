using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioUsuario : IRepositorioBase<Usuario>
{
    Task<Usuario?> BuscarPorEmail(string email);
    Task<bool> EmailJaCadastrado(string email, int idIgnorado = 0);

    // Verifica se o usuário já registrou movimentações de estoque
    Task<bool> PossuiMovimentacoes(int idUsuario);
}

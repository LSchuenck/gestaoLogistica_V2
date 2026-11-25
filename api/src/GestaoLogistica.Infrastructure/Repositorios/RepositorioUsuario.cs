using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioUsuario : RepositorioBase<Usuario>, IRepositorioUsuario
{
    public RepositorioUsuario(ContextoBanco contexto) : base(contexto) { }

    public async Task<Usuario?> BuscarPorEmail(string email)
    {
        return await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailJaCadastrado(string email, int idIgnorado = 0)
    {
        return await _contexto.Usuarios.AnyAsync(u => u.Email == email && u.Id != idIgnorado);
    }

    public async Task<bool> PossuiMovimentacoes(int idUsuario)
    {
        return await _contexto.MovimentacoesEstoque.AnyAsync(m => m.UsuarioId == idUsuario);
    }
}

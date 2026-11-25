using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioArmazem : RepositorioBase<Armazem>, IRepositorioArmazem
{
    public RepositorioArmazem(ContextoBanco contexto) : base(contexto) { }

    public async Task<bool> PossuiEstoqueOuMovimentacoes(int idArmazem)
    {
        return await _contexto.Estoques.AnyAsync(e => e.ArmazemId == idArmazem && e.Quantidade > 0)
            || await _contexto.MovimentacoesEstoque.AnyAsync(m => m.ArmazemId == idArmazem);
    }
}

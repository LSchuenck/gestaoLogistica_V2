using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

// Implementação do CRUD básico, reaproveitada por todos os repositórios
public class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly ContextoBanco _contexto;

    public RepositorioBase(ContextoBanco contexto)
    {
        _contexto = contexto;
    }

    // Consulta base. Os repositórios filhos sobrescrevem para incluir dados relacionados (Include).
    protected virtual IQueryable<T> Consulta() => _contexto.Set<T>();

    public async Task<List<T>> ListarTodos()
    {
        return await Consulta().OrderBy(e => e.Id).ToListAsync();
    }

    public async Task<T?> BuscarPorId(int id)
    {
        return await Consulta().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task Adicionar(T entidade)
    {
        _contexto.Set<T>().Add(entidade);
        await _contexto.SaveChangesAsync();
    }

    // A entidade foi carregada pelo próprio contexto, então o EF já sabe o que mudou
    public async Task Atualizar(T entidade)
    {
        await _contexto.SaveChangesAsync();
    }

    public async Task Remover(T entidade)
    {
        _contexto.Set<T>().Remove(entidade);
        await _contexto.SaveChangesAsync();
    }
}

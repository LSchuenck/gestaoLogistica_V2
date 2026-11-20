using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

// Operações comuns a todos os repositórios (CRUD básico).
public interface IRepositorioBase<T> where T : EntidadeBase
{
    Task<List<T>> ListarTodos();
    Task<T?> BuscarPorId(int id);
    Task Adicionar(T entidade);
    Task Atualizar(T entidade);
    Task Remover(T entidade);
}

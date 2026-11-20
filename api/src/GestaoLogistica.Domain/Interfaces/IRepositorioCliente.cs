using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

// Cliente usa apenas as operações básicas do CRUD
public interface IRepositorioCliente : IRepositorioBase<Cliente>
{
}

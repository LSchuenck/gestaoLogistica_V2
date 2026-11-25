using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioCliente : RepositorioBase<Cliente>, IRepositorioCliente
{
    public RepositorioCliente(ContextoBanco contexto) : base(contexto) { }
}

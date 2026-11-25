using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioMotorista : RepositorioBase<Motorista>, IRepositorioMotorista
{
    public RepositorioMotorista(ContextoBanco contexto) : base(contexto) { }

    protected override IQueryable<Motorista> Consulta() => _contexto.Motoristas.Include(m => m.Transportadora);

    public async Task<bool> CpfOuCnhJaCadastrado(string? cpf, string cnh, int idIgnorado = 0)
    {
        return await _contexto.Motoristas.AnyAsync(m =>
            m.Id != idIgnorado && (m.Cnh == cnh || (cpf != null && cpf != "" && m.Cpf == cpf)));
    }
}

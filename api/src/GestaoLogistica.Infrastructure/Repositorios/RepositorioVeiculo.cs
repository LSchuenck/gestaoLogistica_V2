using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioVeiculo : RepositorioBase<Veiculo>, IRepositorioVeiculo
{
    public RepositorioVeiculo(ContextoBanco contexto) : base(contexto) { }

    protected override IQueryable<Veiculo> Consulta() => _contexto.Veiculos.Include(v => v.Transportadora);

    public async Task<bool> PlacaJaCadastrada(string placa, int idIgnorado = 0)
    {
        return await _contexto.Veiculos.AnyAsync(v => v.Placa == placa && v.Id != idIgnorado);
    }
}

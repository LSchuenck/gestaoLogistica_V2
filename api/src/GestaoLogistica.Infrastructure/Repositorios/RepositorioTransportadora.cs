using GestaoLogistica.Domain.Entidades;
using GestaoLogistica.Domain.Interfaces;
using GestaoLogistica.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistica.Infrastructure.Repositorios;

public class RepositorioTransportadora : RepositorioBase<Transportadora>, IRepositorioTransportadora
{
    public RepositorioTransportadora(ContextoBanco contexto) : base(contexto) { }

    public async Task<bool> CnpjJaCadastrado(string cnpj, int idIgnorado = 0)
    {
        return await _contexto.Transportadoras.AnyAsync(t => t.Cnpj == cnpj && t.Id != idIgnorado);
    }

    public async Task<bool> PossuiMotoristasOuVeiculos(int idTransportadora)
    {
        return await _contexto.Motoristas.AnyAsync(m => m.TransportadoraId == idTransportadora)
            || await _contexto.Veiculos.AnyAsync(v => v.TransportadoraId == idTransportadora);
    }
}

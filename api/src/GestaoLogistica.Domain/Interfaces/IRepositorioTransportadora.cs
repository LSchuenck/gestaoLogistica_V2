using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioTransportadora : IRepositorioBase<Transportadora>
{
    Task<bool> CnpjJaCadastrado(string cnpj, int idIgnorado = 0);

    // Verifica se existem motoristas ou veículos ligados à transportadora
    Task<bool> PossuiMotoristasOuVeiculos(int idTransportadora);
}

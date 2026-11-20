using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioVeiculo : IRepositorioBase<Veiculo>
{
    Task<bool> PlacaJaCadastrada(string placa, int idIgnorado = 0);
}

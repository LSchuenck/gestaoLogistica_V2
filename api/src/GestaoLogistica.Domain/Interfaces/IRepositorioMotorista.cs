using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioMotorista : IRepositorioBase<Motorista>
{
    Task<bool> CpfOuCnhJaCadastrado(string? cpf, string cnh, int idIgnorado = 0);
}

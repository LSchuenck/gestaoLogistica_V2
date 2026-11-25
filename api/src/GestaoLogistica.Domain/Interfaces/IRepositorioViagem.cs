using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Domain.Interfaces;

public interface IRepositorioViagem : IRepositorioBase<Viagem>
{
    // Viagem iniciada ou em trânsito
    Task<bool> VeiculoEstaEmViagem(int idVeiculo);
    Task<bool> MotoristaEstaEmViagem(int idMotorista);

    // Qualquer viagem, mesmo já terminada (usado para impedir exclusões)
    Task<bool> VeiculoPossuiViagens(int idVeiculo);
    Task<bool> MotoristaPossuiViagens(int idMotorista);
    Task<bool> ClientePossuiViagens(int idCliente);
    Task<bool> ArmazemPossuiViagens(int idArmazem);

    // Executa a operação com o motorista, o veículo e o produto travados, para que duas pessoas
    // não iniciem viagens com eles ao mesmo tempo
    Task<T> ExecutarComTrava<T>(int idMotorista, int idVeiculo, int idProduto, Func<Task<T>> operacao);

    // Executa a operação com a viagem, o veículo e o produto dela travados, para que duas pessoas
    // não mudem a mesma viagem ao mesmo tempo (o estoque seria devolvido duas vezes)
    Task<T> ExecutarComViagemTravada<T>(int idViagem, Func<Task<T>> operacao);
}

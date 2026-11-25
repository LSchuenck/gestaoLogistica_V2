using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class ViagemDto
{
    public int Id { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public string Veiculo { get; set; } = string.Empty;
    public string Motorista { get; set; } = string.Empty;
    public decimal DistanciaKm { get; set; }
    public decimal ValorPorKm { get; set; }
    public decimal Faturamento { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataSaida { get; set; }
    public DateTime? DataChegada { get; set; }

    public static ViagemDto Converter(Viagem viagem)
    {
        return new ViagemDto
        {
            Id = viagem.Id,
            Origem = viagem.Armazem?.Nome ?? string.Empty,
            Destino = viagem.Cliente?.Nome ?? string.Empty,
            Veiculo = viagem.Veiculo?.Placa ?? string.Empty,
            Motorista = viagem.Motorista?.Nome ?? string.Empty,
            DistanciaKm = viagem.DistanciaKm,
            ValorPorKm = viagem.ValorPorKm,
            Faturamento = viagem.CalcularFaturamento(),
            Status = viagem.Status,
            DataSaida = viagem.DataSaida,
            DataChegada = viagem.DataChegada
        };
    }
}

public class IniciarViagemDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione o armazém de origem.")]
    public int ArmazemId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o cliente de destino.")]
    public int ClienteId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o veículo.")]
    public int VeiculoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o motorista.")]
    public int MotoristaId { get; set; }

    [Range(0.01, 99999.99, ErrorMessage = "A distância deve ficar entre 0,01 e 99.999,99 km.")]
    public decimal DistanciaKm { get; set; }

    [Range(0.01, 9999.99, ErrorMessage = "O valor por km deve ficar entre 0,01 e 9.999,99.")]
    public decimal ValorPorKm { get; set; }
}

using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class VeiculoDto
{
    public int Id { get; set; }
    public int TransportadoraId { get; set; }
    public string NomeTransportadora { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public string? TipoVeiculo { get; set; }
    public decimal CapacidadeCarga { get; set; }
    public string Status { get; set; } = string.Empty;

    public static VeiculoDto Converter(Veiculo veiculo)
    {
        return new VeiculoDto
        {
            Id = veiculo.Id,
            TransportadoraId = veiculo.TransportadoraId,
            NomeTransportadora = veiculo.Transportadora?.NomeFantasia ?? string.Empty,
            Placa = veiculo.Placa,
            Modelo = veiculo.Modelo,
            TipoVeiculo = veiculo.TipoVeiculo,
            CapacidadeCarga = veiculo.CapacidadeCarga,
            Status = veiculo.Status
        };
    }
}

public class SalvarVeiculoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione a transportadora.")]
    public int TransportadoraId { get; set; }

    [Required(ErrorMessage = "Informe a placa.")]
    [MaxLength(10, ErrorMessage = "O campo placa aceita no máximo 10 caracteres.")]
    public string Placa { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "O campo modelo aceita no máximo 100 caracteres.")]
    public string? Modelo { get; set; }

    [MaxLength(50, ErrorMessage = "O campo tipo do veículo aceita no máximo 50 caracteres.")]
    public string? TipoVeiculo { get; set; }

    [Range(0, 99999999.99, ErrorMessage = "A capacidade deve ficar entre 0 e 99.999.999,99.")]
    public decimal CapacidadeCarga { get; set; }
}

public class AlterarStatusVeiculoDto
{
    [Required(ErrorMessage = "Informe o status.")]
    public string Status { get; set; } = string.Empty;
}

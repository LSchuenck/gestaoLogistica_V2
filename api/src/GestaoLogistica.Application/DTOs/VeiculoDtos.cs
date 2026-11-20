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
    [MaxLength(10)]
    public string Placa { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Modelo { get; set; }

    [MaxLength(50)]
    public string? TipoVeiculo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "A capacidade não pode ser negativa.")]
    public decimal CapacidadeCarga { get; set; }
}

public class AlterarStatusVeiculoDto
{
    [Required(ErrorMessage = "Informe o status.")]
    public string Status { get; set; } = string.Empty;
}

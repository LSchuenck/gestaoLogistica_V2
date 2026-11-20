using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class MotoristaDto
{
    public int Id { get; set; }
    public int TransportadoraId { get; set; }
    public string NomeTransportadora { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Cnh { get; set; } = string.Empty;
    public string? CategoriaCnh { get; set; }
    public DateOnly? ValidadeCnh { get; set; }
    public bool CnhVencida { get; set; }
    public string? Telefone { get; set; }
    public string Status { get; set; } = string.Empty;

    public static MotoristaDto Converter(Motorista motorista)
    {
        return new MotoristaDto
        {
            Id = motorista.Id,
            TransportadoraId = motorista.TransportadoraId,
            NomeTransportadora = motorista.Transportadora?.NomeFantasia ?? string.Empty,
            Nome = motorista.Nome,
            Cpf = motorista.Cpf,
            Cnh = motorista.Cnh,
            CategoriaCnh = motorista.CategoriaCnh,
            ValidadeCnh = motorista.ValidadeCnh,
            CnhVencida = motorista.CnhEstaVencida(),
            Telefone = motorista.Telefone,
            Status = motorista.Status
        };
    }
}

public class SalvarMotoristaDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione a transportadora.")]
    public int TransportadoraId { get; set; }

    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(14)]
    public string? Cpf { get; set; }

    [Required(ErrorMessage = "Informe a CNH.")]
    [MaxLength(20)]
    public string Cnh { get; set; } = string.Empty;

    [MaxLength(5)]
    public string? CategoriaCnh { get; set; }

    public DateOnly? ValidadeCnh { get; set; }

    public string? Telefone { get; set; }
}

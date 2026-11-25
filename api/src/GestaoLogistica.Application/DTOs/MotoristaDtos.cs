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
    [MaxLength(150, ErrorMessage = "O campo nome aceita no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(14, ErrorMessage = "O campo CPF aceita no máximo 14 caracteres.")]
    public string? Cpf { get; set; }

    [Required(ErrorMessage = "Informe a CNH.")]
    [MaxLength(20, ErrorMessage = "O campo CNH aceita no máximo 20 caracteres.")]
    public string Cnh { get; set; } = string.Empty;

    [MaxLength(5, ErrorMessage = "O campo categoria da CNH aceita no máximo 5 caracteres.")]
    public string? CategoriaCnh { get; set; }

    public DateOnly? ValidadeCnh { get; set; }

    [MaxLength(20, ErrorMessage = "O campo telefone aceita no máximo 20 caracteres.")]
    public string? Telefone { get; set; }
}

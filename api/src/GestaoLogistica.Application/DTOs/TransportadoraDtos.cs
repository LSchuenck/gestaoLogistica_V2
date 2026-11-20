using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class TransportadoraDto
{
    public int Id { get; set; }
    public string Cnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public EnderecoDto Endereco { get; set; } = new();
    public string Status { get; set; } = string.Empty;

    public static TransportadoraDto Converter(Transportadora transportadora)
    {
        return new TransportadoraDto
        {
            Id = transportadora.Id,
            Cnpj = transportadora.Cnpj,
            RazaoSocial = transportadora.RazaoSocial,
            NomeFantasia = transportadora.NomeFantasia,
            Telefone = transportadora.Telefone,
            Email = transportadora.Email,
            Endereco = EnderecoDto.Converter(transportadora.Endereco),
            Status = transportadora.Status
        };
    }
}

public class SalvarTransportadoraDto
{
    [Required(ErrorMessage = "Informe o CNPJ.")]
    [MaxLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a razão social.")]
    [MaxLength(150)]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o nome fantasia.")]
    [MaxLength(150)]
    public string NomeFantasia { get; set; } = string.Empty;

    public string? Telefone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    public EnderecoDto Endereco { get; set; } = new();
}

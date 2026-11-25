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
    [MaxLength(18, ErrorMessage = "O campo CNPJ aceita no máximo 18 caracteres.")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a razão social.")]
    [MaxLength(150, ErrorMessage = "O campo razão social aceita no máximo 150 caracteres.")]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o nome fantasia.")]
    [MaxLength(150, ErrorMessage = "O campo nome fantasia aceita no máximo 150 caracteres.")]
    public string NomeFantasia { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "O campo telefone aceita no máximo 20 caracteres.")]
    public string? Telefone { get; set; }

    [MaxLength(100, ErrorMessage = "O campo e-mail aceita no máximo 100 caracteres.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Informe o endereço.")]
    public EnderecoDto Endereco { get; set; } = new();
}

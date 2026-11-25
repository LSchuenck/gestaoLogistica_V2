using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public EnderecoDto Endereco { get; set; } = new();

    public static ClienteDto Converter(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CpfCnpj = cliente.CpfCnpj,
            Telefone = cliente.Telefone,
            Endereco = EnderecoDto.Converter(cliente.Endereco)
        };
    }
}

public class SalvarClienteDto
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(150, ErrorMessage = "O campo nome aceita no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(18, ErrorMessage = "O campo CPF/CNPJ aceita no máximo 18 caracteres.")]
    public string? CpfCnpj { get; set; }

    [MaxLength(20, ErrorMessage = "O campo telefone aceita no máximo 20 caracteres.")]
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "Informe o endereço.")]
    public EnderecoDto Endereco { get; set; } = new();
}

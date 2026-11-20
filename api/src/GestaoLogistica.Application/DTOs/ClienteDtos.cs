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
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(18)]
    public string? CpfCnpj { get; set; }

    public string? Telefone { get; set; }

    public EnderecoDto Endereco { get; set; } = new();
}

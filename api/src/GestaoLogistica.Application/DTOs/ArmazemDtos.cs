using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class ArmazemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public EnderecoDto Endereco { get; set; } = new();

    public static ArmazemDto Converter(Armazem armazem)
    {
        return new ArmazemDto
        {
            Id = armazem.Id,
            Nome = armazem.Nome,
            Endereco = EnderecoDto.Converter(armazem.Endereco)
        };
    }
}

public class SalvarArmazemDto
{
    [Required(ErrorMessage = "Informe o nome do armazém.")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public EnderecoDto Endereco { get; set; } = new();
}

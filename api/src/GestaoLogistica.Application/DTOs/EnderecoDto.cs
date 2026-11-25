using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class EnderecoDto
{
    [MaxLength(9, ErrorMessage = "O campo CEP aceita no máximo 9 caracteres.")]
    public string? Cep { get; set; }
    [MaxLength(255, ErrorMessage = "O campo logradouro aceita no máximo 255 caracteres.")]
    public string? Logradouro { get; set; }
    [MaxLength(20, ErrorMessage = "O campo número aceita no máximo 20 caracteres.")]
    public string? Numero { get; set; }
    [MaxLength(100, ErrorMessage = "O campo complemento aceita no máximo 100 caracteres.")]
    public string? Complemento { get; set; }
    [MaxLength(100, ErrorMessage = "O campo bairro aceita no máximo 100 caracteres.")]
    public string? Bairro { get; set; }
    [MaxLength(100, ErrorMessage = "O campo cidade aceita no máximo 100 caracteres.")]
    public string? Cidade { get; set; }
    [MaxLength(2, ErrorMessage = "O campo estado aceita no máximo 2 caracteres.")]
    public string? Estado { get; set; }

    public static EnderecoDto Converter(Endereco endereco)
    {
        return new EnderecoDto
        {
            Cep = endereco.Cep,
            Logradouro = endereco.Logradouro,
            Numero = endereco.Numero,
            Complemento = endereco.Complemento,
            Bairro = endereco.Bairro,
            Cidade = endereco.Cidade,
            Estado = endereco.Estado
        };
    }

    public Endereco ConverterParaEntidade()
    {
        return new Endereco
        {
            Cep = Cep,
            Logradouro = Logradouro,
            Numero = Numero,
            Complemento = Complemento,
            Bairro = Bairro,
            Cidade = Cidade,
            Estado = Estado
        };
    }
}

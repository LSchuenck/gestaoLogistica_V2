using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class EnderecoDto
{
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
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

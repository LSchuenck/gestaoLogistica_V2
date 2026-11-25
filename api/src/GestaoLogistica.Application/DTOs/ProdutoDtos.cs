using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

public class ProdutoDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal? Peso { get; set; }
    public decimal? Volume { get; set; }
    public DateOnly? Validade { get; set; }
    public int QuantidadeTotal { get; set; }
    public bool Vencido { get; set; }
    public bool EstoqueCritico { get; set; }

    public static ProdutoDto Converter(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Descricao = produto.Descricao,
            Peso = produto.Peso,
            Volume = produto.Volume,
            Validade = produto.Validade,
            QuantidadeTotal = produto.CalcularQuantidadeTotal(),
            Vencido = produto.EstaVencido(),
            EstoqueCritico = produto.EstaComEstoqueCritico()
        };
    }
}

public class SalvarProdutoDto
{
    [Required(ErrorMessage = "Informe a descrição.")]
    [MaxLength(200, ErrorMessage = "O campo descrição aceita no máximo 200 caracteres.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0, 99999999.99, ErrorMessage = "O peso deve ficar entre 0 e 99.999.999,99.")]
    public decimal? Peso { get; set; }

    [Range(0, 99999999.99, ErrorMessage = "O volume deve ficar entre 0 e 99.999.999,99.")]
    public decimal? Volume { get; set; }

    public DateOnly? Validade { get; set; }
}

using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

// Saldo de um produto em um armazém
public class SaldoEstoqueDto
{
    public int ProdutoId { get; set; }
    public string Produto { get; set; } = string.Empty;
    public int ArmazemId { get; set; }
    public string Armazem { get; set; } = string.Empty;
    public int Quantidade { get; set; }

    public static SaldoEstoqueDto Converter(Estoque estoque)
    {
        return new SaldoEstoqueDto
        {
            ProdutoId = estoque.ProdutoId,
            Produto = estoque.Produto?.Descricao ?? string.Empty,
            ArmazemId = estoque.ArmazemId,
            Armazem = estoque.Armazem?.Nome ?? string.Empty,
            Quantidade = estoque.Quantidade
        };
    }
}

// Linha do histórico de movimentações
public class MovimentacaoDto
{
    public int Id { get; set; }
    public string Produto { get; set; } = string.Empty;
    public string Armazem { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; }

    public static MovimentacaoDto Converter(MovimentacaoEstoque movimentacao)
    {
        return new MovimentacaoDto
        {
            Id = movimentacao.Id,
            Produto = movimentacao.Produto?.Descricao ?? string.Empty,
            Armazem = movimentacao.Armazem?.Nome ?? string.Empty,
            Usuario = movimentacao.Usuario?.Nome ?? string.Empty,
            Tipo = movimentacao.Tipo,
            Quantidade = movimentacao.Quantidade,
            DataMovimentacao = movimentacao.DataMovimentacao
        };
    }
}

// Dados recebidos para registrar uma entrada ou saída
public class RegistrarMovimentacaoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecione o produto.")]
    public int ProdutoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o armazém.")]
    public int ArmazemId { get; set; }

    [Required(ErrorMessage = "Informe o tipo (ENTRADA ou SAIDA).")]
    public string Tipo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }
}

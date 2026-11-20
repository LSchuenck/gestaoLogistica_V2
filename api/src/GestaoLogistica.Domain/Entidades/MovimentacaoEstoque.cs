namespace GestaoLogistica.Domain.Entidades;

// Histórico de entradas e saídas do estoque (rastreabilidade)
public class MovimentacaoEstoque : EntidadeBase
{
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int ArmazemId { get; set; }
    public Armazem? Armazem { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Tipo { get; set; } = string.Empty; // ENTRADA ou SAIDA
    public int Quantidade { get; set; }
    public DateTime DataMovimentacao { get; set; } = DateTime.Now;
}

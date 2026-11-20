namespace GestaoLogistica.Domain.Entidades;

public class Produto : EntidadeBase
{
    // Abaixo desta quantidade total o produto é considerado em nível crítico
    public const int LimiteEstoqueCritico = 10;

    public string Descricao { get; set; } = string.Empty;
    public decimal? Peso { get; set; }
    public decimal? Volume { get; set; }
    public DateOnly? Validade { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // Saldos deste produto em cada armazém
    public List<Estoque> Estoques { get; set; } = new();

    public int CalcularQuantidadeTotal() => Estoques.Sum(e => e.Quantidade);

    public bool EstaVencido()
    {
        return Validade.HasValue && Validade.Value < DateOnly.FromDateTime(DateTime.Today);
    }

    public bool EstaComEstoqueCritico() => CalcularQuantidadeTotal() < LimiteEstoqueCritico;
}

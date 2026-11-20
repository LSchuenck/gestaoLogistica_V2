namespace GestaoLogistica.Domain.Entidades;

// Saldo de um produto dentro de um armazém (um registro por produto + armazém)
public class Estoque : EntidadeBase
{
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public int ArmazemId { get; set; }
    public Armazem? Armazem { get; set; }
    public int Quantidade { get; set; }

    // Regra: não pode sair mais do que existe no armazém (o estoque nunca fica negativo)
    public bool TemSaldoSuficiente(int quantidadeSaida) => quantidadeSaida <= Quantidade;

    public void DarEntrada(int quantidade) => Quantidade += quantidade;

    public void DarSaida(int quantidade) => Quantidade -= quantidade;
}

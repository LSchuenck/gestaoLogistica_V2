namespace GestaoLogistica.Domain.Entidades;

// Armazém / Centro de Distribuição
public class Armazem : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public Endereco Endereco { get; set; } = new();
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}

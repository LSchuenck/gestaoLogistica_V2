namespace GestaoLogistica.Domain.Entidades;

public class Cliente : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public Endereco Endereco { get; set; } = new();
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}

using GestaoLogistica.Domain.Constantes;

namespace GestaoLogistica.Domain.Entidades;

public class Motorista : EntidadeBase
{
    public int TransportadoraId { get; set; }
    public Transportadora? Transportadora { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Cnh { get; set; } = string.Empty;
    public string? CategoriaCnh { get; set; }
    public DateOnly? ValidadeCnh { get; set; }
    public string? Telefone { get; set; }
    public string Status { get; set; } = StatusCadastro.Ativo;
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // Regra: a CNH está vencida quando a data de validade já passou
    public bool CnhEstaVencida()
    {
        return ValidadeCnh.HasValue && ValidadeCnh.Value < DateOnly.FromDateTime(DateTime.Today);
    }
}

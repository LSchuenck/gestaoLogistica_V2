using GestaoLogistica.Domain.Constantes;

namespace GestaoLogistica.Domain.Entidades;

public class Veiculo : EntidadeBase
{
    public int TransportadoraId { get; set; }
    public Transportadora? Transportadora { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public string? TipoVeiculo { get; set; }
    public decimal CapacidadeCarga { get; set; }
    public string Status { get; set; } = StatusVeiculo.Disponivel;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}

using GestaoLogistica.Domain.Constantes;

namespace GestaoLogistica.Domain.Entidades;

public class Viagem : EntidadeBase
{
    public int ArmazemId { get; set; }
    public Armazem? Armazem { get; set; } // origem
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; } // destino
    public int VeiculoId { get; set; }
    public Veiculo? Veiculo { get; set; }
    public int MotoristaId { get; set; }
    public Motorista? Motorista { get; set; }
    public decimal DistanciaKm { get; set; }
    public decimal ValorPorKm { get; set; } // valor do frete por km
    public string Status { get; set; } = StatusViagem.Iniciada;
    public DateTime DataSaida { get; set; } = DateTime.Now;
    public DateTime? DataChegada { get; set; }

    // Regra: faturamento = distância x valor do frete por km (viagem cancelada não fatura)
    public decimal CalcularFaturamento()
    {
        return Status == StatusViagem.Cancelada ? 0 : Math.Round(DistanciaKm * ValorPorKm, 2, MidpointRounding.AwayFromZero);
    }

    // Regras: só avança quem foi iniciada e só conclui quem está em trânsito
    public bool PodeAvancar() => Status == StatusViagem.Iniciada;

    public bool PodeConcluir() => Status == StatusViagem.EmTransito;

    public bool EstaEmAndamento() => PodeAvancar() || PodeConcluir();

    public void Avancar() => Status = StatusViagem.EmTransito;

    public void Concluir()
    {
        Status = StatusViagem.Concluida;
        DataChegada = DateTime.Now;
    }

    public void Cancelar() => Status = StatusViagem.Cancelada;
}

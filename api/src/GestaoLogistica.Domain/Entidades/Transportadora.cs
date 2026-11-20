using GestaoLogistica.Domain.Constantes;

namespace GestaoLogistica.Domain.Entidades;

public class Transportadora : EntidadeBase
{
    public string Cnpj { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public Endereco Endereco { get; set; } = new();
    public string Status { get; set; } = StatusCadastro.Ativa;
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    public bool EstaAtiva() => Status == StatusCadastro.Ativa;

    public void AlternarStatus()
    {
        Status = EstaAtiva() ? StatusCadastro.Inativa : StatusCadastro.Ativa;
    }
}

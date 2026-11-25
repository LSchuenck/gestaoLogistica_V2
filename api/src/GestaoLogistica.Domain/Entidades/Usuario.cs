using GestaoLogistica.Domain.Constantes;

namespace GestaoLogistica.Domain.Entidades;

public class Usuario : EntidadeBase
{
    // Id do administrador criado na instalação (não pode ser excluído nem desativado)
    public const int IdAdministradorMaster = 1;

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty; // a senha nunca é guardada em texto puro
    public string Perfil { get; set; } = PerfisUsuario.Operador;
    public string Status { get; set; } = StatusCadastro.Ativo;
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // true enquanto o usuário não trocar a senha temporária no primeiro acesso
    public bool TrocarSenha { get; set; }

    public bool EstaAtivo() => Status == StatusCadastro.Ativo;

    public bool EhAdministradorMaster() => Id == IdAdministradorMaster;

    public void AlternarStatus()
    {
        Status = EstaAtivo() ? StatusCadastro.Inativo : StatusCadastro.Ativo;
    }
}

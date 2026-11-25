using System.ComponentModel.DataAnnotations;

namespace GestaoLogistica.Application.DTOs;

// Dados enviados pela tela de login
public class LoginDto
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha.")]
    public string Senha { get; set; } = string.Empty;
}

// Dados devolvidos após o login com sucesso
public class LoginRespostaDto
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
}

// Nova senha escolhida no primeiro acesso
public class TrocarSenhaDto
{
    [Required(ErrorMessage = "Informe a nova senha.")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    [MaxLength(72, ErrorMessage = "O campo senha aceita no máximo 72 caracteres.")]
    public string NovaSenha { get; set; } = string.Empty;

    [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não conferem.")]
    public string ConfirmacaoSenha { get; set; } = string.Empty;
}

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

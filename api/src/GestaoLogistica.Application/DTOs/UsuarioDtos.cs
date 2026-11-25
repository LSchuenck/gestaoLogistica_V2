using System.ComponentModel.DataAnnotations;
using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.DTOs;

// Dados do usuário devolvidos pela API (sem a senha)
public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    public static UsuarioDto Converter(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            Status = usuario.Status,
            DataCadastro = usuario.DataCadastro
        };
    }
}

// Dados recebidos para cadastrar ou alterar um usuário
public class SalvarUsuarioDto
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O campo nome aceita no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150, ErrorMessage = "O campo e-mail aceita no máximo 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o perfil.")]
    public string Perfil { get; set; } = string.Empty;

    // Obrigatória no cadastro; na alteração, se vier vazia a senha atual é mantida
    [MaxLength(72, ErrorMessage = "O campo senha aceita no máximo 72 caracteres.")]
    public string? Senha { get; set; }
}

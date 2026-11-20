using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestaoLogistica.Application.Interfaces;
using GestaoLogistica.Domain.Entidades;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestaoLogistica.Infrastructure.Seguranca;

// Gera o token JWT assinado com a chave secreta definida na configuração (seção "Jwt")
public class GeradorTokenJwt : IGeradorToken
{
    private readonly IConfiguration _configuracao;

    public GeradorTokenJwt(IConfiguration configuracao)
    {
        _configuracao = configuracao;
    }

    public string GerarToken(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracao["Jwt:Chave"]!));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var minutosExpiracao = int.Parse(_configuracao["Jwt:ExpiracaoMinutos"] ?? "60");

        // Informações do usuário que vão dentro do token (nunca colocar a senha aqui)
        var informacoes = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        var token = new JwtSecurityToken(
            issuer: _configuracao["Jwt:Emissor"],
            audience: _configuracao["Jwt:Publico"],
            claims: informacoes,
            expires: DateTime.UtcNow.AddMinutes(minutosExpiracao),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

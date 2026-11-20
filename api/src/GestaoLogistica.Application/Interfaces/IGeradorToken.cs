using GestaoLogistica.Domain.Entidades;

namespace GestaoLogistica.Application.Interfaces;

// Gera o token JWT do usuário autenticado (implementado na Infrastructure)
public interface IGeradorToken
{
    string GerarToken(Usuario usuario);
}
